using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Dtos;

namespace Services.Resolver;

public class ProcessInstanceDataService : IProcessInstanceDataService
{
    private readonly WorkflowDbContext _context;
    public ProcessInstanceDataService(WorkflowDbContext context)
    {
        _context = context;
    }

    public IProcessDataDto Deserialize(string dataType, string dataJson)
    {
        return ProcessInstanceDataMapper.Deserialize(dataType, dataJson);
    }

    public ProcessInstanceDataBase? CreateEntity(IProcessDataDto? dto, int userId)
    {
        if (dto == null)
            return null;

        ProcessInstanceDataBase entity = dto switch
        {
            LeaveRequestDto leave => new LeaveRequestData
            {
                LeaveType = leave.LeaveType,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                TotalDays = leave.TotalDays,
                Reason = leave.Reason,
                ContactDuringLeave = leave.ContactDuringLeave,
                IsEmergency = leave.IsEmergency,
                BackupPersonId = leave.BackupPersonId
            },
            PurchaseRequestDto purchase => new PurchaseRequestData
            {
                ItemDescription = purchase.ItemDescription,
                Category = purchase.Category,
                Quantity = purchase.Quantity,
                UnitPrice = purchase.UnitPrice,
                TotalAmount = purchase.TotalAmount,
                VendorName = purchase.VendorName,
                VendorContact = purchase.VendorContact,
                Justification = purchase.Justification,
                BudgetCode = purchase.BudgetCode,
                RequiredByDate = purchase.RequiredByDate,
                IsUrgent = purchase.IsUrgent
            },
            _ => throw new InvalidOperationException($"Unknown data type: {dto.DataType}")
        };

        SetCommonFields(entity, dto.DataType, userId, isNew: true);
        ApplyRequiredStringDefaults(entity);
        return entity;
    }

    public async Task<IProcessDataDto?> GetAsync(int processInstanceId)
    {
        var data = await _context.ProcessInstanceDataBase
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.ProcessInstanceId == processInstanceId);

        if (data == null)
            return null;

        return data.DataType switch
        {
            "LEAVE_REQUEST" => ToDto(await _context.LeaveRequestData
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.ProcessInstanceId == processInstanceId)),
            "PURCHASE_REQUEST" => ToDto(await _context.PurchaseRequestData
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.ProcessInstanceId == processInstanceId)),
            _ => null
        };
    }

    public async Task<IProcessDataDto> UpsertAsync(int processInstanceId, IProcessDataDto dto, int userId)
    {
        var existing = await _context.ProcessInstanceDataBase
            .FirstOrDefaultAsync(d => d.ProcessInstanceId == processInstanceId);

        if (existing != null)
        {
            _context.ProcessInstanceDataBase.Remove(existing);
            await _context.SaveChangesAsync();
        }

        var entity = CreateEntity(dto, userId)
            ?? throw new InvalidOperationException("Process instance data is required.");

        entity.ProcessInstanceId = processInstanceId;
        await _context.ProcessInstanceDataBase.AddAsync(entity);
        await _context.SaveChangesAsync();

        return await GetAsync(processInstanceId)
            ?? throw new InvalidOperationException("Process instance data was saved but could not be loaded.");
    }

    private static ProcessInstanceDataBaseDto? ToDto(ProcessInstanceDataBase? entity)
    {
        return entity switch
        {
            LeaveRequestData leave => new LeaveRequestDto
            {
                Id = leave.Id,
                ProcessInstanceId = leave.ProcessInstanceId,
                CreatedAt = leave.CreatedAt,
                UpdatedAt = leave.UpdatedAt,
                LeaveType = leave.LeaveType,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                TotalDays = leave.TotalDays,
                Reason = leave.Reason,
                ContactDuringLeave = leave.ContactDuringLeave,
                IsEmergency = leave.IsEmergency,
                BackupPersonId = leave.BackupPersonId
            },
            PurchaseRequestData purchase => new PurchaseRequestDto
            {
                Id = purchase.Id,
                ProcessInstanceId = purchase.ProcessInstanceId,
                CreatedAt = purchase.CreatedAt,
                UpdatedAt = purchase.UpdatedAt,
                ItemDescription = purchase.ItemDescription,
                Category = purchase.Category,
                Quantity = purchase.Quantity,
                UnitPrice = purchase.UnitPrice,
                TotalAmount = purchase.TotalAmount,
                VendorName = purchase.VendorName,
                VendorContact = purchase.VendorContact,
                Justification = purchase.Justification,
                BudgetCode = purchase.BudgetCode,
                RequiredByDate = purchase.RequiredByDate,
                IsUrgent = purchase.IsUrgent
            },
            _ => null
        };
    }

    private static void SetCommonFields(ProcessInstanceDataBase entity, string dataType, int userId, bool isNew)
    {
        entity.DataType = dataType;

        if (isNew)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedByUserId = userId;
        }
        else
        {
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = userId;
        }
    }

    private static void ApplyRequiredStringDefaults(ProcessInstanceDataBase entity)
    {
        switch (entity)
        {
            case LeaveRequestData leave:
                leave.LeaveType ??= string.Empty;
                leave.Reason ??= string.Empty;
                leave.ContactDuringLeave ??= string.Empty;
                break;
            case PurchaseRequestData purchase:
                purchase.ItemDescription ??= string.Empty;
                purchase.Category ??= string.Empty;
                purchase.VendorName ??= string.Empty;
                purchase.VendorContact ??= string.Empty;
                purchase.Justification ??= string.Empty;
                purchase.BudgetCode ??= string.Empty;
                break;
        }
    }
}
