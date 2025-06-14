using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using YourApp.Data;
using ManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TaskController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,Manager,User")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (User.IsInRole("Admin"))
            return await _context.Tasks.ToListAsync();
        else
            return await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
        //return await _context.Tasks.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        return task;
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> PostTask(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTask(int id, TaskItem task)
    {
        if (id != task.Id) return BadRequest();
        _context.Entry(task).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
