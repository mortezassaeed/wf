namespace Services.Dtos;

public class ExpenseClaimDto : ProcessInstanceDataBaseDto
{
    public override string DataType => "EXPENSE_CLAIM";

    public string ClaimTitle { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string ProjectCode { get; set; }
    public string Purpose { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string ReceiptAttachmentPath { get; set; }
    public bool HasReceipt { get; set; }
    public List<ExpenseClaimItemDto> Items { get; set; } = new();
}
