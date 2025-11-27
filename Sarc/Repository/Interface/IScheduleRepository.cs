using Sarc.Model.Entity;

namespace Sarc.Repository.Interface;

public interface IScheduleRepository
{
    Task<Schedule> CreateAsync(Schedule schedule);
    Task<Schedule?> GetByIdAsync(string id);
    Task<bool> DeleteAsync(string id);

    Task<IEnumerable<Schedule>> GetByUserAsync(string userId);

    Task<IEnumerable<Schedule>> GetByRoomAndRangeAsync(
        string roomId,
        DateTime start,
        DateTime end);
}
