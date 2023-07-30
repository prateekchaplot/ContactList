using ContactListAPI.Controllers;
using ContactListAPI.Data;
using ContactListAPI.Dtos;
using ContactListAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Test;

public class UserControllerTests
{
    public static DataContext InMemoryContext()
    {
        // SEE: https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/sqlite
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

    // Test case for successful user creation
    [Fact]
    public void Create_ValidUser_ReturnsCreatedResult()
    {
        // Arrange
        var userDto = new UserDto { Username = "testuser", Password = "testpassword" };
        var mockDataContext = InMemoryContext();
        var controller = new UserController(mockDataContext);

        // Act
        var result = controller.Create(userDto) as CreatedResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode); // HTTP 201 Created
        Assert.NotNull(result.Value);
        var createdUser = result.Value as User;
        Assert.NotNull(createdUser);
        Assert.Equal(userDto.Username, createdUser.Username);
        Assert.Equal(userDto.Password, createdUser.Password);
    }

    // Test case for creating a user with missing data (e.g., Username or Password is null or empty)
    [Theory]
    [InlineData(null, "testpassword")]
    [InlineData("", "testpassword")]
    [InlineData("testuser", null)]
    [InlineData("testuser", "")]
    public void Create_InvalidUser_ReturnsBadRequest(string username, string password)
    {
        // Arrange
        var userDto = new UserDto { Username = username, Password = password };
        var mockDataContext = InMemoryContext();
        var controller = new UserController(mockDataContext);

        // Act
        var result = controller.Create(userDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestResult>(result);
    }

    // Test case for verifying if the user is added to the database
    [Fact]
    public void Create_ValidUser_AddsUserToDatabase()
    {
        // Arrange
        var userDto = new UserDto { Username = "testuser", Password = "testpassword" };
        var mockDataContext = InMemoryContext();
        var controller = new UserController(mockDataContext);

        // Act
        var result = controller.Create(userDto) as CreatedResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        var createdUser = result.Value as User;
        Assert.NotNull(createdUser);

        // Check if the user was added to the context
        var userInDatabase = mockDataContext.Users.FirstOrDefault(u => u.Username == userDto.Username && u.Password == userDto.Password);
        Assert.NotNull(userInDatabase);

        // You can also check other properties if needed
        Assert.Equal(userDto.Username, userInDatabase.Username);
        Assert.Equal(userDto.Password, userInDatabase.Password);
    }
}