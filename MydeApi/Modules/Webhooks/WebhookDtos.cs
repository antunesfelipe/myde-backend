namespace MydeApi.Modules.Webhooks;

public class BankCallbackRequest
{
    public string Protocol { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public BankCallbackData? Data { get; set; }
}

public class BankCallbackData
{
    public decimal? InterestRate { get; set; }
    public decimal? InstallmentValue { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
}
