using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

public class PDFConditionalRename
{
    public static void Main(string[] args)
    {
        // Check if the program is provided with at least one argument (the input PDF file path)
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: PDFConditionalRename <input_pdf>");
            return;
        }

        string inputPdf = args[0];
        string newFilename = null;

        try
        {
            // Attempt to generate a new filename based on the content of the PDF
            newFilename = GetNewFilename(inputPdf);

            if (!string.IsNullOrEmpty(newFilename))
            {
                string directory = Path.GetDirectoryName(inputPdf);
                string outputPdf = Path.Combine(directory, newFilename + ".pdf");
                int count = 1;

                // Handle name collisions by appending a number to the filename if it already exists
                while (File.Exists(outputPdf))
                {
                    outputPdf = Path.Combine(directory, $"{newFilename} ({count}).pdf");
                    count++;
                }

                // Rename the input PDF to the new filename
                File.Move(inputPdf, outputPdf);
                Console.WriteLine($"Renamed PDF to: {outputPdf}");
            }
            else
            {
                // Inform the user if no matching account number was found
                Console.WriteLine("No matching account number found. PDF not renamed.");
            }
        }
        catch (Exception ex)
        {
            // Catch and display any errors that occur during the renaming process
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static string GetNewFilename(string inputPdf)
    {
        // Define the labels to search for in the PDF content
        string[] searchlabels = { "ACCOUNT NO:", "VISIT #:" }; // Replace with your specific search labels
        string accountNumber = null;

        // Iterate through the search labels to find a matching account number
        foreach (string searchlabel in searchlabels)
        {
            accountNumber = ExtractAccountNumber(inputPdf, searchlabel);
            if (!string.IsNullOrEmpty(accountNumber))
            {
                break; // Stop searching once a match is found
            }
        }

        return accountNumber; // Return the extracted account number or null if not found
    }

    private static string ExtractAccountNumber(string inputPdf, string searchString)
    {
        string accountNumber = null;

        try
        {
            // Determine the path to the Win2PDF command line executable
            var win2pdfcmdline = Environment.SystemDirectory;

            if (System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) == "ARM64")
            {
                win2pdfcmdline += @"\spool\drivers\arm64\3\win2pdfd.exe";
            }
            else if (Environment.Is64BitOperatingSystem)
            {
                win2pdfcmdline += @"\spool\drivers\x64\3\win2pdfd.exe";
            }
            else
            {
                win2pdfcmdline += @"\spool\drivers\w32x86\3\win2pdfd.exe";
            }

            // Check if the Win2PDF executable exists
            if (File.Exists(win2pdfcmdline))
            {
                // Syntax of getcontentsearch:
                // win2pdfd.exe getcontentsearch "sourcepdf" "password" "searchtext" ["fieldlength"]
                // - "sourcepdf": Path to the PDF file
                // - "password": Password for the PDF (use "" if not password-protected)
                // - "searchtext": Text to search for in the PDF
                // - "fieldlength" (optional): Maximum number of characters to extract

                // Set up the process to execute the Win2PDF command
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = win2pdfcmdline,
                    Arguments = $"getcontentsearch \"{inputPdf}\" \"\" \"{searchString}\"", // Search for the specified text in the PDF
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Execute the command and capture the output
                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string output = reader.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            // Trim and store the extracted account number
                            accountNumber = output.Trim();
                        }
                        else
                        {
                            // Log an error if the command fails
                            Console.WriteLine($"Win2PDF command failed with exit code: {process.ExitCode}");
                        }
                    }
                }
            }
            else
            {
                // Inform the user if Win2PDF is not installed
                Console.WriteLine("Win2PDF is not installed.");
            }
        }
        catch (Exception ex)
        {
            // Catch and display any errors that occur during the extraction process
            Console.WriteLine($"Error executing Win2PDF command: {ex.Message}");
        }

        return accountNumber; // Return the extracted account number or null if not found
    }
}
