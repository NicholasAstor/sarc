using Sarc.DTOs;
using Sarc.Model.Entity;

namespace Sarc.Service.Interface;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(string id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(CreateUserDto dto);
    Task<User?> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(string id);
}