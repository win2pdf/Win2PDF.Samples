using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;

using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;

static class PDFAWSS3Upload
{
    const int ERROR_SHARING_VIOLATION = 32;
    public const string PROFILE_NAME = "Win2PDF-S3-Upload";

    public class AmazonUploader
    {
        public bool sendFileToS3(string localFilePath, string bucketName, string subDirectoryInBucket, string fileNameInS3)
        {
            var chain = new CredentialProfileStoreChain();
            CredentialProfile profile;
            if (chain.TryGetProfile(PROFILE_NAME, out profile))
            {
                IAmazonS3 client = new Amazon.S3.AmazonS3Client(profile.GetAWSCredentials(null), profile.Region);

                // create a TransferUtility instance passing it the IAmazonS3 created in the first step
                TransferUtility utility = new TransferUtility(client);
                // making a TransferUtilityUploadRequest instance
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
                {
                    request.BucketName = bucketName; //no subdirectory just bucket name
                }
                else
                {   // subdirectory and bucket name
                    request.BucketName = bucketName + @"/" + subDirectoryInBucket;
                }
                request.Key = fileNameInS3; //file name up in S3
                request.FilePath = localFilePath; //local file name
                utility.Upload(request); //start the transfer

                return true; //indicate that the file was sent
            }
            else
            {
                MessageBox.Show("Invalid AWS credentials");
                return false;
            }
        }
    }

    public const string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const string WIN2PDF_PRODUCT = "Win2PDF";
    public const string WIN2PDF_BUCKET_NAME = "Win2PDF Bucket Name";
    public const string WIN2PDF_FOLDER_NAME = "Win2PDF Folder Name";
    public const string WIN2PDF_UPLOAD_OPERATION = "Win2PDF Upload Operation";
    public const string WIN2PDF_REGION_NAME = "Win2PDF Region Name";


    public static void Main(string[] args)
    {

        try
        {
            if (args.Length == 0)
            {
                PDFAWSConfig.ConfigureS3 s3config = new PDFAWSConfig.ConfigureS3();
                s3config.ShowDialog();
            }
            else if (args.Length == 1)
            {
                if (File.Exists(args[0]))
                {
                    string fileToBackup = args[0]; 
                    string myBucketName = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, WIN2PDF_BUCKET_NAME, ""); 
                    string s3DirectoryName = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, WIN2PDF_FOLDER_NAME, "");
                    string s3FileName = Path.GetFileName(args[0]); 

                    if (myBucketName.Length > 0)
                    {
                        try
                        {
                            AmazonUploader myUploader = new AmazonUploader();
                            myUploader.sendFileToS3(fileToBackup, myBucketName, s3DirectoryName, s3FileName);
                        }
                        catch (AmazonS3Exception ex)
                        {
                            var exception_description = string.Format("Win2PDF plug-in exception \n\n{0}", ex.Message);
                            if (ex.Message.Contains("The bucket you are attempting to access must be addressed using the specified endpoint."))
                            {
                                exception_description += "\n\nCheck bucket name and region.";
                            }
                            else if (ex.Message.Contains("The request signature we calculated does not match the signature you provided."))
                            {
                                exception_description += "\n\nCheck the Secret Access ID.";
                            }
                            MessageBox.Show(exception_description);
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Win2PDF";
                                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
                            }
                        }
                        catch (Exception ex)
                        {
                            var exception_description = string.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);
                            MessageBox.Show(exception_description);
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Win2PDF";
                                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
                            }
                        }
                        finally
                        {
                            String operation = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, WIN2PDF_UPLOAD_OPERATION, "copy");
                            if (operation == "move")
                            {
                                File.Delete(args[0]);
                            }
                        }
                    }
                    else
                    {                        
                        MessageBox.Show("Bucket Name cannot be empty.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            var exception_description = string.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);
            MessageBox.Show(exception_description);
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
