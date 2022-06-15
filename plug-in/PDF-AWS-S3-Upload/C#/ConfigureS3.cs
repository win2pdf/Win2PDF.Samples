using Amazon;
using Amazon.Runtime.CredentialManagement;
using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;

namespace PDFAWSConfig
{
    public partial class ConfigureS3 : Form
    {
        public void WriteProfile(string profileName, string keyId, string secret, RegionEndpoint region)
        {
            var options = new CredentialProfileOptions
            {
                AccessKey = keyId,
                SecretKey = secret
            };
            var profile = new CredentialProfile(profileName, options);
            profile.Region = region;
            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);
        }

        public ConfigureS3()
        {
            InitializeComponent();
        }
        private int RegionTextToIndex(string regiontext)
        {
            string[] regions = {"US East(Ohio)",
                "US East(N.Virginia)",
                "US West(N.California)",
                "US West(Oregon)",
                "Africa(Cape Town)",
                "Asia Pacific(Hong Kong)",
                "Asia Pacific(Jakarta)",
                "Asia Pacific(Mumbai)",
                "Asia Pacific(Osaka)",
                "Asia Pacific(Seoul)",
                "Asia Pacific(Singapore)",
                "Asia Pacific(Sydney)",
                "Asia Pacific(Tokyo)",
                "Canada(Central)",
                "China(Beijing)",
                "China(Ningxia)",
                "Europe(Frankfurt)",
                "Europe(Ireland)",
                "Europe(London)",
                "Europe(Milan)",
                "Europe(Paris)",
                "Europe(Stockholm)",
                "Middle East(Bahrain)",
                "South America(São Paulo)" };

            int i;
            for (i = 0; i < regions.Length; i++)
            {
                if (regiontext == regions[i])
                {
                    return i;
                }
            }
            return 1;//default to US East
        }

        private RegionEndpoint RegionTextToRegionEndpoint(string regiontext)
        {
            RegionEndpoint retval = RegionEndpoint.USEast1;
            switch (regiontext)
            {
                case "US East (Ohio)":
                    retval = RegionEndpoint.USEast2;
                    break;
                case "US East (N. Virginia)":
                    retval = RegionEndpoint.USEast1;
                    break;
                case "US West (N. California)":
                    retval = RegionEndpoint.USWest1;
                    break;
                case "US West (Oregon)":
                    retval = RegionEndpoint.USWest2;
                    break;
                case "Africa (Cape Town)":
                    retval = RegionEndpoint.AFSouth1;
                    break;
                case "Asia Pacific (Hong Kong)":
                    retval = RegionEndpoint.APEast1;
                    break;
                case "Asia Pacific (Jakarta)":
                    retval = RegionEndpoint.APSoutheast3;
                    break;
                case "Asia Pacific (Mumbai)":
                    retval = RegionEndpoint.APSouth1;
                    break;
                case "Asia Pacific (Osaka)":
                    retval = RegionEndpoint.APNortheast3;
                    break;
                case "Asia Pacific (Seoul)":
                    retval = RegionEndpoint.APNortheast2;
                    break;
                case "Asia Pacific (Singapore)":
                    retval = RegionEndpoint.APSoutheast1;
                    break;
                case "Asia Pacific (Sydney)":
                    retval = RegionEndpoint.APSoutheast2;
                    break;
                case "Asia Pacific (Tokyo)":
                    retval = RegionEndpoint.APNortheast1;
                    break;
                case "Canada (Central)":
                    retval = RegionEndpoint.CACentral1;
                    break;
                case "China (Beijing)":
                    retval = RegionEndpoint.CNNorth1;
                    break;
                case "China (Ningxia)":
                    retval = RegionEndpoint.CNNorthWest1;
                    break;
                case "Europe (Frankfurt)":
                    retval = RegionEndpoint.EUCentral1;
                    break;
                case "Europe (Ireland)":
                    retval = RegionEndpoint.EUWest1;
                    break;
                case "Europe (London)":
                    retval = RegionEndpoint.EUWest2;
                    break;
                case "Europe (Milan)":
                    retval = RegionEndpoint.EUSouth1;
                    break;
                case "Europe (Paris)":
                    retval = RegionEndpoint.EUWest3;
                    break;
                case "Europe (Stockholm)":
                    retval = RegionEndpoint.EUNorth1;
                    break;
                case "Middle East (Bahrain)":
                    retval = RegionEndpoint.MESouth1;
                    break;
                case "South America (São Paulo)":
                    retval = RegionEndpoint.SAEast1;
                    break;
                default:
                    retval = RegionEndpoint.USEast1;
                    break;
            }
            return retval;
        }
        private void ConfigureS3_Load(object sender, EventArgs e)
        {
            txtBucketName.Text = Interaction.GetSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_BUCKET_NAME);
            txtFolderName.Text = Interaction.GetSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_FOLDER_NAME);
            //int region =Interaction.GetSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_REGION_NAME, "US East(N.Virginia)");
            string region = Interaction.GetSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_REGION_NAME, "1");
            int regionint;
            if (!Int32.TryParse(region, out regionint))
            {
                regionint = 1; //default to US East
            }
            cboRegion.SelectedIndex = regionint;
            string operation = Interaction.GetSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_UPLOAD_OPERATION);
            if (operation == "move")
            {
                cboOperation.SelectedIndex = 1;
            }
            else
            {
                cboOperation.SelectedIndex = 0;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtAccessKey.Text.Length == 0 || txtSecretAccess.Text.Length == 0)
            {
                MessageBox.Show("keys cannot be empty.");
            }
            else if (txtBucketName.Text.Length == 0 )
            {
                MessageBox.Show("Bucket Name cannot be empty.");
            }
            else
            {
                
                Interaction.SaveSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_BUCKET_NAME, txtBucketName.Text);
                Interaction.SaveSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_FOLDER_NAME, txtFolderName.Text);
                Interaction.SaveSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_REGION_NAME, cboRegion.SelectedIndex.ToString());
                string operation;
                if (cboOperation.SelectedIndex == 1)
                    operation = "move";
                else
                    operation = "copy";
                Interaction.SaveSetting(PDFAWSS3Upload.WIN2PDF_COMPANY, PDFAWSS3Upload.WIN2PDF_PRODUCT, PDFAWSS3Upload.WIN2PDF_UPLOAD_OPERATION, operation);
                WriteProfile(PDFAWSS3Upload.PROFILE_NAME, txtAccessKey.Text, txtSecretAccess.Text, RegionTextToRegionEndpoint(cboRegion.SelectedItem.ToString()));
            }
        }
    }
}
