using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

[ProcessDataType("PURCHASE_REQUEST", "Purchase request")]
public class PurchaseRequestData : ProcessInstanceDataBase
{
    [Required]
    [MaxLength(200)]
    public string ItemDescription { get; set; }

    [Required]
    [MaxLength(50)]
    public string Category { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(100)]
    public string VendorName { get; set; }

    [MaxLength(500)]
    public string VendorContact { get; set; }

    [MaxLength(2000)]
    public string Justification { get; set; }

    [MaxLength(50)]
    public string BudgetCode { get; set; }

    public DateTime? RequiredByDate { get; set; }

    public bool IsUrgent { get; set; }
}
