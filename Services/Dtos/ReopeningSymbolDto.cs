namespace Services.Dtos;

public class ReopeningSymbolDto : ProcessInstanceDataBaseDto
{
    public override string DataType => "REOPENING_SYMBOL";

    [ProcessDataField(Label = "نام نماد", Order = 1)]
    public required string SymbolName { get; set; }
    [ProcessDataField(Label = "تعداد درخواست", Order = 2)]
    public int? RequestCount { get; set; }
    [ProcessDataField(Label = "ارز قبل از بسته شدن", Order = 3)]
    public decimal? ClosingPrice { get; set; }
    [ProcessDataField(Label = "تاریخ بسته شدن", Order = 4)]
    public DateTime ClosingDate { get; set; }
    [ProcessDataField(Label = "توضیحات", Order = 5)]
    public string Description { get; set; }
}