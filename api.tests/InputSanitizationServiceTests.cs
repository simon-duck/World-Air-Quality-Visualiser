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

    [Fact]
    public void SanitizeCoordinates_WithValidCoordinates_ReturnsUnchanged()
    {
        // Arrange
        float lat = 51.5074f;
        float lon = -0.1278f;

        // Act
        var (resultLat, resultLon) = _service.SanitizeCoordinates(lat, lon);

        // Assert
        Assert.Equal(lat, resultLat, 5);
        Assert.Equal(lon, resultLon, 5);
    }

    [Fact]
    public void SanitizeCoordinates_WithNaNLatitude_ThrowsArgumentException()
    {
        // Arrange
        float lat = float.NaN;
        float lon = -0.1278f;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.SanitizeCoordinates(lat, lon));
    }

    [Fact]
    public void SanitizeCoordinates_WithNaNLongitude_ThrowsArgumentException()
    {
        // Arrange
        float lat = 51.5074f;
        float lon = float.NaN;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.SanitizeCoordinates(lat, lon));
    }

    [Fact]
    public void SanitizeCoordinates_WithPositiveInfinityLatitude_ThrowsArgumentException()
    {
        // Arrange
        float lat = float.PositiveInfinity;
        float lon = -0.1278f;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.SanitizeCoordinates(lat, lon));
    }

    [Fact]
    public void SanitizeCoordinates_WithNegativeInfinityLongitude_ThrowsArgumentException()
    {
        // Arrange
        float lat = 51.5074f;
        float lon = float.NegativeInfinity;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.SanitizeCoordinates(lat, lon));
    }

    [Theory]
    [InlineData(100f, 90f)]      // Above max latitude
    [InlineData(-100f, -90f)]    // Below min latitude
    [InlineData(91f, 90f)]       // Just above max
    [InlineData(-91f, -90f)]     // Just below min
    public void SanitizeCoordinates_ClampsLatitudeToValidRange(float inputLat, float expectedLat)
    {
        // Arrange
        float lon = 0f;

        // Act
        var (resultLat, _) = _service.SanitizeCoordinates(inputLat, lon);

        // Assert
        Assert.Equal(expectedLat, resultLat);
    }

    [Theory]
    [InlineData(200f, -180f)]    // Above max longitude (wraps or clamps)
    [InlineData(-200f, -180f)]   // Below min longitude
    [InlineData(180f, -180f)]    // Edge case: 180° normalizes to -180°
    public void SanitizeCoordinates_ClampsLongitudeToValidRange(float inputLon, float expectedLon)
    {
        // Arrange
        float lat = 0f;

        // Act
        var (_, resultLon) = _service.SanitizeCoordinates(lat, inputLon);

        // Assert
        Assert.Equal(expectedLon, resultLon, 5);
    }

    [Fact]
    public void SanitizeCoordinates_NormalizesPrecisionTo5DecimalPlaces()
    {
        // Arrange - coordinates with excessive precision
        float lat = 51.50741234567890f;
        float lon = -0.12781234567890f;

        // Act
        var (resultLat, resultLon) = _service.SanitizeCoordinates(lat, lon);

        // Assert - verify precision is limited (comparing string representation)
        // The result should be rounded to 5 decimal places
        Assert.Equal(Math.Round(lat, 5), resultLat, 5);
        Assert.Equal(Math.Round(lon, 5), resultLon, 5);
    }

    [Fact]
    public void SanitizeCoordinates_WithZeroCoordinates_ReturnsZero()
    {
        // Arrange
        float lat = 0f;
        float lon = 0f;

        // Act
        var (resultLat, resultLon) = _service.SanitizeCoordinates(lat, lon);

        // Assert
        Assert.Equal(0f, resultLat);
        Assert.Equal(0f, resultLon);
    }

    [Fact]
    public void SanitizeCoordinates_WithBoundaryValues_ReturnsWithinRange()
    {
        // Arrange - exact boundary values
        float lat = 90f;
        float lon = -180f;

        // Act
        var (resultLat, resultLon) = _service.SanitizeCoordinates(lat, lon);

        // Assert
        Assert.InRange(resultLat, -90f, 90f);
        Assert.InRange(resultLon, -180f, 180f);
    }

    #endregion

    #region SanitizeString Tests

    [Fact]
    public void SanitizeString_WithValidString_ReturnsUnchanged()
    {
        // Arrange
        string input = "test-station_123";

        // Act
        var result = _service.SanitizeString(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void SanitizeString_WithNullInput_ReturnsEmptyString()
    {
        // Act
        var result = _service.SanitizeString(null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void SanitizeString_WithEmptyInput_ReturnsEmptyString()
    {
        // Act
        var result = _service.SanitizeString(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>", "scriptalertxss/script")]
    [InlineData("test<>\"'%;()&+test", "testtest")]
    [InlineData("normal text", "normal text")]
    public void SanitizeString_RemovesDangerousCharacters(string input, string expected)
    {
        // Act
        var result = _service.SanitizeString(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SanitizeString_TruncatesToMaxLength()
    {
        // Arrange
        string input = new string('a', 200);
        int maxLength = 50;

        // Act
        var result = _service.SanitizeString(input, maxLength);

        // Assert
        Assert.Equal(maxLength, result.Length);
    }

    [Fact]
    public void SanitizeString_TrimsWhitespace()
    {
        // Arrange
        string input = "  test  ";

        // Act
        var result = _service.SanitizeString(input);

        // Assert
        Assert.Equal("test", result);
    }

    [Fact]
    public void SanitizeString_RemovesControlCharacters()
    {
        // Arrange - string with control characters
        string input = "test\x00\x1F\x7Fstring";

        // Act
        var result = _service.SanitizeString(input);

        // Assert
        Assert.Equal("teststring", result);
    }

    #endregion

    #region SanitizeInteger Tests

    [Fact]
    public void SanitizeInteger_WithValueInRange_ReturnsUnchanged()
    {
        // Arrange
        int value = 50;
        int min = 0;
        int max = 100;

        // Act
        var result = _service.SanitizeInteger(value, min, max);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void SanitizeInteger_WithValueAboveMax_ClampsToMax()
    {
        // Arrange
        int value = 150;
        int min = 0;
        int max = 100;

        // Act
        var result = _service.SanitizeInteger(value, min, max);

        // Assert
        Assert.Equal(max, result);
    }

    [Fact]
    public void SanitizeInteger_WithValueBelowMin_ClampsToMin()
    {
        // Arrange
        int value = -50;
        int min = 0;
        int max = 100;

        // Act
        var result = _service.SanitizeInteger(value, min, max);

        // Assert
        Assert.Equal(min, result);
    }

    [Fact]
    public void SanitizeInteger_WithDefaultBounds_AcceptsAnyValue()
    {
        // Arrange
        int value = int.MaxValue;

        // Act
        var result = _service.SanitizeInteger(value);

        // Assert
        Assert.Equal(value, result);
    }

    #endregion
}
