using Sarc.Model.Entity;

namespace Sarc.Repository.Interface;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllAsync(int page, int pageSize, int? minCapacity);
    Task<Room?> GetByIdAsync(string id);
    Task<Room> CreateAsync(Room room);
    Task<Room?> UpdateAsync(Room room);
    Task<bool> DeleteAsync(string id);
}
