using DataAccess.Entities;

namespace Services;

public interface IProcessRepository : IRepository<Process>
{
    Task<Process> GetByCodeAsync(string code);
    Task<Process> GetWithStepsAsync(int processId);
    Task<IEnumerable<Process>> GetActiveProcessesAsync();
}
