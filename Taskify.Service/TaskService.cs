using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Taskify.Data;
using Taskify.Model;

namespace Taskify.Service
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _db;

        public TaskService(TaskDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _db.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
            return await _db.Tasks.FindAsync(id);
        }

        public async Task<TaskItem> CreateAsync(TaskItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new ArgumentException("Title is required.", nameof(item.Title));

            item.Id = Guid.NewGuid();

            // Ensure timestamps are set on the client so EF won't attempt to read nulls back from the DB
            var utcNow = DateTime.UtcNow;
            if (item.CreatedAt == null || item.CreatedAt == default)
                item.CreatedAt = utcNow;
            if (item.UpdatedAt == null || item.UpdatedAt == default)
                item.UpdatedAt = utcNow;

            _db.Tasks.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem item)
        {
            if (item.Id == Guid.Empty)
                throw new ArgumentException("Id is required for update.", nameof(item.Id));

            var existing = await _db.Tasks.FindAsync(item.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Task with id {item.Id} not found.");

            existing.Title = item.Title;
            existing.Description = item.Description;
            existing.DueDate = item.DueDate;
            existing.Priority = item.Priority;
            existing.Status = item.Status;
            existing.UserEmail = item.UserEmail;

            // Set UpdatedAt on update to avoid NULL issues
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _db.Tasks.FindAsync(id);
            if (existing != null)
            {
                _db.Tasks.Remove(existing);
                await _db.SaveChangesAsync();
            }
        }
    }
}
