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
                    if (string.IsNullOrEmpty(parameter1))
                    {
                        Console.WriteLine("Please provide a file path for the 'parse' process.");
                        return;
                    }

                    // Read all lines with Shift-JIS encoding
                    Encoding shiftJis = Encoding.GetEncoding("shift_jis");
                    string[] lines = File.ReadAllLines(parameter1, shiftJis);
                    if (lines.Length == 0)
                    {
                        Console.WriteLine("The file is empty.");
                        return;
                    }

                    // Define the output columns
                    string[] outputHeaders = new string[] { "口座", "ファンド名", "保有口数", "取得単価", "基準価額", "取得金額", "評価額", "評価損益" };
                    var outputRows = new List<string>();
                    outputRows.Add(string.Join(",", outputHeaders));

                    // For each section, find the header and extract data rows
                    Dictionary<string, int>? colMap = null;
                    string currentCategory = "";
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i].Trim();
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var cols = line.Split(',');
                        // Detect section header (カテゴリ)
                        if (line.Contains("/"))
                        {
                            currentCategory = line.Replace(",", "").Trim();
                            continue;
                        }
                        // Detect table header
                        if (cols.Contains("ファンド名") && cols.Contains("評価損益"))
                        {
                            colMap = new Dictionary<string, int>();
                            for (int c = 0; c < cols.Length; c++)
                            {
                                colMap[cols[c].Trim()] = c;
                            }
                            continue;
                        }
                        // Data row
                        if (colMap != null && cols.Length >= colMap.Count && !line.StartsWith("\""))
                        {
                            // skip summary rows or non-data rows
                            continue;
                        }
                        if (colMap != null && cols.Length >= colMap.Count)
                        {
                            // Extract columns by header name
                            string[] row = new string[outputHeaders.Length];
                            row[0] = currentCategory; // Now represents 口座
                            row[1] = colMap.ContainsKey("ファンド名") ? cols[colMap["ファンド名"]] : "";
                            row[2] = colMap.ContainsKey("保有口数") ? cols[colMap["保有口数"]] : "";
                            row[3] = colMap.ContainsKey("取得単価") ? cols[colMap["取得単価"]] : "";
                            row[4] = colMap.ContainsKey("基準価額") ? cols[colMap["基準価額"]] : "";
                            row[5] = colMap.ContainsKey("取得金額") ? cols[colMap["取得金額"]] : "";
                            row[6] = colMap.ContainsKey("評価額") ? cols[colMap["評価額"]] : "";
                            row[7] = colMap.ContainsKey("評価損益") ? cols[colMap["評価損益"]] : "";
                            outputRows.Add(string.Join(",", row));
                        }
                    }

                    // Write organized CSV to a new file with date in the filename
                    string dateStr = DateTime.Now.ToString("yyyyMMdd");
                    string organizedFilePath = Path.Combine(
                        Path.GetDirectoryName(parameter1) ?? string.Empty,
                        Path.GetFileNameWithoutExtension(parameter1) + $".organized.{dateStr}.csv"
                    );
                    File.WriteAllLines(organizedFilePath, outputRows, shiftJis);
                    Console.WriteLine($"Organized CSV saved as: {organizedFilePath}");
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