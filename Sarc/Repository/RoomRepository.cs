using Sarc.Model.Entity;
using Sarc.Repository.Interface;

namespace Sarc.Repository;

public class RoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms = new();

    public RoomRepository()
    {
        _rooms.Add(new Room { Name = "Sala 101", Capacity = 10 });
        _rooms.Add(new Room { Name = "Sala 102", Capacity = 20 });
        _rooms.Add(new Room { Name = "Sala Reunião", Capacity = 6 });
    }

    public Task<IEnumerable<Room>> GetAllAsync(int page, int pageSize, int? minCapacity)
    {
        var query = _rooms.Where(r => r.IsActive);

        if (minCapacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= minCapacity.Value);
        }

        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return Task.FromResult(query.AsEnumerable());
    }

    public Task<Room?> GetByIdAsync(string id)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(room);
    }

    public Task<Room> CreateAsync(Room room)
    {
        _rooms.Add(room);
        return Task.FromResult(room);
    }

    public Task<Room?> UpdateAsync(Room room)
    {
        var existing = _rooms.FirstOrDefault(r => r.Id == room.Id);
        if (existing == null) return Task.FromResult<Room?>(null);

        existing.Name = room.Name;
        existing.Capacity = room.Capacity;
        existing.IsActive = room.IsActive;

        return Task.FromResult<Room?>(existing);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var existing = _rooms.FirstOrDefault(r => r.Id == id);
        if (existing == null) return Task.FromResult(false);

        _rooms.Remove(existing);
        return Task.FromResult(true);
    }
}
