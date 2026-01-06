using System;
using System.ComponentModel.DataAnnotations;
using Taskify.Model;

namespace Taskify.Api.Controllers
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        
        public DateTime? DueDate { get; set; }
        public Taskify.Model.TaskPriority Priority { get; set; }
        public Taskify.Model.TaskStatus Status { get; set; }
        [EmailAddress]
		public string OwnerEmail { get; set; } = default!; // API-facing name
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime? DueDate { get; set; }
        public Taskify.Model.TaskPriority Priority { get; set; }
        public Taskify.Model.TaskStatus Status { get; set; }
        public string OwnerEmail { get; set; } = default!;
    }

    public class UpdateTaskDto : CreateTaskDto
    {
        public Guid Id { get; set; }
    }
}
