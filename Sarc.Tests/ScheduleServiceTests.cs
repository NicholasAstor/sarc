using System;
using System.Linq;
using System.Threading.Tasks;
using Sarc.DTOs;
using Sarc.Model.Entity;
using Sarc.Repository;
using Sarc.Repository.Interface;
using Sarc.Service;
using Sarc.Service.Interface;
using Xunit;

namespace Sarc.Tests;

public class ScheduleServiceTests
{
    private readonly IScheduleService _scheduleService;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IRoomRepository _roomRepository;

    public ScheduleServiceTests()
    {
        _scheduleRepository = new ScheduleRepository();
        _roomRepository = new RoomRepository();
        _scheduleService = new ScheduleService(_scheduleRepository, _roomRepository);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenStartAfterEnd()
    {
        // Arrange
        var userId = "user-001";
        var dto = new CreateScheduleDto
        {
            RoomId = (await _roomRepository.GetAllAsync(1, 1, null)).First().Id,
            StartAt = DateTime.UtcNow.AddHours(2),
            EndAt = DateTime.UtcNow.AddHours(1)
        };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _scheduleService.CreateAsync(userId, dto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenRoomDoesNotExist()
    {
        // Arrange
        var userId = "user-001";
        var dto = new CreateScheduleDto
        {
            RoomId = "room-inexistente",
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _scheduleService.CreateAsync(userId, dto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenConflictExists()
    {
        // Arrange
        var userId = "user-001";
        var room = (await _roomRepository.GetAllAsync(1, 1, null)).First();

        var baseStart = DateTime.UtcNow.AddHours(1);
        var baseEnd = baseStart.AddHours(1);

        // Reserva base
        await _scheduleService.CreateAsync(userId, new CreateScheduleDto
        {
            RoomId = room.Id,
            StartAt = baseStart,
            EndAt = baseEnd
        });

        // Tenta criar outra reserva que conflita com a anterior
        var dtoConflict = new CreateScheduleDto
        {
            RoomId = room.Id,
            StartAt = baseStart.AddMinutes(30),
            EndAt = baseEnd.AddMinutes(30)
        };

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _scheduleService.CreateAsync(userId, dtoConflict));
    }

    [Fact]
    public async Task CreateAsync_Creates_WhenNoConflict()
    {
        // Arrange
        var userId = "user-001";
        var room = (await _roomRepository.GetAllAsync(1, 1, null)).First();

        var dto = new CreateScheduleDto
        {
            RoomId = room.Id,
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        // Act
        var schedule = await _scheduleService.CreateAsync(userId, dto);

        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(userId, schedule.UserId);
        Assert.Equal(room.Id, schedule.RoomId);
    }

    [Fact]
    public async Task CancelAsync_AllowsOwner()
    {
        // Arrange
        var userId = "user-001";
        var room = (await _roomRepository.GetAllAsync(1, 1, null)).First();

        var schedule = await _scheduleService.CreateAsync(userId, new CreateScheduleDto
        {
            RoomId = room.Id,
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2)
        });

        // Act
        var result = await _scheduleService.CancelAsync(schedule.Id, userId, isAdmin: false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CancelAsync_Throws_WhenNotOwnerAndNotAdmin()
    {
        // Arrange
        var ownerId = "owner-001";
        var otherUserId = "other-999";
        var room = (await _roomRepository.GetAllAsync(1, 1, null)).First();

        var schedule = await _scheduleService.CreateAsync(ownerId, new CreateScheduleDto
        {
            RoomId = room.Id,
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2)
        });

        // Act + Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _scheduleService.CancelAsync(schedule.Id, otherUserId, isAdmin: false));
    }

    [Fact]
    public async Task CancelAsync_AllowsAdmin()
    {
        // Arrange
        var ownerId = "owner-001";
        var adminUserId = "admin-001";
        var room = (await _roomRepository.GetAllAsync(1, 1, null)).First();

        var schedule = await _scheduleService.CreateAsync(ownerId, new CreateScheduleDto
        {
            RoomId = room.Id,
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2)
        });

        // Act
        var result = await _scheduleService.CancelAsync(schedule.Id, adminUserId, isAdmin: true);

        // Assert
        Assert.True(result);
    }
}
