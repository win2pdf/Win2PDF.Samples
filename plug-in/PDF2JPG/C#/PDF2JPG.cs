﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;

static class PDF2JPG
{
    // Constants for Win2PDF settings and error codes
    public const string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const string WIN2PDF_PRODUCT = "Win2PDF";
    public const string JPG_RESOLUTION = "JPGResolution";
    public const string PLUG_IN_NAME = "Win2PDF PDF2JPG Plug-in";

    public const int ERROR_LOCKED = 212;
    public const int ERROR_INVALID_FUNCTION = 2;
    public const int ERROR_FILE_NOT_FOUND = 2;
    public const int ERROR_ACCESS_DENIED = 5;
    public const int ERROR_BAD_FORMAT = 11;
    public const int ERROR_INVALID_PARAMETER = 87;
    public const int ERROR_SUCCESS = 0;

    /// <summary>
    /// Main entry point for the application.
    /// Converts a PDF file to a JPG file using the Win2PDF command line tool.
    /// </summary>
    /// <param name="args">Command line arguments. Expects the path to a PDF file as the first argument.</param>
    public static void Main(string[] args)
    {
        try
        {
            // Retrieve the JPG resolution from saved settings or prompt the user for input
            string resolution = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, JPG_RESOLUTION, "");

            if (args.Length == 0 || resolution == "")
            {
                // Prompt the user to enter the JPG resolution if not provided
                resolution = Interaction.InputBox("Enter JPG Resolution:", PLUG_IN_NAME, "100");
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, JPG_RESOLUTION, resolution);
            }
            else if (args.Length == 1)
            {
                System.Diagnostics.Process newProc;
                var win2pdfcmdline = Environment.SystemDirectory;

                // Determine the path to the Win2PDF command line executable based on the system architecture
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
                    // Check if the input file has a .PDF extension
                    String ext = Path.GetExtension(args[0]).ToUpper();
                    if (ext == ".PDF")
                    {
                        // Construct the output JPG file name
                        string jpgname = System.IO.Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(args[0])) + ".jpg";

                        // Command line format: pdf2jpg "sourcepdf" "destjpg" pagenumber xresolution yresolution
                        // Notes:
                        // - If "pagenumber" is 0, all pages are converted as separate files (e.g., destjpg.1.jpg, destjpg.2.jpg, ...).
                        // - To convert a page range, specify "startpage" and "endpage" instead of "pagenumber".
                        // - To set the output resolution, specify "xresolution" and "yresolution". Default is 300 DPI.
                        // - The "pdf2jpggray" variation saves the output as a grayscale image.
                        string arguments1 = string.Format("pdf2jpg \"{0}\" \"{1}\" 0 {2} {3}", args[0], jpgname, resolution, resolution);

                        // Configure the process start info
                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                        {
                            var withBlock = startInfo;
                            withBlock.Arguments = arguments1;
                            withBlock.WindowStyle = ProcessWindowStyle.Hidden;
                        }

                        // Execute the command
                        newProc = System.Diagnostics.Process.Start(startInfo);
                        newProc.WaitForExit();

                        // Handle the process exit codes
                        if (newProc.HasExited)
                        {
                            switch (newProc.ExitCode)
                            {
                                case ERROR_SUCCESS:
                                    {
                                        // Delete the PDF file on successful conversion
                                        File.Delete(args[0]);
                                        break;
                                    }

                                case ERROR_FILE_NOT_FOUND:
                                    {
                                        System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF could not find file: {0}", win2pdfcmdline), PLUG_IN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        break;
                                    }

                                case ERROR_ACCESS_DENIED:
                                    {
                                        System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0}", win2pdfcmdline), PLUG_IN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        break;
                                    }

                                default:
                                    {
                                        MessageBox.Show(string.Format("Win2PDF command line failed: {0}, error code {1}", win2pdfcmdline, newProc.ExitCode), PLUG_IN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        // Show an error message if the input file is not a PDF
                        System.Windows.Forms.MessageBox.Show(string.Format("Invalid save as type {0}. The save as type must be set to PDF", ext), PLUG_IN_NAME);
                    }
                }
                else
                {
                    // Show an error message if Win2PDF is not installed
                    System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"), PLUG_IN_NAME);
                }
            }
            else
            {
                // Show an error message for invalid number of parameters
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions and log them to the Windows Event Log
            var exception_description = string.Format("{0} exception {1}, stack {2}, targetsite {3}", PLUG_IN_NAME, ex.Message, ex.StackTrace, ex.TargetSite);
            MessageBox.Show(exception_description);
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
