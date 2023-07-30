using ContactListAPI.Data;
using ContactListAPI.Dtos;
using ContactListAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContactListAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly DataContext _dataContext;

    public ContactController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet("[action]")]
    public IActionResult Get(Guid userId)
    {
        var contacts = _dataContext.Contacts.Where(x => x.UserId == userId);
        return Ok(contacts);
    }

    [HttpPost("[action]")]
    public IActionResult Create(Guid userId, ContactDto dto)
    {
        //TODO: Check if already exists

        // Add contact
        var contact = new Contact { Name = dto.Name, Phone = dto.Phone, UserId = userId };
        _dataContext.Add(contact);
        _dataContext.SaveChanges();

        return Created("", contact);
    }

    [HttpPut("[action]")]
    public IActionResult Update(Guid userId, ContactDto dto)
    {
        var contact = _dataContext.Contacts.FirstOrDefault(x => x.UserId == userId && x.Id == dto.ContactId);
        if (contact == null)
        {
            return BadRequest("Contact not found.");
        }

        contact.Name = dto.Name;
        contact.Phone = dto.Phone;
        _dataContext.Update(contact);
        _dataContext.SaveChanges();

        return Ok(contact);
    }

    [HttpDelete("[action]")]
    public IActionResult Delete(Guid userId, Guid contactId)
    {
        var contact = _dataContext.Contacts.FirstOrDefault(x => x.UserId == userId && x.Id == contactId);
        if (contact == null)
        {
            return BadRequest("Contact not found.");
        }

        _dataContext.Remove(contact);
        _dataContext.SaveChanges();
        return Ok(contact);
    }

    [HttpGet("[action]")]
    public IActionResult Search(Guid userId, string searchText)
    {
        var contacts = _dataContext.Contacts.Where(x => x.UserId == userId &&
        (x.Name.Contains(searchText) || x.Phone.Contains(searchText)));

        return Ok(contacts);
    }
}