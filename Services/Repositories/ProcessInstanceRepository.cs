using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class ProcessInstanceRepository : Repository<ProcessInstance>, IProcessInstanceRepository
{
    public ProcessInstanceRepository(WorkflowDbContext context) : base(context)
    {
    }

    public async Task<ProcessInstance> GetWithStepsAsync(int instanceId)
    {
        return await _dbSet
            .Include(pi => pi.Process)
            .Include(pi => pi.ProcessInstanceSteps.Where(pis => !pis.IsDeleted))
                .ThenInclude(pis => pis.ProcessStep)
            .Include(pi => pi.ProcessInstanceHistories)
                .ThenInclude(h => h.ProcessStep)
            .Include(m => m.Data)
            .FirstOrDefaultAsync(pi => pi.Id == instanceId);
    }

    public async Task<ProcessInstance> GetWithHistoryAsync(int instanceId)
    {
        return await _dbSet
            .Include(pi => pi.ProcessInstanceHistories)
                .ThenInclude(h => h.ProcessStep)
            .FirstOrDefaultAsync(pi => pi.Id == instanceId);
    }

    public async Task<IEnumerable<ProcessInstance>> GetByUserAsync(int userId)
    {
        return await _dbSet
            .Where(pi => pi.CreatedByUserId == userId)
            .Include(pi => pi.Process)
            .OrderByDescending(pi => pi.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProcessInstance>> GetByStateAsync(ProcessInstanceState state)
    {
        return await _dbSet
            .Where(pi => pi.State == state)
            .Include(pi => pi.Process)
            .ToListAsync();
    }

    public async Task<ProcessInstanceStep> GetCurrentStepAsync(int instanceId)
    {
        return await _context.ProcessInstanceSteps
            .Include(pis => pis.ProcessStep)
            .Where(pis => pis.ProcessInstanceId == instanceId &&
                          pis.State == ProcessInstanceStepState.Active)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ProcessInstanceStep>> GetPendingStepsByUserAsync(int userId)
    {
        return await _context.ProcessInstanceSteps
            .Include(pis => pis.ProcessInstance)
                .ThenInclude(pi => pi.Process)
            .Include(pis => pis.ProcessStep)
            .Where(pis => pis.AssignedToUserId == userId &&
                          pis.State == ProcessInstanceStepState.Active)
            //.OrderBy(pis => pis.DueDate)
            .ToListAsync();
    }
}
