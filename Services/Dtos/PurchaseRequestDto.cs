namespace Services.Dtos;

public class PurchaseRequestDto : ProcessInstanceDataBaseDto
{
    public override string DataType => "PURCHASE_REQUEST";

    public string ItemDescription { get; set; }
    public string Category { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string VendorName { get; set; }
    public string VendorContact { get; set; }
    public string Justification { get; set; }
    public string BudgetCode { get; set; }
    public DateTime? RequiredByDate { get; set; }
    public bool IsUrgent { get; set; }
}
