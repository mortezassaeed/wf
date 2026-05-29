using DataAccess.Entities;
using Services.Dtos;

namespace Services.Resolver;

public sealed class PurchaseRequestDataHandler : ProcessDataHandlerBase<PurchaseRequestDto, PurchaseRequestData>
{
    public override string DataType => "PURCHASE_REQUEST";

    protected override PurchaseRequestData CreateEntity(PurchaseRequestDto dto)
    {
        return new PurchaseRequestData
        {
            ItemDescription = dto.ItemDescription ?? string.Empty,
            Category = dto.Category ?? string.Empty,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            TotalAmount = dto.TotalAmount,
            VendorName = dto.VendorName ?? string.Empty,
            VendorContact = dto.VendorContact ?? string.Empty,
            Justification = dto.Justification ?? string.Empty,
            BudgetCode = dto.BudgetCode ?? string.Empty,
            RequiredByDate = dto.RequiredByDate,
            IsUrgent = dto.IsUrgent
        };
    }

    protected override IProcessDataDto ToDto(PurchaseRequestData entity)
    {
        return new PurchaseRequestDto
        {
            Id = entity.Id,
            ProcessInstanceId = entity.ProcessInstanceId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            ItemDescription = entity.ItemDescription ?? string.Empty,
            Category = entity.Category ?? string.Empty,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice,
            TotalAmount = entity.TotalAmount,
            VendorName = entity.VendorName ?? string.Empty,
            VendorContact = entity.VendorContact ?? string.Empty,
            Justification = entity.Justification ?? string.Empty,
            BudgetCode = entity.BudgetCode ?? string.Empty,
            RequiredByDate = entity.RequiredByDate,
            IsUrgent = entity.IsUrgent
        };
    }
}


public sealed class ReopeningSymbolDataHandler : ProcessDataHandlerBase<ReopeningSymbolDto, ReopeningSymbolData>
{
    public override string DataType => "REOPENING_SYMBOL";

    protected override ReopeningSymbolData CreateEntity(ReopeningSymbolDto dto)
    {
        return new ReopeningSymbolData
        {
            SymbolName = dto.SymbolName,
            ClosingDate = dto.ClosingDate,
            ClosingPrice = dto.ClosingPrice,
            Description = dto.Description,
        };
    }

    protected override IProcessDataDto ToDto(ReopeningSymbolData entity)
    {
        return new ReopeningSymbolDto
        {
            Id = entity.Id,
            ProcessInstanceId = entity.ProcessInstanceId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            SymbolName = entity.SymbolName,
            ClosingDate = entity.ClosingDate,
            ClosingPrice = entity.ClosingPrice,
            Description = entity.Description,
        };
    }
}
