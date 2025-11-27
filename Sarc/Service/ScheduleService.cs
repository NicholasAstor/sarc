using Sarc.DTOs;
using Sarc.Model.Entity;
using Sarc.Repository.Interface;
using Sarc.Service.Interface;

namespace Sarc.Service;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IRoomRepository _roomRepository;

    public ScheduleService(
        IScheduleRepository scheduleRepository,
        IRoomRepository roomRepository)
    {
        _scheduleRepository = scheduleRepository;
        _roomRepository = roomRepository;
    }

    public async Task<Schedule> CreateAsync(string userId, CreateScheduleDto dto)
    {
        if (dto.StartAt >= dto.EndAt)
        {
            throw new ArgumentException("Data inicial deve ser menor que a final.");
        }

        var room = await _roomRepository.GetByIdAsync(dto.RoomId);
        if (room == null || !room.IsActive)
        {
            throw new ArgumentException("Sala inválida ou inativa.");
        }

        var conflicts = await _scheduleRepository.GetByRoomAndRangeAsync(
            dto.RoomId,
            dto.StartAt,
            dto.EndAt);

        if (conflicts.Any())
        {
            throw new InvalidOperationException("Conflito de horário para esta sala.");
        }

        var schedule = new Schedule
        {
            UserId = userId,
            RoomId = dto.RoomId,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt
        };

        return await _scheduleRepository.CreateAsync(schedule);
    }

    public async Task<bool> CancelAsync(string scheduleId, string requesterUserId, bool isAdmin)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        if (schedule == null) return false;

        if (!isAdmin && schedule.UserId != requesterUserId)
        {
            throw new UnauthorizedAccessException("Usuário não pode cancelar esta reserva.");
        }

        return await _scheduleRepository.DeleteAsync(scheduleId);
    }

    public Task<IEnumerable<Schedule>> GetMySchedulesAsync(string userId)
        => _scheduleRepository.GetByUserAsync(userId);
}
