using api.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace api.tests;

public class InputSanitizationServiceTests
{
    private readonly Mock<ILogger<InputSanitizationService>> _mockLogger;
    private readonly InputSanitizationService _service;

    public InputSanitizationServiceTests()
    {
        _mockLogger = new Mock<ILogger<InputSanitizationService>>();
        _service = new InputSanitizationService(_mockLogger.Object);
    }

    #region SanitizeCoordinates Tests

    [Theory]
    [InlineData(float.NaN, 0f)]
    [InlineData(0f, float.NaN)]
    [InlineData(float.NaN, float.NaN)]
    public void SanitizeCoordinates_ThrowsArgumentException_WhenNaNValuesProvided(float lat, float lon)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.SanitizeCoordinates(lat, lon));
        Assert.Equal("Invalid coordinate values detected.", exception.Message);
        
        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Invalid float values detected")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(float.PositiveInfinity, 0f)]
    [InlineData(0f, float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity, 0f)]
    [InlineData(0f, float.NegativeInfinity)]
    [InlineData(float.PositiveInfinity, float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity, float.NegativeInfinity)]
    public void SanitizeCoordinates_ThrowsArgumentException_WhenInfinityValuesProvided(float lat, float lon)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.SanitizeCoordinates(lat, lon));
        Assert.Equal("Invalid coordinate values detected.", exception.Message);
        
        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Invalid float values detected")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(100f, 0f, 90f, 0f)] // Latitude above max
    [InlineData(-100f, 0f, -90f, 0f)] // Latitude below min
    [InlineData(91f, 0f, 90f, 0f)] // Latitude slightly above max
    [InlineData(-91f, 0f, -90f, 0f)] // Latitude slightly below min
    public void SanitizeCoordinates_ClampsLatitude_WhenOutOfRange(
        float inputLat, float inputLon, float expectedLat, float expectedLon)
    {
        // Act
        var (resultLat, resultLon) = _service.SanitizeCoordinates(inputLat, inputLon);

        // Assert
        Assert.Equal(expectedLat, resultLat);
        Assert.Equal(expectedLon, resultLon);
    }

    [Theory]
    [InlineData(0f, 200f, 0f, -180f)] // Longitude above max - clamped to 180 then wrapped to -180
    [InlineData(0f, -200f, 0f, -180f)] // Longitude below min
    [InlineData(0f, 181f, 0f, -180f)] // Longitude slightly above max - clamped to 180 then wrapped to -180
    [InlineData(0f, -181f, 0f, -180f)] // Longitude slightly below min
    public void SanitizeCoordinates_ClampsLongitude_WhenOutOfRange(
        float inputLat, float inputLon, float expectedLat, float expectedLon)
    {
        // Act
        var (resultLat, resultLon) = _service.SanitizeCoordinates(inputLat, inputLon);

        // Assert
        Assert.Equal(expectedLat, resultLat);
        Assert.Equal(expectedLon, resultLon);
    }

    [Theory]
    [InlineData(0f, 180f, 0f, -180f)] // Longitude at 180° should wrap to -180°
    [InlineData(0f, 180.00001f, 0f, -180f)] // Longitude slightly above 180° clamped to 180, then wrapped to -180
    [InlineData(0f, 179.99999f, 0f, 179.999990f)] // Longitude slightly below 180° should remain
    public void SanitizeCoordinates_WrapsLongitudeAt180Degrees(
        float inputLat, float inputLon, float expectedLat, float expectedLon)
    {
        // Act
        var (resultLat, resultLon) = _service.SanitizeCoordinates(inputLat, inputLon);

        // Assert
        Assert.Equal(expectedLat, resultLat);
        Assert.Equal(expectedLon, resultLon, precision: 5);
    }

    [Theory]
    [InlineData(51.5074123456789f, -0.1278987654321f, 51.507412f, -0.127899f)] // London coordinates
    [InlineData(40.7127837483871f, -74.0059413509172f, 40.712784f, -74.005941f)] // New York coordinates
    [InlineData(35.6761919283746f, 139.6503106384720f, 35.676192f, 139.650311f)] // Tokyo coordinates
    public void SanitizeCoordinates_NormalizesPrecisionToSixDecimalPlaces(
        float inputLat, float inputLon, float expectedLat, float expectedLon)
    {
        // Act
        var (resultLat, resultLon) = _service.SanitizeCoordinates(inputLat, inputLon);

        // Assert
        Assert.Equal(expectedLat, resultLat, precision: 6);
        Assert.Equal(expectedLon, resultLon, precision: 6);
    }

    [Theory]
    [InlineData(51.5074f, -0.1278f)] // Valid coordinates - no change
    [InlineData(0f, 0f)] // Equator and Prime Meridian
    [InlineData(-90f, -180f)] // South Pole, Date Line
    [InlineData(89f, 179f)] // Close to pole and date line but no wrapping
    public void SanitizeCoordinates_DoesNotLog_WhenChangesAreLessThanOneDegree(
        float inputLat, float inputLon)
    {
        // Act
        _service.SanitizeCoordinates(inputLat, inputLon);

        // Assert - should not log information message about sanitization
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Coordinates sanitized")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Theory]
    [InlineData(100f, 0f)] // Latitude change > 1 degree
    [InlineData(0f, 200f)] // Longitude change > 1 degree
    [InlineData(92f, 185f)] // Both coordinates change > 1 degree
    [InlineData(-92f, -185f)] // Both negative coordinates change > 1 degree
    public void SanitizeCoordinates_LogsInformation_WhenChangesExceedOneDegree(
        float inputLat, float inputLon)
    {
        // Act
        _service.SanitizeCoordinates(inputLat, inputLon);

        // Assert - should log information message about sanitization
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Coordinates sanitized")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(51.123456789f, -0.987654321f)]
    [InlineData(-33.8688197f, 151.2092955f)]
    public void SanitizeCoordinates_ReturnsTuple_WithSanitizedValues(
        float inputLat, float inputLon)
    {
        // Act
        var result = _service.SanitizeCoordinates(inputLat, inputLon);

        // Assert
        Assert.IsType<ValueTuple<float, float>>(result);
        Assert.InRange(result.Latitude, -90f, 90f);
        Assert.InRange(result.Longitude, -180f, 180f);
    }

    #endregion

    #region SanitizeString Tests

    [Fact]
    public void SanitizeString_ReturnsEmptyString_WhenInputIsNull()
    {
        // Act
        var result = _service.SanitizeString(null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void SanitizeString_ReturnsEmptyString_WhenInputIsEmpty()
    {
        // Act
        var result = _service.SanitizeString(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("  hello  ", "hello")]
    [InlineData("\thello\t", "hello")]
    [InlineData("\nhello\n", "hello")]
    public void SanitizeString_TrimsWhitespace(string input, string expected)
    {
        // Act
        var result = _service.SanitizeString(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>", "scriptalertxss/script")]
    [InlineData("test<>test", "testtest")]
    [InlineData("test\"'test", "testtest")]
    [InlineData("test%;()&+test", "testtest")]
    [InlineData("<>&\"'%;()+ all removed", "all removed")]
    public void SanitizeString_RemovesDangerousCharacters(string input, string expected)
    {
        // Act
        var result = _service.SanitizeString(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("hello\x00world", "helloworld")] // Null character
    [InlineData("hello\x01world", "helloworld")] // Start of heading
    [InlineData("hello\x1Fworld", "helloworld")] // Unit separator
    [InlineData("hello\x7Fworld", "helloworld")] // Delete character
    public void SanitizeString_RemovesControlCharacters(string input, string expected)
    {
        // Act
        var result = _service.SanitizeString(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SanitizeString_TruncatesToMaxLength_WithDefaultValue()
    {
        // Arrange
        var longString = new string('a', 150);

        // Act
        var result = _service.SanitizeString(longString);

        // Assert
        Assert.Equal(100, result.Length);
        Assert.Equal(new string('a', 100), result);
        
        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("String input truncated to 100 characters")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void SanitizeString_TruncatesToCustomMaxLength()
    {
        // Arrange
        var longString = new string('a', 100);

        // Act
        var result = _service.SanitizeString(longString, maxLength: 50);

        // Assert
        Assert.Equal(50, result.Length);
        Assert.Equal(new string('a', 50), result);
        
        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("String input truncated to 50 characters")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ValidString123", "ValidString123")]
    [InlineData("test-string_with.symbols", "test-string_with.symbols")]
    [InlineData("ABC xyz 123", "ABC xyz 123")]
    public void SanitizeString_PreservesValidCharacters(string input, string expected)
    {
        // Act
        var result = _service.SanitizeString(input);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region SanitizeInteger Tests

    [Theory]
    [InlineData(50, 0, 100, 50)] // Value within range
    [InlineData(0, 0, 100, 0)] // Value at min
    [InlineData(100, 0, 100, 100)] // Value at max
    public void SanitizeInteger_ReturnsValue_WhenWithinRange(
        int value, int min, int max, int expected)
    {
        // Act
        var result = _service.SanitizeInteger(value, min, max);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(150, 0, 100, 100)] // Value above max
    [InlineData(-50, 0, 100, 0)] // Value below min
    [InlineData(1000, 0, 100, 100)] // Value far above max
    [InlineData(-1000, 0, 100, 0)] // Value far below min
    public void SanitizeInteger_ClampsValue_WhenOutOfRange(
        int value, int min, int max, int expected)
    {
        // Act
        var result = _service.SanitizeInteger(value, min, max);

        // Assert
        Assert.Equal(expected, result);
        
        // Verify warning was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Integer value clamped")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void SanitizeInteger_UsesDefaultMinMax_WhenNotProvided()
    {
        // Arrange
        int testValue = 42;

        // Act
        var result = _service.SanitizeInteger(testValue);

        // Assert
        Assert.Equal(testValue, result);
    }

    [Theory]
    [InlineData(int.MaxValue, 0, int.MaxValue - 1, int.MaxValue - 1)]
    [InlineData(int.MinValue, int.MinValue + 1, 0, int.MinValue + 1)]
    public void SanitizeInteger_HandlesExtremeValues(
        int value, int min, int max, int expected)
    {
        // Act
        var result = _service.SanitizeInteger(value, min, max);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SanitizeInteger_DoesNotLog_WhenValueIsWithinRange()
    {
        // Arrange
        int value = 50;
        int min = 0;
        int max = 100;

        // Act
        _service.SanitizeInteger(value, min, max);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Integer value clamped")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    #endregion
}
