using DataAccess.Entities;
using DataAccess.Enums;

namespace Services;

public interface IProcessInstanceRepository : IRepository<ProcessInstance>
{
    Task<ProcessInstance> GetWithStepsAsync(int instanceId);
    Task<ProcessInstance> GetWithHistoryAsync(int instanceId);
    Task<IEnumerable<ProcessInstance>> GetByUserAsync(int userId);
    Task<IEnumerable<ProcessInstance>> GetByStateAsync(ProcessInstanceState state);
    Task<ProcessInstanceStep> GetCurrentStepAsync(int instanceId);
    Task<IEnumerable<ProcessInstanceStep>> GetPendingStepsByUserAsync(int userId);
}
