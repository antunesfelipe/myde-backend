# Myde Backend - Teste Técnico .NET

API de gerenciamento de propostas de crédito desenvolvida em .NET 8 com arquitetura modular.

## 🚀 Tecnologias

- **.NET 8** - Framework principal
- **PostgreSQL 16** - Banco de dados
- **Entity Framework Core** - ORM
- **JWT** - Autenticação
- **AWS SQS (LocalStack)** - Fila de mensagens
- **Docker** - Containerização

## 📋 Pré-requisitos

- .NET SDK 8.0+
- Docker Desktop
- Git

## 🔧 Como rodar

### 1. Clonar repositório

```bash
ggit clone https://github.com/antunesfelipe/myde-backend.git
cd myde-backend
```

### 2. Subir infraestrutura (Docker)

```bash
docker-compose up -d
```

Aguarde ~30 segundos para tudo subir.

### 3. Verificar serviços

```bash
# PostgreSQL
docker ps | grep postgres

# LocalStack (SQS)
curl http://localhost:4566/_localstack/health

# Mock Bank
curl http://localhost:8001/health
```

### 4. Instalar dependências

```bash
cd MydeApi
dotnet restore
```

### 5. Criar migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 6. Rodar API

```bash
dotnet run
```

API disponível em: `http://localhost:5000`

Swagger em: `http://localhost:5000/swagger`

## 🔐 Dados de Teste (Seed)

### Tenants

| Nome | CNPJ | Usuário | Senha |
|------|------|---------|-------|
| Banco A | 12345678000100 | operador@bancoa.com | senha123 |
| Banco B | 98765432000199 | operador@bancob.com | senha123 |

## 📡 Endpoints

### Auth

- `POST /api/auth/login` - Login (retorna JWT)

### Clients

- `GET /api/clients` - Listar clientes (paginado)
- `GET /api/clients/{id}` - Buscar cliente
- `POST /api/clients` - Criar cliente
- `PUT /api/clients/{id}` - Atualizar cliente

### Proposals

- `GET /api/proposals` - Listar propostas (paginado)
- `GET /api/proposals/{id}` - Buscar proposta
- `POST /api/proposals/simulate` - Simular crédito
- `POST /api/proposals/{id}/submit` - Submeter proposta

### Webhooks

- `POST /api/webhooks/bank-callback` - Callback do banco mock

## 🧪 Testar fluxo completo

### 1. Login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "operador@bancoa.com",
    "password": "senha123"
  }'
```

Copie o `token` retornado.

### 2. Criar cliente

```bash
curl -X POST http://localhost:5000/api/clients \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "name": "João Silva",
    "cpf": "12345678900",
    "birthDate": "1990-01-15",
    "phone": "11999999999"
  }'
```

Copie o `id` do cliente.

### 3. Simular crédito

```bash
curl -X POST http://localhost:5000/api/proposals/simulate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "clientId": "ID_DO_CLIENTE",
    "amount": 5000,
    "installments": 12
  }'
```

Aguarde ~5 segundos e consulte a proposta para ver o resultado.

## 🏗️ Arquitetura
MydeApi/
├── Modules/              # Módulos por domínio
│   ├── Auth/            # Autenticação
│   ├── Clients/         # Clientes
│   ├── Proposals/       # Propostas
│   └── Webhooks/        # Webhooks
├── Models/              # Entidades do banco
├── Data/                # DbContext + Seed
├── Services/            # Serviços compartilhados
├── Workers/             # Background workers
└── Middleware/          # Middleware customizado

## 📊 Multi-tenancy

Todas as queries filtram por `tenant_id` automaticamente.

Tenant A **NÃO VÊ** dados do Tenant B.

## 🔄 Fluxo Assíncrono

1. Endpoint recebe requisição
2. Cria proposta com status `pending`
3. Enfileira job no SQS
4. Retorna `202 Accepted`
5. Worker processa fila
6. Atualiza status para `processing`
7. Chama Mock Bank
8. Banco retorna protocolo
9. Aguarda webhook
10. Webhook atualiza proposta

## 🧪 Testes

```bash
# Rodar testes (quando implementados)
dotnet test
```

## 📝 Decisões Técnicas


Conforme solicitado pela equipe Myde foi usado .net para este desafio.

### Gaps conhecidos

- Primeira experiência adaptando arquitetura Python para .NET
- Código reflete aprendizado intensivo focado em funcionalidade



## 👤 Autor

**Felipe Antunes**

## 📄 Licença

MIT
