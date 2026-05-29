# Adding a Process Instance Data Type

This guide explains how to add a new process instance data type to the workflow backend.

The current design uses:

- A data entity in `DataAccess.Entities`
- A DTO in `Services.Dtos`
- A data handler in `Services.Resolver`
- EF Core TPT mapping in `WorkflowDbContext`
- Dependency injection registration in `WorkflowRBC/Program.cs`
- A database migration

Use the existing leave request and purchase request types as references:

- `DataAccess/Entities/LeaveRequestData.cs`
- `Services/Dtos/LeaveRequestDto.cs`
- `Services/Resolver/LeaveRequestDataHandler.cs`

## 1. Choose The Data Type Code

Choose a stable, uppercase code. This code is stored in the database and sent by API clients.

Example:

```text
EXPENSE_CLAIM
```

Do not rename this code later unless you also migrate existing database rows.

## 2. Create The Data Entity

Create a new entity under:

```text
DataAccess/Entities
```

Example:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

[ProcessDataType("EXPENSE_CLAIM", "Expense claim")]
public class ExpenseClaimData : ProcessInstanceDataBase
{
    [Required]
    [MaxLength(200)]
    public string ClaimTitle { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(3)]
    public string Currency { get; set; }

    [MaxLength(2000)]
    public string Purpose { get; set; }

    public DateTime ExpenseDate { get; set; }

    public bool HasReceipt { get; set; }
}
```

Important:

- Inherit from `ProcessInstanceDataBase`.
- Add `[ProcessDataType("CODE", "Display name")]`.
- Use validation attributes such as `[Required]`, `[MaxLength]`, and `[Column(TypeName = "decimal(18,2)")]` where needed.
- Required string properties should be populated by the handler to avoid null database values.

## 3. Register The Entity In `WorkflowDbContext`

Open:

```text
DataAccess/Entities/WorkflowDbContext.cs
```

Add a `DbSet`:

```csharp
public DbSet<ExpenseClaimData> ExpenseClaimData { get; set; }
```

Then add the TPT table mapping inside `ConfigureProcessInstanceData`:

```csharp
modelBuilder.Entity<ExpenseClaimData>().ToTable("ExpenseClaimData");
```

This lets EF Core create a derived table for the new data type.

## 4. Create The DTO

Create a DTO under:

```text
Services/Dtos
```

Example:

```csharp
namespace Services.Dtos;

public class ExpenseClaimDto : ProcessInstanceDataBaseDto
{
    public override string DataType => "EXPENSE_CLAIM";

    public string ClaimTitle { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public string Purpose { get; set; }
    public DateTime ExpenseDate { get; set; }
    public bool HasReceipt { get; set; }
}
```

Important:

- Inherit from `ProcessInstanceDataBaseDto`.
- Return the same `DataType` code used by the entity attribute.
- Keep DTO fields aligned with the entity fields that the API should expose.

## 5. Create The Data Handler

Create a handler under:

```text
Services/Resolver
```

Example:

```csharp
using DataAccess.Entities;
using Services.Dtos;

namespace Services.Resolver;

public sealed class ExpenseClaimDataHandler : ProcessDataHandlerBase<ExpenseClaimDto, ExpenseClaimData>
{
    public override string DataType => "EXPENSE_CLAIM";

    protected override ExpenseClaimData CreateEntity(ExpenseClaimDto dto)
    {
        return new ExpenseClaimData
        {
            ClaimTitle = dto.ClaimTitle ?? string.Empty,
            TotalAmount = dto.TotalAmount,
            Currency = dto.Currency ?? string.Empty,
            Purpose = dto.Purpose ?? string.Empty,
            ExpenseDate = dto.ExpenseDate,
            HasReceipt = dto.HasReceipt
        };
    }

