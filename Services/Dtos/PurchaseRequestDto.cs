namespace Services.Dtos;

public class PurchaseRequestDto : ProcessInstanceDataBaseDto
{
    public override string DataType => "PURCHASE_REQUEST";

    [ProcessDataField(Label = "Item description", Order = 10)]
    public string ItemDescription { get; set; }

    [ProcessDataField(Label = "Category", Order = 20)]
    public string Category { get; set; }

    [ProcessDataField(Label = "Quantity", Order = 30)]
    public int Quantity { get; set; }

    [ProcessDataField(Label = "Unit price", Order = 40)]
    public decimal UnitPrice { get; set; }

    [ProcessDataField(Label = "Total amount", HelpText = "Total requested amount for this purchase.", Order = 50)]
    public decimal TotalAmount { get; set; }

    [ProcessDataField(Label = "Vendor name", Order = 60)]
    public string VendorName { get; set; }

    [ProcessDataField(Label = "Vendor contact", Order = 70)]
    public string VendorContact { get; set; }

    [ProcessDataField(Label = "Justification", ControlType = "textarea", Order = 80)]
    public string Justification { get; set; }

    [ProcessDataField(Label = "Budget code", Order = 90)]
    public string BudgetCode { get; set; }

    [ProcessDataField(Label = "Required by date", ControlType = "date", Order = 100)]
    public DateTime? RequiredByDate { get; set; }

    [ProcessDataField(Label = "Urgent request", Order = 110)]
    public bool IsUrgent { get; set; }
}
