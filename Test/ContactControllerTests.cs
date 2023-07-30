using ContactListAPI.Controllers;
using ContactListAPI.Data;
using ContactListAPI.Dtos;
using ContactListAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Test;

public class ContactControllerTests
{
    private static DataContext InMemoryContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;
        connection.Open();

        // create the schema
        using (var context = new DataContext(options))
        {
            context.Database.EnsureCreated();
        }

        return new DataContext(options);
    }

    // Test case for successful Get action
    [Fact]
    public void Get_ExistingUserId_ReturnsOkResultWithContacts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var contacts = new List<Contact>
        {
            new Contact { Name = "Contact1", Phone = "123456789", UserId = userId },
            new Contact { Name = "Contact2", Phone = "987654321", UserId = userId }
        };
        var mockDataContext = InMemoryContext();
        mockDataContext.Users.Add(new User { Id = userId });
        mockDataContext.Contacts.AddRange(contacts);
        mockDataContext.SaveChanges();
        var controller = new ContactController(mockDataContext);

        // Act
        var result = controller.Get(userId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode); // HTTP 200 OK
        Assert.NotNull(result.Value);
        var resultContacts = result.Value as IEnumerable<Contact>;
        Assert.NotNull(resultContacts);
        Assert.Equal(2, resultContacts.Count());
        Assert.Contains(resultContacts, c => c.Name == "Contact1");
        Assert.Contains(resultContacts, c => c.Name == "Contact2");
    }

    // Test case for Get action with non-existent UserId
    [Fact]
    public void Get_NonExistentUserId_ReturnsEmptyResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockDataContext = InMemoryContext();
        var controller = new ContactController(mockDataContext);

        // Act
        var result = controller.Get(userId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode); // HTTP 200 OK
        Assert.NotNull(result.Value);
        var resultContacts = result.Value as IEnumerable<Contact>;
        Assert.Empty(resultContacts);
    }

    // Test case for successful Create action
    [Fact]
    public void Create_ValidContact_ReturnsCreatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var contactDto = new ContactDto { Name = "NewContact", Phone = "5555555555" };
        var mockDataContext = InMemoryContext();
        mockDataContext.Users.Add(new User { Id = userId });
        var controller = new ContactController(mockDataContext);

        // Act
        var result = controller.Create(userId, contactDto) as CreatedResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode); // HTTP 201 Created
        Assert.NotNull(result.Value);
        var createdContact = result.Value as Contact;
        Assert.NotNull(createdContact);
        Assert.Equal(contactDto.Name, createdContact.Name);
        Assert.Equal(contactDto.Phone, createdContact.Phone);
        Assert.Equal(userId, createdContact.UserId);

        // Check if the contact was added to the context
        var contactInDatabase = mockDataContext.Contacts.FirstOrDefault(c => c.Id == createdContact.Id);
        Assert.NotNull(contactInDatabase);
        Assert.Equal(contactDto.Name, contactInDatabase.Name);
        Assert.Equal(contactDto.Phone, contactInDatabase.Phone);
        Assert.Equal(userId, contactInDatabase.UserId);
    }

    // Test case for Create action with invalid UserId
    [Fact]
    public void Create_InvalidUserId_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var contactDto = new ContactDto { Name = "NewContact", Phone = "5555555555" };
        var mockDataContext = InMemoryContext();
        var controller = new ContactController(mockDataContext);

        // Act
        var result = controller.Create(Guid.Empty, contactDto) as BadRequestResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode); // HTTP 400 Bad Request
    }

}