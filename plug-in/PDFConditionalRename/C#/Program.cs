using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

public class PDFConditionalRename
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: PDFConditionalRename <input_pdf>");
            return;
        }

        string inputPdf = args[0];
        string newFilename = null;

        try
        {
            newFilename = GetNewFilename(inputPdf);

            if (!string.IsNullOrEmpty(newFilename))
            {
                string outputPdf = Path.Combine(Path.GetDirectoryName(inputPdf), newFilename + ".pdf");
                File.Move(inputPdf, outputPdf);
                Console.WriteLine($"Renamed PDF to: {outputPdf}");
            }
            else
            {
                Console.WriteLine("No matching account number found. PDF not renamed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static string GetNewFilename(string inputPdf)
    {
        string[] searchlabels = { "ACCOUNT NO:", "VISIT #:" }; //replace with your search labels
        string accountNumber = null;

        foreach (string searchlabel in searchlabels)
        {
            accountNumber = ExtractAccountNumber(inputPdf, searchlabel);
            if (!string.IsNullOrEmpty(accountNumber))
            {
                break;
            }
        }

        return accountNumber;
    }

    private static string ExtractAccountNumber(string inputPdf, string searchString)
    {
        string accountNumber = null;

        try
        {
            var win2pdfcmdline = Environment.SystemDirectory;

            // get the path to the Win2PDF command line executable
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

            if (File.Exists(win2pdfcmdline))
            { 
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = win2pdfcmdline,
                    Arguments = $"getcontentsearch \"{inputPdf}\" \"\" \"{searchString}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string output = reader.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            accountNumber = output.Trim();
                        }
                        else
                        {
                            Console.WriteLine($"Win2PDF command failed with exit code: {process.ExitCode}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Win2PDF is not installed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing Win2PDF command: {ex.Message}");
        }

        return accountNumber;
    }
}