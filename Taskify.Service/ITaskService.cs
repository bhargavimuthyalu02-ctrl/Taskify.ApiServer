using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taskify.Model;

namespace Taskify.Service
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<TaskItem> CreateAsync(TaskItem item);
        Task<TaskItem> UpdateAsync(TaskItem item);
        Task DeleteAsync(Guid id);
    }
}
