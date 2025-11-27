using Sarc.Model.Entity;
using Sarc.Repository.Interface;

namespace Sarc.Repository;

public class ScheduleRepository : IScheduleRepository
{
    private readonly List<Schedule> _schedules = new();

    public Task<Schedule> CreateAsync(Schedule schedule)
    {
        _schedules.Add(schedule);
        return Task.FromResult(schedule);
    }

    public Task<Schedule?> GetByIdAsync(string id)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(schedule);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var existing = _schedules.FirstOrDefault(s => s.Id == id);
        if (existing == null) return Task.FromResult(false);

        _schedules.Remove(existing);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Schedule>> GetByUserAsync(string userId)
    {
        var result = _schedules
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.StartAt);

        return Task.FromResult(result.AsEnumerable());
    }

    public Task<IEnumerable<Schedule>> GetByRoomAndRangeAsync(
        string roomId,
        DateTime start,
        DateTime end)
    {
        var result = _schedules.Where(s =>
            s.RoomId == roomId &&
            start < s.EndAt &&
            end > s.StartAt);

        return Task.FromResult(result.AsEnumerable());
    }
}
