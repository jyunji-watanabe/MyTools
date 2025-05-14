using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Register the code pages encoding provider
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a process type and parameters as command line arguments.");
            return;
        }

        string processType = args[0];
        string parameter1 = args.Length > 1 ? args[1] : string.Empty;

        try
        {
            switch (processType.ToLower())
            {
                case "parse":
                    if (parameter1 == null)
                    {
                        Console.WriteLine("Please provide a file path for the 'parse' process.");
                        return;
                    }

                    // Read the file with Shift-JIS encoding
                    Encoding shiftJis = Encoding.GetEncoding("shift_jis");
                    string text = File.ReadAllText(parameter1, shiftJis);

                    // Print the text to the console
                    Console.WriteLine("Parsed Content:");
                    Console.WriteLine(text);
                    break;

                case "convert-to-utf8":
                    if (parameter1 == null)
                    {
                        Console.WriteLine("Please provide a file path for the 'convert-to-utf8' process.");
                        return;
                    }

                    // Convert the file content to UTF-8 and save it
                    Encoding shiftJisForConversion = Encoding.GetEncoding("shift_jis");
                    string content = File.ReadAllText(parameter1, shiftJisForConversion);
                    string utf8FilePath = Path.ChangeExtension(parameter1, ".utf8.csv");
                    File.WriteAllText(utf8FilePath, content, Encoding.UTF8);
                    Console.WriteLine($"File converted to UTF-8 and saved as: {utf8FilePath}");
                    break;

                case "mask":
                    // receive a file path and read it as a text file in Shift-JIS encoding
                    if (parameter1 == null)
                    {
                        Console.WriteLine("Please provide a file path for the 'mask' process.");
                        return;
                    }
                    Encoding shiftJisForMasking = Encoding.GetEncoding("shift_jis");
                    string maskContent = File.ReadAllText(parameter1, shiftJisForMasking);
                    // Mask the all the occurrences of numbers in the text with the same length of random number characters
                    // However, numbers written in full-width characters should not be masked
                    Random random = new Random();
                    string maskedContent = System.Text.RegularExpressions.Regex.Replace(maskContent, "[0-9]+", match =>
                    {
                        int length = match.Length;
                        char[] randomChars = new char[length];
                        for (int i = 0; i < length; i++)
                        {
                            randomChars[i] = (char)random.Next('0', '9' + 1);
                        }
                        return new string(randomChars);
                    });
                    // Save the masked content to a new file with the extension of .csv in the same folder in Shift-JIS encoding
                    string maskedFilePath = Path.ChangeExtension(parameter1, ".masked.csv");
                    File.WriteAllText(maskedFilePath, maskedContent, shiftJisForMasking);
                    break;

                case "help":
                    Console.WriteLine("Usage:");
                    Console.WriteLine("  parse <file_path> - Reads a Shift-JIS encoded file and prints its content.");
                    Console.WriteLine("  convert-to-utf8 <file_path> - Converts a Shift-JIS encoded file to UTF-8.");
                    break;

                default:
                    Console.WriteLine($"Unknown process type: {processType}");
                    Console.WriteLine("Supported process types: parse, convert-to-utf8");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}