namespace WOVSIM.Bot.Extensions;

public static partial class StringExtensions
{
    // Puts a message in a text file if it's longer than maxLength, otherwise returns the original message.
    public static (string message, Stream? stream) HandleLongMessage(this string message, int maxLength = 2000)
    {
        if (message.Length <= maxLength)
        {
            return (message, null);
        }

        var fileContent = System.Text.Encoding.UTF8.GetBytes(message);
        Stream stream = new MemoryStream(fileContent);
        var shortMessage = message.Substring(0, 100) + 
                           "... \n\n[Message too long, see attached file for full content.]";

        return (shortMessage, stream);
    }
}