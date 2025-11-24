using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services;

public class TodoService : ITodoService
{
    private readonly IRepository<Todo> _todoRepo;
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public TodoService(
        IRepository<Todo> todoRepo,
        IRepository<User> userRepo,
        IUnitOfWork uow,
        IMapper mapper)
    {
        _todoRepo = todoRepo;
        _userRepo = userRepo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<TodoDto> CreateTodoAsync(CreateTodoDto dto, CancellationToken ct = default)
    {
        // شيك إذا اليوزر موجود
        var user = await _userRepo.GetByIdAsync(dto.UserId, ct);
        if (user == null)
            throw new ArgumentException("User not found");

        var todo = new Todo
        {
            UserId = user.Id,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            DueDate = dto.DueDate,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _todoRepo.AddAsync(todo, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<TodoDto>(todo);
    }

    public async Task<List<TodoDto>> GetTodosForUserAsync(int userId, CancellationToken ct = default)
    {
        var todos = await _todoRepo.FindAsync(t => t.UserId == userId, ct);
        return _mapper.Map<List<TodoDto>>(todos);
    }

    public async Task<bool> CompleteTodoAsync(int todoId, CancellationToken ct = default)
    {
        var todo = await _todoRepo.GetByIdAsync(todoId, ct);
        if (todo is null)
            return false;

        if (!todo.IsCompleted)
        {
            todo.IsCompleted = true;
            _todoRepo.Update(todo);
            await _uow.SaveChangesAsync(ct);
        }
        return true;
    }

    public Task<TodoDto> CreateTodoAsync(TodoDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}