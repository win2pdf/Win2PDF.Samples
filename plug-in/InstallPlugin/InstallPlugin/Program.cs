using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace InstallPlugin
{
    internal static class Program
    {
        /// <summary>
        /// Checks if the specified printer is a Win2PDF printer.
        /// </summary>
        /// <param name="printername">The name of the printer to check.</param>
        /// <returns>True if the printer is a Win2PDF printer, otherwise false.</returns>
        private static bool isWin2PDFPrinter(string printername)
        {
            bool bisWin2PDFPrinter = false;
            try
            {
                // Query the Win32_Printer class to find the specified printer
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", string.Format("SELECT * FROM Win32_Printer WHERE Name LIKE '%{0}'", printername));

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    // Check if the printer's driver name matches "Win2PDF-A"
                    if (queryObj["DriverName"].ToString().Equals("Win2PDF-A"))
                    {
                        bisWin2PDFPrinter = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("Install Plug In Error: " + ex.Message, "Install Plug In Error", MessageBoxButtons.OK);
            }
            return bisWin2PDFPrinter;
        }

        /// <summary>
        /// Displays a print dialog to allow the user to select a printer.
        /// Ensures that the selected printer is not a Win2PDF printer.
        /// </summary>
        /// <param name="printername">The name of the selected printer (output parameter).</param>
        /// <returns>True if a valid printer is selected, otherwise false.</returns>
        static bool selectWin2PDFprinter(ref string printername)
        {
            printername = string.Empty;
            bool retval = false;
            do
            {
                // Create and configure the print dialog
                System.Windows.Forms.PrintDialog dlgPrint = new System.Windows.Forms.PrintDialog();
                try
                {
                    var withBlock = dlgPrint;
                    withBlock.AllowSelection = true;
                    withBlock.ShowNetwork = false;

                    // Show the print dialog
                    if (dlgPrint.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // Check if the selected printer is a Win2PDF printer
                        if (!isWin2PDFPrinter(dlgPrint.PrinterSettings.PrinterName))
                        {
                            MessageBox.Show(dlgPrint.PrinterSettings.PrinterName + " is not a Win2PDF printer. Select a Win2PDF printer.", "Install Plug In Error", MessageBoxButtons.OK);
                        }
                        else
                        {
                            // Set the selected printer name and return success
                            printername = dlgPrint.PrinterSettings.PrinterName;
                            retval = true;
                            break;
                        }
                    }
                    else
                    {
                        // User canceled the dialog
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // Display an error message if an exception occurs
                    MessageBox.Show("Install Plug In Error: " + ex.Message, "Install Plug In Error", MessageBoxButtons.OK);
                    break;
                }
            }
            while (true);

            return retval;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles and set text rendering compatibility
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Open a file dialog to allow the user to select a plug-in executable or script
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select the plug-in executable or script",
                Filter = "All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file path
                string selectedFilePath = openFileDialog.FileName;
                string printername = string.Empty;

                // Allow the user to select a printer
                if (selectWin2PDFprinter(ref printername) == true)
                {
                    // Launch the ConfigurePlugin form with the selected file and printer
                    Application.Run(new ConfigurePlugin(selectedFilePath, printername));
                }
            }
        }
    }
}
