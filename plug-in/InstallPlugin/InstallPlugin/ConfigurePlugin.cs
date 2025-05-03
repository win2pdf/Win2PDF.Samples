using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace InstallPlugin
{
    public partial class ConfigurePlugin : Form
    {
        // Base registry path for Dane Prairie Systems
        private const string REGSTR_DANEPRAIRIE_BASE = "SOFTWARE\\Dane Prairie Systems\\";
        // Registry values for plug-ins 
        private const string REGSTR_DEFAULT_POST_ACTION = "Default Post Action";
        private const string REGSTR_DEFAULT_BATCH_ACTION = "Default Batch Action";
        private const string REGSTR_DEFAULT_WATCH_ACTION = "Default Watch Action";

        private string _pluginPath = string.Empty; // Path to the plugin executable
        private string _printername = string.Empty; // Name of the printer

        public ConfigurePlugin(string pluginExeName, string printername)
        {
            _pluginPath = pluginExeName;
            _printername = printername;
            InitializeComponent();

            // Display the plugin file name in the text box
            txtPlugIn.Text = Path.GetFileName(_pluginPath);

            // Set the default radio button selection to "Current User"
            rdoCurrentUser.Checked = true;
        }

        /// <summary>
        /// Deletes a registry value for the specified printer and value name.
        /// </summary>
        /// <param name="allUsers">True to delete from HKLM, false to delete from HKCU.</param>
        /// <param name="PrinterName">The name of the printer.</param>
        /// <param name="ValueName">The name of the registry value to delete.</param>
        void myDeleteValue(Boolean allUsers, String PrinterName, String ValueName)
        {
            try
            {
                // Determine the base registry key based on the allUsers flag
                RegistryKey baseKey = allUsers ? Registry.LocalMachine : Registry.CurrentUser;

                // Construct the full registry path
                string registryPath = REGSTR_DANEPRAIRIE_BASE + PrinterName;

                // Open the registry key
                using (RegistryKey key = baseKey.OpenSubKey(registryPath, true))
                {
                    if (key == null)
                    {
                        throw new InvalidOperationException($"Failed to open registry key: {registryPath}");
                    }

                    // Delete the value from the registry
                    key.DeleteValue(ValueName, false);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error or show a message box)
                MessageBox.Show($"Error deleting registry value: {ex.Message}", "Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sets a registry value for the specified printer and value name.
        /// </summary>
        /// <param name="allUsers">True to write to HKLM, false to write to HKCU.</param>
        /// <param name="PrinterName">The name of the printer.</param>
        /// <param name="ValueName">The name of the registry value to set.</param>
        /// <param name="ValueData">The data to write to the registry value.</param>
        /// <param name="kind">The type of the registry value.</param>
        void mySetValue(Boolean allUsers, String PrinterName, String ValueName, object ValueData, RegistryValueKind kind)
        {
            try
            {
                // Determine the base registry key based on the allUsers flag
                RegistryKey baseKey = allUsers ? Registry.LocalMachine : Registry.CurrentUser;

                // Construct the full registry path
                string registryPath = REGSTR_DANEPRAIRIE_BASE + PrinterName;

                // Open or create the registry key
                using (RegistryKey key = baseKey.CreateSubKey(registryPath, true))
                {
                    if (key == null)
                    {
                        throw new InvalidOperationException($"Failed to open or create registry key: {registryPath}");
                    }

                    // Set the value in the registry
                    key.SetValue(ValueName, ValueData, kind);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error or show a message box)
                MessageBox.Show($"Error writing to registry: {ex.Message}", "Registry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the registry based on the checkbox state and user selection.
        /// </summary>
        /// <param name="checkboxchecked">True if the checkbox is checked, false otherwise.</param>
        /// <param name="valuename">The name of the registry value to update.</param>
        /// <param name="bAllUsers">True to update HKLM, false to update HKCU.</param>
        private void updateRegistry(bool checkboxchecked, string valuename, bool bAllUsers)
        {
            if (checkboxchecked)
            {
                // Set the value for the selected user scope and delete it from the other scope
                mySetValue(bAllUsers, _printername, valuename, _pluginPath, RegistryValueKind.ExpandString);
                myDeleteValue(!bAllUsers, _printername, valuename);
            }
            else
            {
                // Delete the value for the selected user scope
                myDeleteValue(bAllUsers, _printername, valuename);
            }
        }

        /// <summary>
        /// Handles the OK button click event to update the registry and close the form.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Update registry values to install plug-ins based on the state of the checkboxes and user selection
            updateRegistry(chkPrinter.Checked, REGSTR_DEFAULT_POST_ACTION, rdoAllUsers.Checked);
            updateRegistry(chkBatch.Checked, REGSTR_DEFAULT_BATCH_ACTION, rdoAllUsers.Checked);
            updateRegistry(chkWatch.Checked, REGSTR_DEFAULT_WATCH_ACTION, rdoAllUsers.Checked);

            // Close the form
            Close();
        }

        /// <summary>
        /// Handles the Cancel button click event to close the form without making changes.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
