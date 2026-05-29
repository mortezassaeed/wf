using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities;

[ProcessDataType("REOPENING_SYMBOL", "Reopening symbol")]
public class ReopeningSymbolData : ProcessInstanceDataBase
{
    [Required]
    [MaxLength(200)]
    public required string SymbolName { get; set; }
    public int? RequestCount { get; set; }
    public decimal? ClosingPrice { get; set; }
    [Required]
    public DateTime ClosingDate { get; set; }
    [MaxLength(2000)]
    public string Description { get; set; }
}
