using Sarc.DTOs;
using Sarc.Model.Entity;
using Sarc.Repository.Interface;
using Sarc.Service.Interface;

namespace Sarc.Service;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public Task<IEnumerable<Room>> GetRoomsAsync(int page, int pageSize, int? minCapacity)
        => _roomRepository.GetAllAsync(page, pageSize, minCapacity);

    public Task<Room?> GetByIdAsync(string id)
        => _roomRepository.GetByIdAsync(id);

    public async Task<Room> CreateAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            IsActive = true
        };

        return await _roomRepository.CreateAsync(room);
    }

    public async Task<Room?> UpdateAsync(string id, UpdateRoomDto dto)
    {
        var existing = await _roomRepository.GetByIdAsync(id);
        if (existing == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            existing.Name = dto.Name;

        if (dto.Capacity.HasValue)
            existing.Capacity = dto.Capacity.Value;

        if (dto.IsActive.HasValue)
            existing.IsActive = dto.IsActive.Value;

        return await _roomRepository.UpdateAsync(existing);
    }

    public Task<bool> DeleteAsync(string id)
        => _roomRepository.DeleteAsync(id);
}
