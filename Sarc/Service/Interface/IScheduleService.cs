using Sarc.DTOs;
using Sarc.Model.Entity;

namespace Sarc.Service.Interface;

public interface IScheduleService
{
    Task<Schedule> CreateAsync(string userId, CreateScheduleDto dto);
    Task<bool> CancelAsync(string scheduleId, string requesterUserId, bool isAdmin);
    Task<IEnumerable<Schedule>> GetMySchedulesAsync(string userId);
}
