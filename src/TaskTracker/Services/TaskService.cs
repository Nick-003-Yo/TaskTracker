using TaskTracker.Models;

namespace TaskTracker.Services;

public class TaskService
{
    private readonly List<TaskItem> tasks;

    public TaskService(List<TaskItem> tasks)
    {
        this.tasks = tasks;
    }

    public List<TaskItem> GetAll()
    {
        return tasks;
    }

    public void Add(TaskItem task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Название задачи не может быть пустым.");

        task.Id = tasks.Count == 0
            ? 1
            : tasks.Max(t => t.Id) + 1;

        task.CreatedAt = DateTime.Now;
        tasks.Add(task);
    }

    public TaskItem? GetById(int id)
    {
        return tasks.FirstOrDefault(t => t.Id == id);
    }

    public bool Delete(int id)
    {
        var task = GetById(id);

        if (task == null)
            return false;

        tasks.Remove(task);
        return true;
    }

    public List<TaskItem> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<TaskItem>();

        return tasks
            .Where(t => t.Title.Contains(
                query,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<TaskItem> FilterByStatus(TaskState status)
    {
        return tasks
            .Where(t => t.Status == status)
            .ToList();
    }

    public List<TaskItem> FilterByPriority(TaskPriority priority)
    {
        return tasks
            .Where(t => t.Priority == priority)
            .ToList();
    }
}
