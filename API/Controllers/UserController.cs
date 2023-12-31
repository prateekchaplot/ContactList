using ContactListAPI.Data;
using ContactListAPI.Dtos;
using ContactListAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContactListAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContext _dataContext;

    public UserController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpPost("[action]")]
    public IActionResult Create(UserDto dto)
    {
        // Check if a user exists
        if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
            return BadRequest();

        // Add user to db
        var user = new User { Username = dto.Username, Password = dto.Password };
        _dataContext.Add(user);
        _dataContext.SaveChanges();

        return Created("", user);
    }

    [HttpPost("[action]")]
    public IActionResult Login(UserDto dto)
    {
        var user = _dataContext.Users.FirstOrDefault(x => x.Username == dto.Username);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        if (user.Password != dto.Password)
        {
            return BadRequest("Password don't match.");
        }

        return Ok(user);
    }

    [HttpGet("[action]")]
    public IActionResult Logout(Guid userId)
    {
        // Logout user
        return Ok();
    }
}