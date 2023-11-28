namespace YggdrasilApi.Extenstions
{
    public static class StringExtension
    {
        public static string RemoveWhitespaces(this string source)
        {
            return new string(source.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
        public static string[] SplitByWhitespaces(this string source)
        {
            return source.Split(new char[0]).ToArray();
        }
    }
}

