using System.ComponentModel.DataAnnotations;

namespace ContactListAPI.Dtos;

public class ContactDto
{
    public Guid ContactId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Phone { get; set; }
}