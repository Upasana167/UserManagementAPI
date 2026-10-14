using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> Users =
    [
        new User { Id = 1, Name = "Avery Johnson", Email = "avery.johnson@example.com", Department = "Engineering" },
        new User { Id = 2, Name = "Morgan Lee", Email = "morgan.lee@example.com", Department = "Marketing" },
        new User { Id = 3, Name = "Taylor Smith", Email = "taylor.smith@example.com", Department = "Finance" }
    ];

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(Users);
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetUserById(int id)
    {
        var user = Users.FirstOrDefault(user => user.Id == id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        user.Id = Users.Count == 0 ? 1 : Users.Max(existingUser => existingUser.Id) + 1;
        Users.Add(user);

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest("The route ID must match the user ID.");
        }

        var index = Users.FindIndex(existingUser => existingUser.Id == id);
        if (index < 0)
        {
            return NotFound();
        }

        Users[index] = user;
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteUser(int id)
    {
        var index = Users.FindIndex(user => user.Id == id);
        if (index < 0)
        {
            return NotFound();
        }

        Users.RemoveAt(index);
        return NoContent();
    }
}