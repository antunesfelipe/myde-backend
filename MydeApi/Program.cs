using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Amazon.SQS;
using Amazon.Runtime;
using MydeApi.Data;
using MydeApi.Services;
using MydeApi.Modules.Auth;
using MydeApi.Modules.Clients;
using MydeApi.Modules.Proposals;
using MydeApi.Workers;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado");
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configurar AWS SQS (LocalStack)
builder.Services.AddSingleton<IAmazonSQS>(sp =>
{
    var config = new AmazonSQSConfig
    {
        ServiceURL = builder.Configuration["AWS:SQS:ServiceUrl"]
    };

    var credentials = new BasicAWSCredentials("test", "test");

    return new AmazonSQSClient(credentials, config);
});

// Configurar HttpClient para Mock Bank
builder.Services.AddHttpClient<MockBankService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MockBank:BaseUrl"] ?? "http://localhost:8001");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Registrar Services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<SqsService>();

// Registrar Auth Module
builder.Services.AddScoped<IAuthService, AuthService>();

// Registrar Clients Module
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();

// Registrar Proposals Module
builder.Services.AddScoped<IProposalRepository, ProposalRepository>();
builder.Services.AddScoped<IProposalService, ProposalService>();

// Registrar SQS Worker
builder.Services.AddHostedService<SqsWorker>();

// Configurar Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Myde API", Version = "v1" });

    // Configurar JWT no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Executar Seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Seed.Initialize(context);
}

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// REMOVIDO: Middlewares customizados que causavam conflito
// app.UseMiddleware<JwtMiddleware>();
// app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();  // ← Autenticação padrão do .NET
app.UseAuthorization();

app.MapControllers();

app.Run();
