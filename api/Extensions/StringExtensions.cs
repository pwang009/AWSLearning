namespace api.Extensions;

public static class stringExtensions {

    public static bool IsEmpty(this string data)
        => String.IsNullOrEmpty(data);
        
}