using Xunit;
using Moq;
using UserManagementApi.Models;
using UserManagementApi.Repositories;
using UserManagementApi.Services;

namespace UserManagementApi.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _service = new UserService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userId = "user-001";
        var expectedUser = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com"
        };
        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(expectedUser.Username, result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "non-existent";
        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_CreatesUser_WithCorrectData()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Email = "newuser@example.com",
            FullName = "New User"
        };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _service.CreateUserAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Username, result.Username);
        Assert.Equal(dto.Email, result.Email);
        Assert.Contains("user", result.Roles);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesUser_WhenUserExists()
    {
        // Arrange
        var userId = "user-001";
        var existingUser = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "old@example.com",
            FullName = "Old Name"
        };
        var updateDto = new UpdateUserDto
        {
            Email = "new@example.com",
            FullName = "New Name"
        };
        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.UpdateAsync(userId, It.IsAny<User>()))
            .ReturnsAsync((string id, User u) => u);

        // Act
        var result = await _service.UpdateUserAsync(userId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Email, result.Email);
        Assert.Equal(updateDto.FullName, result.FullName);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsTrue_WhenUserIsDeleted()
    {
        // Arrange
        var userId = "user-001";
        _mockRepository.Setup(r => r.DeleteAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "non-existent";
        _mockRepository.Setup(r => r.DeleteAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        Assert.False(result);
    }
}