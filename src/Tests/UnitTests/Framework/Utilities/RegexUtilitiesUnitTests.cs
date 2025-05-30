using Framework.Utilities;
using NUnit.Framework;

namespace UnitTests.Framework.Utilities;

public sealed class RegexUtilitiesUnitTests
{
    [TestCase("test@gmail.de")]
    [TestCase("admin@ac.sk")]
    [TestCase("abc@asd.pl")]
    [TestCase("martin@radobystartup.org")]
    [TestCase("petr@korporat.it")]
    [TestCase("leo@krymska.cz")]
    public void IsEmailFormatValid_Valid_Input(string emailAddress)
    {
        // Act
        var res = RegexUtilities.IsEmailValid(emailAddress);

        // Assert
        Assert.That(res, Is.True);
    }

    [TestCase("")]
    [TestCase("abC@asD.pl")]
    [TestCase("ac@asD.pl")]
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
        Assert.That(res, Is.False);
    }
}
