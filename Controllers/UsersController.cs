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
    private static readonly object UsersLock = new();
    private static int NextUserId = Users.Max(user => user.Id) + 1;

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        if (page.HasValue != pageSize.HasValue)
        {
            return BadRequest("Supply both page and pageSize to use pagination.");
        }

        if (page.HasValue && (page.Value < 1 || pageSize!.Value < 1 || pageSize.Value > 100))
        {
            return BadRequest("page must be positive and pageSize must be between 1 and 100.");
        }

        User[] users;
        lock (UsersLock)
        {
            if (!page.HasValue)
            {
                users = Users.ToArray();
            }
            else
            {
                var skip = (long)(page.Value - 1) * pageSize!.Value;
                users = skip >= Users.Count
                    ? []
                    : Users.Skip((int)skip).Take(pageSize.Value).ToArray();
            }
        }

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetUserById(int id)
    {
        User? user;
        lock (UsersLock)
        {
            user = Users.FirstOrDefault(user => user.Id == id);
        }

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        NormalizeUser(user);

        lock (UsersLock)
        {
            if (Users.Any(existingUser => EmailsMatch(existingUser.Email, user.Email)))
            {
                return Conflict("A user with this email already exists.");
            }

            user.Id = NextUserId++;
            Users.Add(user);
        }

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest("The route ID must match the user ID.");
        }

        NormalizeUser(user);

        lock (UsersLock)
        {
            var index = Users.FindIndex(existingUser => existingUser.Id == id);
            if (index < 0)
            {
                return NotFound();
            }

            if (Users.Any(existingUser => existingUser.Id != id && EmailsMatch(existingUser.Email, user.Email)))
            {
                return Conflict("A user with this email already exists.");
            }

            Users[index] = user;
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteUser(int id)
    {
        lock (UsersLock)
        {
            var index = Users.FindIndex(user => user.Id == id);
            if (index < 0)
            {
                return NotFound();
            }

            Users.RemoveAt(index);
        }

        return NoContent();
    }

    private static bool EmailsMatch(string firstEmail, string secondEmail)
    {
        return string.Equals(firstEmail, secondEmail, StringComparison.OrdinalIgnoreCase);
    }

    private static void NormalizeUser(User user)
    {
        user.Name = user.Name.Trim();
        user.Email = user.Email.Trim();
        user.Department = user.Department.Trim();
    }
}