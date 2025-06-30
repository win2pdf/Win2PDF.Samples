Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics

Module PDFDeleteBlankPages

    ''' <summary>
    ''' The main entry point for the application.
    ''' </summary>
    Sub Main()
        ' Get command line arguments
        Dim args As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
        args = My.Application.CommandLineArgs

        ' 1. VALIDATE ARGUMENTS
        ' Check if exactly one argument is provided.
        If args.Count <> 1 Then
            Console.WriteLine("Usage: RemoveBlanks.exe ""C:\path\to\your\document.pdf""")
            Console.WriteLine("Error: Please provide a single path to a PDF file.")
            ' Exit with an error code
            Return
        End If

        Dim pdfFilePath As String = args(0)

        ' 2. VALIDATE FILE PATH
        ' Check if the provided PDF file actually exists.
        If Not File.Exists(pdfFilePath) Then
            Console.WriteLine($"Error: The file was not found at '{pdfFilePath}'")
            Return
        End If

        ' 3. Ensure the file has a .PDF extension (case-insensitive)
        If Path.GetExtension(args(0)).ToUpper = "PDF" Then
            Console.WriteLine($"Error: Not a PDF '{pdfFilePath}'")
            Return
        End If

        Console.WriteLine($"Processing file: {pdfFilePath}")

        ' 4. LOCATE WIN2PDF EXECUTABLE
        Dim win2pdfPath = Environment.SystemDirectory

        ' Determine the path to the Win2PDF command line executable based on system architecture
        If System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) = "ARM64" Then
            win2pdfPath += "\spool\drivers\arm64\3\win2pdfd.exe"
        ElseIf Environment.Is64BitOperatingSystem Then
            win2pdfPath += "\spool\drivers\x64\3\win2pdfd.exe"
        Else
            win2pdfPath += "\spool\drivers\w32x86\3\win2pdfd.exe"
        End If

        If Not File.Exists(win2pdfPath) Then
            Console.WriteLine("Error: Could not find win2pdfd.exe.")
            Console.WriteLine("Please ensure Win2PDF is installed.")
            Return
        End If

        ' 5. CONSTRUCT AND EXECUTE THE COMMAND
        ' Command format: deleteblankpages "sourcefile" "destfile"
        ' We use the same path for both to overwrite the original file.
        Dim commandArguments As String = $"deleteblankpages ""{pdfFilePath}"" ""{pdfFilePath}"""

        Try
            ' Configure the process to run the command
            Dim startInfo As New ProcessStartInfo()
            startInfo.FileName = win2pdfPath
            startInfo.Arguments = commandArguments
            startInfo.UseShellExecute = False ' Required to redirect output or run without a shell
            startInfo.CreateNoWindow = True    ' Hides the command window

            Console.WriteLine("Executing Win2PDF command to remove blank pages...")

            ' Start the process and wait for it to finish
            Using win2pdfProcess As Process = Process.Start(startInfo)
                win2pdfProcess.WaitForExit() ' Wait indefinitely for the process to exit.

                ' 6. PROVIDE FEEDBACK
                ' Check the exit code. 0 typically means success.
                If win2pdfProcess.ExitCode = 0 Then
                    Console.WriteLine("Success! Blank pages have been removed.")
                Else
                    Console.WriteLine($"Win2PDF process finished with a non-zero exit code: {win2pdfProcess.ExitCode}")
                    Console.WriteLine("The file may not have been processed correctly.")
                End If
            End Using

        Catch ex As Exception
            Console.WriteLine($"An unexpected error occurred: {ex.Message}")
        End Try

    End Sub


End Module
