namespace Arthur.Helpers;

public static class Base64
{
    public static string FromBase64ToString(string s)
    {
        var encodedBytes = Convert.FromBase64String(s);
        var decodedString = System.Text.Encoding.UTF8.GetString(encodedBytes);

        return decodedString;
    }

    public static string FromFileToBase64(string path)
    {
        var bytes = File.ReadAllBytes(path);
        return Convert.ToBase64String(bytes);
    }
}