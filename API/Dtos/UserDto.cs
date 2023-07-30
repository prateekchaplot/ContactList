using System.ComponentModel.DataAnnotations;

namespace ContactListAPI.Dtos;

public class UserDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}