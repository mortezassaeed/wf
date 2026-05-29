# Adding a Process Instance Data Type

This guide explains how to add a new process instance data type to the workflow backend.

The current design uses:

- A data entity in `DataAccess.Entities`
- A DTO in `Services.Dtos`
- Reflection metadata from DTO fields for API documentation and Blazor forms
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

    [ProcessDataField(Label = "Claim title", Order = 10)]
    public string ClaimTitle { get; set; }

    [ProcessDataField(Label = "Total amount", Order = 20)]
    public decimal TotalAmount { get; set; }

    [ProcessDataField(Label = "Currency", Placeholder = "USD", Order = 30)]
    public string Currency { get; set; }

    [ProcessDataField(Label = "Purpose", ControlType = "textarea", Order = 40)]
    public string Purpose { get; set; }

    [ProcessDataField(Label = "Expense date", ControlType = "date", Order = 50)]
    public DateTime ExpenseDate { get; set; }

    [ProcessDataField(Label = "Has receipt", Order = 60)]
    public bool HasReceipt { get; set; }
}
```

Important:

- Inherit from `ProcessInstanceDataBaseDto`.
- Return the same `DataType` code used by the entity attribute.
- Keep DTO fields aligned with the entity fields that the API should expose.
- Add `[ProcessDataField]` when you want better labels, ordering, placeholders, help text, control type, or hidden fields.
- The metadata attribute is optional. Without it, the API still discovers public DTO properties and derives display names and field types.

Base DTO fields are not exposed as form fields:

- `id`
- `processInstanceId`
- `dataType`
- `createdAt`
- `updatedAt`

Supported field metadata:

```json
{
  "name": "claimTitle",
  "displayName": "Claim title",
  "type": "string",
  "required": true,
  "maxLength": 200,
  "minimum": null,
  "maximum": null,
  "helpText": null,
  "placeholder": null,
  "controlType": null,
  "order": 10,
  "options": []
}
```

Field type values are currently:

- `string`
- `number`
- `date`
- `boolean`
- `enum`
- `object`

Blazor uses this metadata to render forms. For example, `controlType: "textarea"` renders a textarea, `controlType: "date"` renders a date input, enum fields render select options, and boolean fields render checkboxes.

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

Frontend clients can discover available types and payload fields from:

```http
GET /api/Processes/data-types
GET /api/Processes/{processId}/data-types
```

Example response item:

```json
{
  "code": "EXPENSE_CLAIM",
  "displayName": "Expense claim",
  "entityType": "ExpenseClaimData",
  "dtoType": "ExpenseClaimDto",
  "fields": [
    {
      "name": "claimTitle",
      "displayName": "Claim title",
      "type": "string",
      "required": true,
      "maxLength": 200,
      "minimum": null,
      "maximum": null,
      "helpText": null,
      "placeholder": null,
      "controlType": null,
      "order": 10,
      "options": []
    }
  ]
}
```

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

The endpoint also checks the active workflow step. If the active `ProcessStep.CanEditData` value is `false`, the update is rejected with `400 Bad Request`.

Use `CanEditData` on process steps to control whether users can edit data at that workflow step:

```json
{
  "code": "MANAGER_APPROVAL",
  "name": "Manager approval",
  "order": 20,
  "isStart": false,
  "isEnd": false,
  "requiresApproval": true,
  "canEditData": false,
  "isActive": true
}
```

Default value is `true`, so existing editable steps keep the old behavior unless configured otherwise.

## 11. Verify The Backend

Build the API project:

```powershell
dotnet build .\WorkflowRBC\WorkflowRBC.csproj
```

Then verify:

- `GET /api/Processes/data-types` includes the new data type.
- `GET /api/Processes/data-types` includes the new DTO field schema.
- `GET /api/Processes/{processId}/data-types` returns only the configured data types for that process.
- Creating a process instance succeeds when the process allows the new data type.
- Creating a process instance fails when the process does not allow the new data type.
- `GET /api/ProcessInstances/{id}` returns the typed data in `data`.
- `PUT /api/ProcessInstances/{id}/data` updates the typed data.
- `PUT /api/ProcessInstances/{id}/data` fails when the active step has `canEditData: false`.

## 12. Verify Blazor Test UI Support

The Blazor test UI renders process data forms from the `fields` metadata returned by the data-types APIs.

After adding a new DTO and allowing it for a process, verify:

- The Start Process section shows the new data type in the process data type select.
- Changing the selected data type updates the generated form.
- Cartable renders the current instance data using the generated form.
- Read-only workflow steps disable the generated form and update button.

No per-data-type Razor form code is required for normal fields.

## Checklist

Use this checklist for every new process instance data type:

- Create `DataAccess/Entities/{Name}Data.cs`.
- Add `[ProcessDataType("CODE", "Display name")]`.
- Add `DbSet<{Name}Data>` to `WorkflowDbContext`.
- Add TPT mapping in `ConfigureProcessInstanceData`.
- Create `Services/Dtos/{Name}Dto.cs`.
- Optionally add `[ProcessDataField]` attributes to DTO properties.
- Create `Services/Resolver/{Name}DataHandler.cs`.
- Register the handler in `WorkflowRBC/Program.cs`.
- Add and review an EF Core migration.
- Apply the migration.
- Allow the data type for the target process.
- Test create, get, and update API flows.
- Verify the data-types API exposes the field schema.
- Verify Blazor renders the generated form.
