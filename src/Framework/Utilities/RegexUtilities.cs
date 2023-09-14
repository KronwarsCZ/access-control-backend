using System.Text.RegularExpressions;

namespace Framework.Utilities;

public static class RegexUtilities
{
    private static readonly string EmailAddressPattern = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";

    private static readonly string PasswordPattern =
        "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
    public static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, EmailAddressPattern);
    }

    public static bool IsPasswordValid(string password)
    {
        // aspon 8 znaku
        // jedno velke pismeno
        // jedno male pismeno
        // jeden spesl znak
        // jedna cislice
        return Regex.IsMatch(password, PasswordPattern);
    }
}
