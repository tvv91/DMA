using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class StatisticControllerIndexTests
{
    private readonly StatisticControllerTestFactory _factory = new();
    private static readonly DateTime FixedLastUpdate = new(2024, 9, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Index_ValidStatistic_ReturnsIndexViewWithDeserializedModel()
    {
        var data = JsonSerializer.Serialize(new StatisticCounters
        {
            TotalAlbums = 12,
            TotalReleases = 20,
            TotalArtists = 8,
            TotalEquipment = 5,
            TotalSize = 102.5,
            StorageCount = 3,
        });
        _factory.StatisticServiceMock
            .Setup(s => s.ProcessAsync())
            .ReturnsAsync(new Statistic { Id = 1, Data = data, LastUpdate = FixedLastUpdate });

        var controller = _factory.CreateController();
        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        var model = Assert.IsType<StatisticViewModel>(viewResult.Model);
        Assert.Equal(12, model.TotalAlbums);
        Assert.Equal(20, model.TotalReleases);
        Assert.Equal(8, model.TotalArtists);
        Assert.Equal(5, model.TotalEquipment);
        Assert.Equal(102.5, model.TotalSize);
        Assert.Equal(3, model.StorageCount);
    }

    [Fact]
    public async Task Index_SetsLastUpdateFromStatisticEntity()
    {
        var lastUpdate = new DateTime(2023, 5, 1, 8, 30, 0, DateTimeKind.Utc);
        var data = JsonSerializer.Serialize(new StatisticCounters { TotalAlbums = 1 });
        _factory.StatisticServiceMock
            .Setup(s => s.ProcessAsync())
            .ReturnsAsync(new Statistic { Id = 1, Data = data, LastUpdate = lastUpdate });

        var controller = _factory.CreateController();
        var result = await controller.Index();

        var model = Assert.IsType<StatisticViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Equal(lastUpdate, model.LastUpdate);
    }

    [Fact]
    public async Task Index_DeserializesCounterLists()
    {
        var data = JsonSerializer.Serialize(new StatisticCounters
        {
            TotalAlbums = 2,
            Genre =
            [
                new CounterItem { Description = "Rock", Count = 5 },
            ],
            Artist =
            [
                new CounterItem { Description = "Pink Floyd", Count = 2 },
            ],
            Sampling =
            [
                new CounterItem { Description = "96 kHz", Count = 3 },
            ],
        });
        _factory.StatisticServiceMock
            .Setup(s => s.ProcessAsync())
            .ReturnsAsync(new Statistic { Id = 1, Data = data, LastUpdate = FixedLastUpdate });

        var controller = _factory.CreateController();
        var result = await controller.Index();

        var model = Assert.IsType<StatisticViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Single(model.Genre!);
        Assert.Equal("Rock", model.Genre![0].Description);
        Assert.Equal(5, model.Genre[0].Count);
        Assert.Single(model.Artist!);
        Assert.Equal("Pink Floyd", model.Artist![0].Description);
        Assert.Single(model.Sampling!);
        Assert.Equal("96 kHz", model.Sampling![0].Description);
    }

    [Fact]
    public async Task Index_CallsProcessAsyncOnce()
    {
        var data = JsonSerializer.Serialize(new StatisticCounters { TotalAlbums = 0 });
        _factory.StatisticServiceMock
            .Setup(s => s.ProcessAsync())
            .ReturnsAsync(new Statistic { Id = 1, Data = data, LastUpdate = FixedLastUpdate });

        var controller = _factory.CreateController();
        await controller.Index();

        _factory.StatisticServiceMock.Verify(s => s.ProcessAsync(), Times.Once);
    }

    [Fact]
    public async Task Index_NullJsonPayload_ReturnsProblem()
    {
        _factory.StatisticServiceMock
            .Setup(s => s.ProcessAsync())
            .ReturnsAsync(new Statistic { Id = 1, Data = "null", LastUpdate = FixedLastUpdate });

        var controller = _factory.CreateController();
        var result = await controller.Index();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("Failed to deserialize StatisticViewModel.", problem.Detail);
    }

    [Fact]
    public async Task Index_MalformedJson_ThrowsJsonException()
    {
        _factory.StatisticServiceMock
            .Setup(s => s.ProcessAsync())
            .ReturnsAsync(new Statistic { Id = 1, Data = "{invalid", LastUpdate = FixedLastUpdate });

        var controller = _factory.CreateController();

        await Assert.ThrowsAsync<JsonException>(() => controller.Index());
    }
}
