using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PL.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    // هون عملنا كونستركتور عادي بدل الشكل الجديد عشان نتفادى مشاكل الـ preview
    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpPost]
    public async Task<ActionResult<TodoDto>> Create(CreateTodoDto dto, CancellationToken ct)
    {
        var created = await _todoService.CreateTodoAsync(dto, ct);
        return CreatedAtAction(nameof(GetForUser), new { userId = created.UserId }, created);
    }

    // هاد روت كامل مستقل (استخدمنا السلاش بالبداية) إذا بدك يخضع لقاعدته احذف السلاش الأول
    [HttpGet("/api/users/{userId:int}/todos")]
    public async Task<ActionResult<List<TodoDto>>> GetForUser(int userId, CancellationToken ct)
    {
        var todos = await _todoService.GetTodosForUserAsync(userId, ct);
        return Ok(todos);
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id, CancellationToken ct)
    {
        var ok = await _todoService.CompleteTodoAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}