namespace freekvanzee.Helpers;

public static class BlogHelper
{
    public static string GetTitleCaseTitle(string title)
    {
        // Take a snake_case string and return a Title Case string
        return string.Join(
            " ",
            title.Split('-').Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
        );
    }
}
