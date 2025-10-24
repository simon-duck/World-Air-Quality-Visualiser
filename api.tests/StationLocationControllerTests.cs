using api.Controllers;
using api.Models.Database;
using api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace api.tests;

public class StationLocationControllerTests
{
    private readonly Mock<IStationLocationRepository> _mockRepository;
    private readonly StationLocationController _controller;

    public StationLocationControllerTests()
    {
        _mockRepository = new Mock<IStationLocationRepository>();
        _controller = new StationLocationController(_mockRepository.Object);
    }

    [Fact]
    public async Task GetStations_ReturnsOkResult_WithStationList()
    {
        // Arrange
        var expectedStations = new List<StationLocation>
        {
            new StationLocation
            {
                Uid = 1,
                Name = "London Station",
                Lat = 51.5074f,
                Lon = -0.1278f,
                Country = "UK",
            },
            new StationLocation
            {
                Uid = 2,
                Name = "Paris Station",
                Lat = 48.8566f,
                Lon = 2.3522f,
                Country = "France",
            },
        };

        _mockRepository.Setup(repo => repo.GetStationLocations()).ReturnsAsync(expectedStations);

        // Act
        var result = await _controller.GetStations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<StationLocation>>(okResult.Value);
        Assert.Equal(expectedStations.Count, returnValue.Count);
        Assert.Equal(expectedStations[0].Name, returnValue[0].Name);
        Assert.Equal(expectedStations[1].Name, returnValue[1].Name);
    }

    [Fact]
    public async Task GetStations_CallsRepositoryOnce()
    {
        // Arrange
        var expectedStations = new List<StationLocation>();
        _mockRepository.Setup(repo => repo.GetStationLocations()).ReturnsAsync(expectedStations);

        // Act
        await _controller.GetStations();

        // Assert
        _mockRepository.Verify(repo => repo.GetStationLocations(), Times.Once);
    }

    [Fact]
    public async Task GetStations_ReturnsEmptyList_WhenNoStations()
    {
        // Arrange
        var emptyList = new List<StationLocation>();
        _mockRepository.Setup(repo => repo.GetStationLocations()).ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetStations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<StationLocation>>(okResult.Value);
        Assert.Empty(returnValue);
    }
}
