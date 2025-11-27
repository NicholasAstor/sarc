using Sarc.DTOs;
using Sarc.Model.Entity;

namespace Sarc.Service.Interface;

public interface IRoomService
{
    Task<IEnumerable<Room>> GetRoomsAsync(int page, int pageSize, int? minCapacity);
    Task<Room?> GetByIdAsync(string id);
    Task<Room> CreateAsync(CreateRoomDto dto);
    Task<Room?> UpdateAsync(string id, UpdateRoomDto dto);
    Task<bool> DeleteAsync(string id);
}
