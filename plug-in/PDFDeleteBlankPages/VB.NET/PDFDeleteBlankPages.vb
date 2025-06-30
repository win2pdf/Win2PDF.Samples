' ==============================================================================
' PDF Blank Page Remover
' ==============================================================================
' This program removes blank pages from PDF files using the Win2PDF command line tool.
' It validates input, locates the Win2PDF executable, and executes the removal command.
' 
' Requirements:
' - Win2PDF must be installed with a valid license
' - PDF file must exist and be accessible
' 
' Exit behavior:
' - Returns silently on success or error (no explicit exit codes)
' ==============================================================================

Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics

Module PDFDeleteBlankPages
    ''' <summary>
    ''' The main entry point for the application.
    ''' Orchestrates the entire process of removing blank pages from a PDF file.
    ''' </summary>
    Sub Main()
        ' ==============================================================================
        ' STEP 1: GET AND PARSE COMMAND LINE ARGUMENTS
        ' ==============================================================================

        ' Retrieve command line arguments passed to the application
        ' Note: My.Application.CommandLineArgs excludes the executable name itself
        Dim args As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
        args = My.Application.CommandLineArgs

        ' ==============================================================================
        ' STEP 2: VALIDATE ARGUMENT COUNT
        ' ==============================================================================

        ' Ensure exactly one argument (the PDF file path) is provided
        ' If not, display usage information and exit
        If args.Count <> 1 Then
            Console.WriteLine("Usage: RemoveBlanks.exe ""C:\path\to\your\document.pdf""")
            Console.WriteLine("Error: Please provide a single path to a PDF file.")
            ' Exit the application without processing
            ' Note: No explicit exit code is returned (defaults to 0)
            Return
        End If

        ' Extract the PDF file path from the first (and only) argument
        Dim pdfFilePath As String = args(0)

        ' ==============================================================================
        ' STEP 3: VALIDATE FILE EXISTENCE
        ' ==============================================================================

        ' Check if the specified PDF file actually exists on the file system
        ' This prevents attempting to process non-existent files
        If Not File.Exists(pdfFilePath) Then
            Console.WriteLine($"Error: The file was not found at '{pdfFilePath}'")
            Return ' Exit if file doesn't exist
        End If

        ' ==============================================================================
        ' STEP 4: VALIDATE FILE TYPE (PDF EXTENSION)
        ' ==============================================================================

        ' Ensure the file has a .pdf extension (case-insensitive check)
        ' This helps prevent processing non-PDF files that might cause errors
        If Not Path.GetExtension(pdfFilePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase) Then
            Console.WriteLine($"Error: Not a PDF '{pdfFilePath}'")
            Return ' Exit if not a PDF file
        End If

        ' Inform user that processing is beginning
        Console.WriteLine($"Processing file: {pdfFilePath}")

        ' ==============================================================================
        ' STEP 5: LOCATE WIN2PDF EXECUTABLE
        ' ==============================================================================

        ' Start with the Windows system directory (usually C:\Windows\System32)
        Dim win2pdfPath = Environment.SystemDirectory

        ' Build the path to Win2PDF executable based on system architecture
        ' Win2PDF installs different versions for different processor architectures

        ' Check if running on ARM64 architecture (newer ARM-based Windows devices)
        If System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) = "ARM64" Then
            ' ARM64 path: typically C:\Windows\System32\spool\drivers\arm64\3\win2pdfd.exe
            win2pdfPath += "\spool\drivers\arm64\3\win2pdfd.exe"
        ElseIf Environment.Is64BitOperatingSystem Then
            ' 64-bit x64 path: typically C:\Windows\System32\spool\drivers\x64\3\win2pdfd.exe
            win2pdfPath += "\spool\drivers\x64\3\win2pdfd.exe"
        Else
            ' 32-bit x86 path: typically C:\Windows\System32\spool\drivers\w32x86\3\win2pdfd.exe
            win2pdfPath += "\spool\drivers\w32x86\3\win2pdfd.exe"
        End If

        ' Verify that the Win2PDF executable actually exists at the determined path
        ' If not found, Win2PDF may not be installed or may not have command line support
        If Not File.Exists(win2pdfPath) Then
            Console.WriteLine("Error: Could not find win2pdfd.exe.")
            Console.WriteLine("Please ensure Win2PDF is installed.")
            Return ' Exit if Win2PDF executable is not found
        End If

        ' ==============================================================================
        ' STEP 6: CONSTRUCT COMMAND ARGUMENTS
        ' ==============================================================================

        ' Build the command line arguments for Win2PDF
        ' Format: deleteblankpages "sourcefile" "destfile"
        ' Using the same file path for both source and destination overwrites the original
        ' Quotes around file paths handle spaces in file names
        Dim commandArguments As String = $"deleteblankpages ""{pdfFilePath}"" ""{pdfFilePath}"""

        ' ==============================================================================
        ' STEP 7: EXECUTE WIN2PDF COMMAND
        ' ==============================================================================

        Try
            ' Configure the process that will run the Win2PDF command
            Dim startInfo As New ProcessStartInfo()

            ' Set the executable to run (the Win2PDF command line tool)
            startInfo.FileName = win2pdfPath

            ' Set the arguments to pass to the executable
            startInfo.Arguments = commandArguments

            ' UseShellExecute = False allows us to:
            ' - Redirect standard input/output/error streams
            ' - Have more control over the process execution
            ' - Run the process directly without going through the Windows shell
            startInfo.UseShellExecute = False

            ' CreateNoWindow = True prevents a command prompt window from appearing
            ' This keeps the interface clean for the user
            startInfo.CreateNoWindow = True

            ' Inform the user that processing is starting
            Console.WriteLine("Executing Win2PDF command to remove blank pages...")

            ' Start the Win2PDF process and manage its lifecycle
            Using win2pdfProcess As Process = Process.Start(startInfo)

                ' Wait indefinitely for the Win2PDF process to complete
                ' This blocks the current thread until Win2PDF finishes processing
                ' Note: This could potentially hang if Win2PDF encounters issues
                win2pdfProcess.WaitForExit()

                ' ==============================================================================
                ' STEP 8: EVALUATE RESULTS AND PROVIDE FEEDBACK
                ' ==============================================================================

                ' Check the exit code returned by the Win2PDF process
                ' By convention, 0 indicates successful completion
                ' Non-zero values typically indicate various types of errors
                If win2pdfProcess.ExitCode = 0 Then
                    ' Success - inform user that blank pages were removed
                    Console.WriteLine("Success! Blank pages have been removed.")
                Else
                    ' Failure - inform user about the error condition
                    Console.WriteLine($"Win2PDF process finished with a non-zero exit code: {win2pdfProcess.ExitCode}")
                    Console.WriteLine("The file may not have been processed correctly.")

                    ' Note: The original file may be corrupted or unchanged
                    ' Consider implementing backup/restore functionality for production use
                End If
            End Using ' Automatically disposes of the Process object and releases resources

        Catch ex As Exception
            ' Handle any unexpected errors that occur during process execution
            ' This could include:
            ' - File access permissions issues  
            ' - Win2PDF executable corruption
            ' - System resource exhaustion
            ' - Security policy restrictions
            Console.WriteLine($"An unexpected error occurred: {ex.Message}")

            ' Note: In a production environment, you might want to:
            ' - Log the full exception details (including stack trace)
            ' - Attempt to restore from a backup if the original file was corrupted
            ' - Return specific exit codes for different error types
        End Try

        ' ==============================================================================
        ' END OF MAIN PROCESSING
        ' ==============================================================================
        ' At this point, the program will exit naturally
        ' No explicit cleanup is needed due to the Using statement and proper resource management
    End Sub
End Module