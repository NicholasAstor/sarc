using Sarc.Service.Interface;
using Sarc.Model.Entity;
using Sarc.DTOs;
using Sarc.Repository.Interface;

namespace Sarc.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            FullName = dto.FullName,
            Roles = new List<string> { "user" }
        };

        return await _repository.CreateAsync(user);
    }

    public async Task<User?> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        var existingUser = await _repository.GetByIdAsync(id);
        if (existingUser == null)
            return null;

        if (!string.IsNullOrEmpty(dto.Email))
            existingUser.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.FullName))
            existingUser.FullName = dto.FullName;

        return await _repository.UpdateAsync(id, existingUser);
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }
}