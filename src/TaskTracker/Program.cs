using System.Globalization;
using System.Text;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.Modules.Reports;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var storage = new TaskStorage("data/tasks.json");
var tasks = storage.Load();
var taskService = new TaskService(tasks);

while (true)
{
    Console.Clear();
    Console.WriteLine("==============================");
    Console.WriteLine("        TASKTRACKER 2.1.1");
    Console.WriteLine("==============================");
    Console.WriteLine("1. Показать задачи");
    Console.WriteLine("2. Создать задачу");
    Console.WriteLine("3. Изменить задачу");
    Console.WriteLine("4. Удалить задачу");
    Console.WriteLine("5. Поиск");
    Console.WriteLine("6. Фильтр по статусу");
    Console.WriteLine("7. Фильтр по приоритету");
    Console.WriteLine("8. Отчёты и экспорт");
    Console.WriteLine("0. Выход");
    Console.Write("\nВыберите действие: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ShowTasks(taskService.GetAll());
            break;
        case "2":
            AddTask(taskService);
            storage.Save(taskService.GetAll());
            break;
        case "3":
            EditTask(taskService);
            storage.Save(taskService.GetAll());
            break;
        case "4":
            DeleteTask(taskService);
            storage.Save(taskService.GetAll());
            break;
        case "5":
            SearchTasks(taskService);
            break;
        case "6":
            FilterByStatus(taskService);
            break;
        case "7":
            FilterByPriority(taskService);
            break;
        case "8":
            new ReportMenu(taskService).Run();
            Pause();
            break;
        case "0":
            storage.Save(taskService.GetAll());
            return;
        default:
            Console.WriteLine("Неизвестная команда.");
            Pause();
            break;
    }
}

static void ShowTasks(List<TaskItem> tasks)
{
    Console.Clear();

    if (tasks.Count == 0)
    {
        Console.WriteLine("Задачи отсутствуют.");
        Pause();
        return;
    }

    foreach (var task in tasks)
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"ID: {task.Id}");
        Console.WriteLine($"Название: {task.Title}");
        Console.WriteLine($"Исполнитель: {task.Assignee}");
        Console.WriteLine($"Приоритет: {task.Priority}");
        Console.WriteLine($"Статус: {task.Status}");
        Console.WriteLine(
            $"Срок: {(task.DueDate.HasValue ? task.DueDate.Value.ToShortDateString() : "не указан")}");
    }

    Pause();
}

static TaskPriority ReadPriority(TaskPriority current)
{
    Console.Write($"Приоритет 1-Low, 2-Medium, 3-High ({current}, Enter - оставить): ");
    return Console.ReadLine() switch
    {
        "1" => TaskPriority.Low,
        "2" => TaskPriority.Medium,
        "3" => TaskPriority.High,
        _ => current
    };
}

static TaskState ReadStatus(TaskState current)
{
    Console.Write($"Статус 1-New, 2-InProgress, 3-Completed ({current}, Enter - оставить): ");
    return Console.ReadLine() switch
    {
        "1" => TaskState.New,
        "2" => TaskState.InProgress,
        "3" => TaskState.Completed,
        _ => current
    };
}

static DateTime? ReadDueDate(DateTime? current)
{
    Console.Write("Срок дд.мм.гггг (Enter - оставить): ");
    var input = Console.ReadLine();

    if (DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var date))
        return date;

    return current;
}

static void AddTask(TaskService service)
{
    Console.Clear();

    Console.Write("Название: ");
    var title = Console.ReadLine();

    Console.Write("Описание: ");
    var description = Console.ReadLine();

    Console.Write("Исполнитель: ");
    var assignee = Console.ReadLine();

    var task = new TaskItem
    {
        Title = title ?? string.Empty,
        Description = description ?? string.Empty,
        Assignee = assignee ?? string.Empty,
        Priority = ReadPriority(TaskPriority.Medium),
        Status = TaskState.New,
        DueDate = ReadDueDate(null)
    };

    try
    {
        service.Add(task);
        Console.WriteLine("Задача создана.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }

    Pause();
}

static void EditTask(TaskService service)
{
    Console.Clear();

    Console.Write("Введите ID задачи: ");

    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Некорректный ID.");
        Pause();
        return;
    }

    var task = service.GetById(id);

    if (task == null)
    {
        Console.WriteLine("Задача не найдена.");
        Pause();
        return;
    }

    Console.Write($"Название ({task.Title}): ");
    var title = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(title))
        task.Title = title;

    Console.Write($"Исполнитель ({task.Assignee}): ");
    var assignee = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(assignee))
        task.Assignee = assignee;

    task.Status = ReadStatus(task.Status);
    task.Priority = ReadPriority(task.Priority);
    task.DueDate = ReadDueDate(task.DueDate);

    Console.WriteLine("Задача изменена.");
    Pause();
}

static void DeleteTask(TaskService service)
{
    Console.Clear();

    Console.Write("Введите ID задачи: ");

    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Некорректный ID.");
        Pause();
        return;
    }

    if (service.Delete(id))
        Console.WriteLine("Задача удалена.");
    else
        Console.WriteLine("Задача не найдена.");

    Pause();
}

static void SearchTasks(TaskService service)
{
    Console.Clear();

    Console.Write("Введите текст поиска: ");
    var query = Console.ReadLine() ?? string.Empty;

    var result = service.Search(query);
    ShowTasks(result);
}

static void FilterByStatus(TaskService service)
{
    Console.Clear();

    Console.WriteLine("1. New");
    Console.WriteLine("2. InProgress");
    Console.WriteLine("3. Completed");
    Console.Write("Выберите статус: ");

    var input = Console.ReadLine();

    var status = input switch
    {
        "1" => TaskState.New,
        "2" => TaskState.InProgress,
        "3" => TaskState.Completed,
        _ => TaskState.New
    };

    ShowTasks(service.FilterByStatus(status));
}

static void FilterByPriority(TaskService service)
{
    Console.Clear();

    Console.WriteLine("1. Low");
    Console.WriteLine("2. Medium");
    Console.WriteLine("3. High");
    Console.Write("Выберите приоритет: ");

    var input = Console.ReadLine();

    var priority = input switch
    {
        "1" => TaskPriority.Low,
        "2" => TaskPriority.Medium,
        "3" => TaskPriority.High,
        _ => TaskPriority.Medium
    };

    ShowTasks(service.FilterByPriority(priority));
}

static void Pause()
{
    Console.WriteLine();
    Console.WriteLine("Нажмите Enter для продолжения...");
    Console.ReadLine();
}
