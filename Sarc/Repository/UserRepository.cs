using Sarc.Model.Entity;
using Sarc.Repository.Interface;

namespace Sarc.Repository;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _users = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public InMemoryUserRepository()
    {
        // Usuários de teste
        var admin = new User
        {
            Id = "admin-001",
            Username = "admin",
            Email = "admin@example.com",
            FullName = "Administrator",
            Roles = new List<string> { "admin" }
        };

        var regularUser = new User
        {
            Id = "user-001",
            Username = "user",
            Email = "user@example.com",
            FullName = "Regular User",
            Roles = new List<string> { "user" }
        };

        _users[admin.Id] = admin;
        _users[regularUser.Id] = regularUser;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        await _lock.WaitAsync();
        try
        {
            return _users.TryGetValue(id, out var user) ? user : null;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        await _lock.WaitAsync();
        try
        {
            return _users.Values.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return _users.Values.ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<User> CreateAsync(User user)
    {
        await _lock.WaitAsync();
        try
        {
            user.Id = Guid.NewGuid().ToString();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            _users[user.Id] = user;
            return user;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<User?> UpdateAsync(string id, User user)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_users.ContainsKey(id))
                return null;

            user.Id = id;
            user.UpdatedAt = DateTime.UtcNow;
            _users[id] = user;
            return user;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        await _lock.WaitAsync();
        try
        {
            return _users.Remove(id);
        }
        finally
        {
            _lock.Release();
        }
    }
}