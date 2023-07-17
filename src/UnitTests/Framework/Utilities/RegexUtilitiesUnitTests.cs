using Framework.Option;
using Framework.Utilities;

namespace UnitTests.Framework.Utilities;

public sealed class RegexUtilitiesUnitTests
{
    [TestCase("test@gmail.de")]
    [TestCase("admin@AC.sk")]
    [TestCase("abc@asd.pl")]
    [TestCase("martin@radoby_startup.org")]
    [TestCase("petr@korporat.it")]
    [TestCase("leo@krymska.cz")]
    public void IsEmailFormatValid_Valid_Input(string emailAddress)
    {
        // Act
        var res = RegexUtilities.IsEmailValid(emailAddress);

        // Assert
        Assert.IsTrue(res);
    }

    [TestCase("")]
    [TestCase("@")]
    [TestCase("@.com")]
    [TestCase("abc@.")]
    [TestCase(".")]
    [TestCase("asd@")]
    public void IsEmailFormatValid_Invalid_Input(string emailAddress)
    {
        // Act
        var res = RegexUtilities.IsEmailValid(emailAddress);

        // Assert
        Assert.IsFalse(res);
    }
}
