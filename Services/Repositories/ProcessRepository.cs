using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class ProcessRepository : Repository<Process>, IProcessRepository
{
    public ProcessRepository(WorkflowDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Process>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.ProcessAllowedDataTypes)
            .Include(p => p.ProcessSteps.Where(ps => !ps.IsDeleted))
            .Include(ps => ps.ProcessStepActions.Where(psa => !psa.IsDeleted))
            .ToListAsync();
    }

    public async Task<Process> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(p => p.ProcessAllowedDataTypes)
            .Include(p => p.ProcessSteps.Where(ps => !ps.IsDeleted))
            .Include(ps => ps.ProcessStepActions.Where(psa => !psa.IsDeleted))
            .FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<Process> GetWithStepsAsync(int processId)
    {
        return await _dbSet
            .Include(p => p.ProcessAllowedDataTypes)
            .Include(p => p.ProcessSteps.Where(ps => !ps.IsDeleted))
            .Include(ps => ps.ProcessStepActions.Where(psa => !psa.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == processId);
    }

    public async Task<IEnumerable<Process>> GetActiveProcessesAsync()
    {
        return await _dbSet
            .Include(p => p.ProcessAllowedDataTypes)
            .Where(p => p.IsActive)
            .ToListAsync();
    }
}
