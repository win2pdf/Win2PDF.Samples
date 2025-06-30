// ==============================================================================
// PDF Blank Page Remover
// ==============================================================================
// This program removes blank pages from PDF files using the Win2PDF command line tool.
// It validates input, locates the Win2PDF executable, and executes the removal command.
// 
// Requirements:
// - Win2PDF must be installed with a valid license
// - PDF file must exist and be accessible
// 
// Exit behavior:
// - Returns silently on success or error (no explicit exit codes)
// ==============================================================================

using System;
using System.IO;
using System.Diagnostics;

namespace PDFDeleteBlankPages
{
    /// <summary>
    /// Main program class for removing blank pages from PDF files
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Orchestrates the entire process of removing blank pages from a PDF file.
        /// </summary>
        /// <param name="args">Command line arguments - expects a single PDF file path</param>
        static void Main(string[] args)
        {
            // ==============================================================================
            // STEP 1: GET AND PARSE COMMAND LINE ARGUMENTS
            // ==============================================================================

            // In C#, command line arguments are passed directly to Main()
            // Unlike VB.NET, this includes all arguments (no need for special handling)

            // ==============================================================================
            // STEP 2: VALIDATE ARGUMENT COUNT
            // ==============================================================================

            // Ensure exactly one argument (the PDF file path) is provided
            // If not, display usage information and exit
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: RemoveBlanks.exe \"C:\\path\\to\\your\\document.pdf\"");
                Console.WriteLine("Error: Please provide a single path to a PDF file.");
                // Exit the application without processing
                // Note: No explicit exit code is returned (defaults to 0)
                return;
            }

            // Extract the PDF file path from the first (and only) argument
            string pdfFilePath = args[0];

            // ==============================================================================
            // STEP 3: VALIDATE FILE EXISTENCE
            // ==============================================================================

            // Check if the specified PDF file actually exists on the file system
            // This prevents attempting to process non-existent files
            if (!File.Exists(pdfFilePath))
            {
                Console.WriteLine($"Error: The file was not found at '{pdfFilePath}'");
                return; // Exit if file doesn't exist
            }

            // ==============================================================================
            // STEP 4: VALIDATE FILE TYPE (PDF EXTENSION)
            // ==============================================================================

            // Ensure the file has a .pdf extension (case-insensitive check)
            // This helps prevent processing non-PDF files that might cause errors
            if (!Path.GetExtension(pdfFilePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Error: Not a PDF '{pdfFilePath}'");
                return; // Exit if not a PDF file
            }

            // Inform user that processing is beginning
            Console.WriteLine($"Processing file: {pdfFilePath}");

            // ==============================================================================
            // STEP 5: LOCATE WIN2PDF EXECUTABLE
            // ==============================================================================

            // Start with the Windows system directory (usually C:\Windows\System32)
            string win2pdfPath = Environment.SystemDirectory;

            // Build the path to Win2PDF executable based on system architecture
            // Win2PDF installs different versions for different processor architectures

            // Check if running on ARM64 architecture (newer ARM-based Windows devices)
            string processorArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine);

            if (processorArchitecture == "ARM64")
            {
                // ARM64 path: typically C:\Windows\System32\spool\drivers\arm64\3\win2pdfd.exe
                win2pdfPath = Path.Combine(win2pdfPath, "spool", "drivers", "arm64", "3", "win2pdfd.exe");
            }
            else if (Environment.Is64BitOperatingSystem)
            {
                // 64-bit x64 path: typically C:\Windows\System32\spool\drivers\x64\3\win2pdfd.exe
                win2pdfPath = Path.Combine(win2pdfPath, "spool", "drivers", "x64", "3", "win2pdfd.exe");
            }
            else
            {
                // 32-bit x86 path: typically C:\Windows\System32\spool\drivers\w32x86\3\win2pdfd.exe
                win2pdfPath = Path.Combine(win2pdfPath, "spool", "drivers", "w32x86", "3", "win2pdfd.exe");
            }

            // Verify that the Win2PDF executable actually exists at the determined path
            // If not found, Win2PDF may not be installed or may not have command line support
            if (!File.Exists(win2pdfPath))
            {
                Console.WriteLine("Error: Could not find win2pdfd.exe.");
                Console.WriteLine("Please ensure Win2PDF is installed.");
                return; // Exit if Win2PDF executable is not found
            }

            // ==============================================================================
            // STEP 6: CONSTRUCT COMMAND ARGUMENTS
            // ==============================================================================

            // Build the command line arguments for Win2PDF
            // Format: deleteblankpages "sourcefile" "destfile"
            // Using the same file path for both source and destination overwrites the original
            // Quotes around file paths handle spaces in file names
            string commandArguments = $"deleteblankpages \"{pdfFilePath}\" \"{pdfFilePath}\"";

            // ==============================================================================
            // STEP 7: EXECUTE WIN2PDF COMMAND
            // ==============================================================================

            try
            {
                // Configure the process that will run the Win2PDF command
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    // Set the executable to run (the Win2PDF command line tool)
                    FileName = win2pdfPath,

                    // Set the arguments to pass to the executable
                    Arguments = commandArguments,

                    // UseShellExecute = false allows us to:
                    // - Redirect standard input/output/error streams
                    // - Have more control over the process execution
                    // - Run the process directly without going through the Windows shell
                    UseShellExecute = false,

                    // CreateNoWindow = true prevents a command prompt window from appearing
                    // This keeps the interface clean for the user
                    CreateNoWindow = true
                };

                // Inform the user that processing is starting
                Console.WriteLine("Executing Win2PDF command to remove blank pages...");

                // Start the Win2PDF process and manage its lifecycle
                using (Process win2pdfProcess = Process.Start(startInfo))
                {
                    // Wait indefinitely for the Win2PDF process to complete
                    // This blocks the current thread until Win2PDF finishes processing
                    // Note: This could potentially hang if Win2PDF encounters issues
                    win2pdfProcess.WaitForExit();

                    // ==============================================================================
                    // STEP 8: EVALUATE RESULTS AND PROVIDE FEEDBACK
                    // ==============================================================================

                    // Check the exit code returned by the Win2PDF process
                    // By convention, 0 indicates successful completion
                    // Non-zero values typically indicate various types of errors
                    if (win2pdfProcess.ExitCode == 0)
                    {
                        // Success - inform user that blank pages were removed
                        Console.WriteLine("Success! Blank pages have been removed.");
                    }
                    else
                    {
                        // Failure - inform user about the error condition
                        Console.WriteLine($"Win2PDF process finished with a non-zero exit code: {win2pdfProcess.ExitCode}");
                        Console.WriteLine("The file may not have been processed correctly.");

                        // Note: The original file may be corrupted or unchanged
                        // Consider implementing backup/restore functionality for production use
                    }
                } // Automatically disposes of the Process object and releases resources
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors that occur during process execution
                // This could include:
                // - File access permissions issues  
                // - Win2PDF executable corruption
                // - System resource exhaustion
                // - Security policy restrictions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");

                // Note: In a production environment, you might want to:
                // - Log the full exception details (including stack trace)
                // - Attempt to restore from a backup if the original file was corrupted
                // - Return specific exit codes for different error types
            }

            // ==============================================================================
            // END OF MAIN PROCESSING
            // ==============================================================================
            // At this point, the program will exit naturally
            // No explicit cleanup is needed due to the using statement and proper resource management
        }
    }
}