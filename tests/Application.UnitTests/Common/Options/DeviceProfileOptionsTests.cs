using Application.Common.Options;

namespace Application.UnitTests.Common.Options;

[TestFixture]
public sealed class DeviceProfileOptionsTests
{
    [Test]
    public void IsValid_DefaultObject_ReturnsFalse()
    {
        var options = new DeviceProfileOptions();
        var result = options.IsValid();
        result.Should().BeFalse("default object should not be valid.");
    }
}