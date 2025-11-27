using Sarc.Model.Entity;

namespace Sarc.Repository.Interface;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(string id, User user);
    Task<bool> DeleteAsync(string id);
}