    protected override IProcessDataDto ToDto(ExpenseClaimData entity)
    {
        return new ExpenseClaimDto
        {
            Id = entity.Id,
            ProcessInstanceId = entity.ProcessInstanceId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            ClaimTitle = entity.ClaimTitle ?? string.Empty,
            TotalAmount = entity.TotalAmount,
            Currency = entity.Currency ?? string.Empty,
            Purpose = entity.Purpose ?? string.Empty,
            ExpenseDate = entity.ExpenseDate,
            HasReceipt = entity.HasReceipt
        };
    }
}
```

Important:

- The handler owns JSON deserialization through `ProcessDataHandlerBase`.
- The handler owns DTO-to-entity mapping.
- The handler owns entity-to-DTO mapping.
- The handler should protect required strings with `?? string.Empty`.

## 6. Register The Handler In DI

Open:

```text
WorkflowRBC/Program.cs
```

Register the new handler:

```csharp
builder.Services.AddScoped<IProcessDataHandler, ExpenseClaimDataHandler>();
```

Keep it near the existing process data handler registrations.

## 7. Add A Database Migration

Run a migration after adding the entity and DbContext mapping.

Example:

```powershell
dotnet ef migrations add AddExpenseClaimData --project DataAccess --startup-project WorkflowRBC
dotnet ef database update --project DataAccess --startup-project WorkflowRBC
```

Check the generated migration before applying it. It should create a new TPT table for the derived data entity.

## 8. Allow The Data Type For A Process

A data type can exist in code but still not be allowed for a specific process.

Use the existing process data type API to add the new code to the process allowed data types:

```http
PUT /api/Processes/{processId}/data-types
```

Body:

```json
{
  "dataTypes": [
    "EXPENSE_CLAIM"
  ]
}
```

The create instance endpoint checks this list before accepting process instance data.

## 9. Create A Process Instance With The New Data Type

Use:

```http
POST /api/ProcessInstances
```

Example body:

```json
{
  "processCode": "EXPENSE_PROCESS",
  "title": "Taxi and lunch reimbursement",
  "dataType": "EXPENSE_CLAIM",
  "data": {
    "claimTitle": "Client visit expenses",
    "totalAmount": 120.50,
    "currency": "USD",
    "purpose": "Client meeting travel",
    "expenseDate": "2026-05-29T00:00:00",
    "hasReceipt": true
  }
}
```

The model binder reads `dataType`, then asks `IProcessInstanceDataService` to deserialize `data` using the registered handler.

## 10. Update Existing Instance Data

Use:

```http
PUT /api/ProcessInstances/{processInstanceId}/data
```

Example body:

```json
{
  "dataType": "EXPENSE_CLAIM",
  "userId": 1,
  "data": {
    "claimTitle": "Updated client visit expenses",
    "totalAmount": 135.75,
    "currency": "USD",
    "purpose": "Client meeting travel and parking",
    "expenseDate": "2026-05-29T00:00:00",
    "hasReceipt": true
  }
}
```

This endpoint updates existing data. It returns `404 Not Found` if the process instance has no data row.

## 11. Verify The Backend

Build the API project:

```powershell
dotnet build .\WorkflowRBC\WorkflowRBC.csproj
```

Then verify:

- `GET /api/Processes/data-types` includes the new data type.
- Creating a process instance succeeds when the process allows the new data type.
- Creating a process instance fails when the process does not allow the new data type.
- `GET /api/ProcessInstances/{id}` returns the typed data in `data`.
- `PUT /api/ProcessInstances/{id}/data` updates the typed data.

## 12. Optional: Add Blazor Test UI Support

The backend can support a new data type without changing Blazor.

If the Blazor test UI should edit or display the new type, update:

```text
WorkflowRBC.Blazor/Models/WorkflowApiModels.cs
WorkflowRBC.Blazor/Services/WorkflowApiClient.cs
WorkflowRBC.Blazor/Components/Pages/Home.razor
WorkflowRBC.Blazor/Components/Pages/Cartable.razor
WorkflowRBC.Blazor/Components/Pages/Archive.razor
```

Typical Blazor changes:

- Add a DTO model for the new data type.
- Add a data type constant.
- Add form rendering for the new type.
- Add JSON deserialization for the selected type.
- Add update API client method.
- Add display support in archive/cartable pages.

## Checklist

Use this checklist for every new process instance data type:

- Create `DataAccess/Entities/{Name}Data.cs`.
- Add `[ProcessDataType("CODE", "Display name")]`.
- Add `DbSet<{Name}Data>` to `WorkflowDbContext`.
- Add TPT mapping in `ConfigureProcessInstanceData`.
- Create `Services/Dtos/{Name}Dto.cs`.
- Create `Services/Resolver/{Name}DataHandler.cs`.
- Register the handler in `WorkflowRBC/Program.cs`.
- Add and review an EF Core migration.
- Apply the migration.
- Allow the data type for the target process.
- Test create, get, and update API flows.
- Optionally add Blazor UI support.
