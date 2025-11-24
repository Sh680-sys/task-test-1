using BLL.DTOs;

namespace BLL.Interfaces;

public interface ITodoService
{
    Task<TodoDto> CreateTodoAsync(CreateTodoDto dto, CancellationToken ct = default);
    Task<List<TodoDto>> GetTodosForUserAsync(int userId, CancellationToken ct = default);
    Task<bool> CompleteTodoAsync(int todoId, CancellationToken ct = default);
}