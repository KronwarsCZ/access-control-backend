using System.Text.RegularExpressions;

namespace Framework.Utilities;

public static class RegexUtilities
{
    private static readonly string EmailAddressPattern = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
    public static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, EmailAddressPattern);
    }
}
