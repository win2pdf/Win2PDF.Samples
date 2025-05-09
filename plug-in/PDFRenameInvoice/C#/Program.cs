using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Diagnostics.Eventing.Reader;

public class PDFRenameInvoice
{
    public const string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const string WIN2PDF_PRODUCT = "Win2PDF";
    public const string WIN2PDF_PLUGIN = "Win2PDF Invoice Renamer";
    public const string WIN2PDF_DESTFOLDER = "convert folder dest";
    public const string WIN2PDF_ERRORFOLDER = "convert folder error";
    public const string WIN2PDF_COMPLETEDFOLDER = "convert folder completed";

    public static void Main(string[] args)
    {
        // Check if the program is provided with at least one argument (the input PDF file path)
        if (args.Length < 1)
        {
            writeToEventLog("Usage: PDFRenameInvoice <input_pdf>",EventLogEntryType.Error);
            return;
        }

        string inputPdf = args[0];
        string newFilename = null;

        try
        {
            // Attempt to generate a new filename based on the content of the PDF
            //newFilename = GetNewFilenameFromSearch(inputPdf);
            newFilename = GetNewFilenameFromField(inputPdf);

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
                    writeToEventLog($"Renamed PDF to: {outputPdf}", EventLogEntryType.Information);
            }
            else
            {
                // Save to an error folder
                string directory = Path.GetDirectoryName(inputPdf) + $"\\error";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string outputPdf = Path.Combine(directory, Path.GetFileName(inputPdf));
                File.Move(inputPdf, outputPdf); 
                writeToEventLog($"No matching account number found. PDF renamed to {outputPdf}", EventLogEntryType.Warning);
            }
        }
        catch (Exception ex)
        {
            var exception_description = string.Format("exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);

            // Catch and display any errors that occur during the renaming process
            writeToEventLog(exception_description, EventLogEntryType.Error);
        }
    }

    private static void writeToEventLog(string message, System.Diagnostics.EventLogEntryType et)
    {
        // Write the message to the Windows Event Log
        using (EventLog eventLog = new EventLog("Application"))
        {
            eventLog.Source = "Win2PDF";
            eventLog.WriteEntry(WIN2PDF_PLUGIN + ": " + message, et);
        }
        Console.WriteLine(message);
    }

    private static string GetRegistryString(string printername, string valueName)
    {
        string registryPath = @"Software\Dane Prairie Systems\" + printername;

        try
        {
            // Open the registry key
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    // Read the value
                    object value = key.GetValue(valueName);
                    if (value is string stringValue)
                    {
                        return stringValue; // Return the string value
                    }
                    else
                    {
                        writeToEventLog($"The registry value '{valueName}' is not a string.",EventLogEntryType.Error);
                        
                    }
                }
                else
                {
                    writeToEventLog($"Registry key '{registryPath}' not found.", EventLogEntryType.Error);
                }
            }
        }
        catch (Exception ex)
        {
            writeToEventLog($"Error reading registry value: {ex.Message}", EventLogEntryType.Error);
        }

        return null; // Return null if the value is not found or an error occurs
    }

    private static string GetNewFilenameFromSearch(string inputPdf)
    {
        // Define the labels to search for in the PDF content
        string[] searchlabels = { "ACCOUNT NO:", "VISIT #:" }; // Replace with your specific search labels
        string invoiceNumber = null;

        // Iterate through the search labels to find a matching account number
        foreach (string searchlabel in searchlabels)
        {
            invoiceNumber = ExtractInvoiceNumberFromSearch(inputPdf, searchlabel);
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                break; // Stop searching once a match is found
            }
        }

        return invoiceNumber; // Return the extracted account number or null if not found
    }

    private static string GetNewFilenameFromField(string inputPdf)
    {
        // Define the printer name to search for in the PDF content
        string printername = "Win2PDF"; // Replace with your specific printer name
        string invoiceNumber = null;
        // set the field bounds in points
        // This value can be found by defining a content filed in the Win2PDF Desktop App,
        // and then viewing "PDFAutoNameContentField" in the registry at "HKEY_CURRENT_USER\Software\VB and VBA Program Settings\Dane Prairie Systems\Win2PDF"
        Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, "PDFAutoNameContentField", "0,255.39156424386158,10.919076192251207,130.02812517438616,58.841688369353733");

        // Extract the invoice number from the specified field in the PDF
        invoiceNumber = ExtractInvoiceNumberFromField(inputPdf, printername);
        return invoiceNumber; // Return the extracted account number or null if not found
    }

    private static string ExtractInvoiceNumberFromSearch(string inputPdf, string searchString)
    {
        string invoiceNumber = null;

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
                            invoiceNumber = output.Trim();
                        }
                        else
                        {
                            // Log an error if the command fails
                            writeToEventLog($"Win2PDF command failed with exit code: {process.ExitCode}", EventLogEntryType.Error);
                        }
                    }
                }
            }
            else
            {
                // Inform the user if Win2PDF is not installed
                writeToEventLog("Win2PDF is not installed.", EventLogEntryType.Error);
            }
        }
        catch (Exception ex)
        {
            // Catch and display any errors that occur during the extraction process
            writeToEventLog($"Error executing Win2PDF command: {ex.Message}", EventLogEntryType.Error);
        }

        return invoiceNumber; // Return the extracted account number or null if not found
    }
    private static string ExtractInvoiceNumberFromField(string inputPdf, string printername)
    {
        string invoiceNumber = null;

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
                // win2pdfd.exe getcontentfield "printername" "sourcepdf" "password" 
                // - "printername": Name of the printer (e.g., "Win2PDF")
                // - "sourcepdf": Path to the PDF file
                // - "password": Password for the PDF (use "" if not password-protected)

                // Set up the process to execute the Win2PDF command
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = win2pdfcmdline,
                    Arguments = $"getcontentfield \"{printername}\" \"{inputPdf}\" \"\"", // Search for the specified text in the PDF
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
                            invoiceNumber = output.Trim();
                        }
                        else
                        {
                            // Log an error if the command fails
                            writeToEventLog($"Win2PDF command failed with exit code: {process.ExitCode}", EventLogEntryType.Error);
                        }
                    }
                }
            }
            else
            {
                // Inform the user if Win2PDF is not installed
                writeToEventLog("Win2PDF is not installed.", EventLogEntryType.Error);
            }
        }
        catch (Exception ex)
        {
            // Catch and display any errors that occur during the extraction process
            writeToEventLog($"Error executing Win2PDF command: {ex.Message}", EventLogEntryType.Error);
        }

        return invoiceNumber; // Return the extracted account number or null if not found
    }
}
