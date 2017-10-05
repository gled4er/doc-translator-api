using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.IO;
using Microsoft.SharePoint.Client;
using System.Security;
using TranslationAssistant.TranslationServices.Core;
using TranslationAssistant.Business;
using TestApp.Authentication;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
          
            // Authenticate 
            var userToken =  AuthenticationHelper.GetTokenForUser(ConfigurationManager.AppSettings["AADTenant"], ConfigurationManager.AppSettings["AADAppClientID"]).Result;
            // Find People by name

            // Provide Meeting Slots options by date

            // Select meeting slot and room

            // Get document links
            var documentLinks = TranslateFile("documents", "AI05.pptx", "Japanese", "English");

            // Schedule meeting 

        }

        public static DocumentLinks TranslateFile(string containerName, string fileName, string originalLanguage, string translationLanguage)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            var blockBlob = container.GetBlockBlobReference(fileName);
            // Copy File
            blockBlob.DownloadToFile(blockBlob.Name, FileMode.Create);

            // Translate File
            TranslationServiceFacade.Initialize(ConfigurationManager.AppSettings["ApiKey"]);

            DocumentTranslationManager.DoTranslation(blockBlob.Name, false, originalLanguage, translationLanguage);

            var languageCode = TranslationServiceFacade.AvailableLanguages.Where(p => p.Value == translationLanguage).Select(p => p.Key).FirstOrDefault();

            var extension = GetExtension(fileName);

            var translatedDocumentName = blockBlob.Name.Replace(string.Format(".{0}", extension), string.Format(".{0}.{1}", languageCode, extension));

            // Move original file to SharePoint
            var originalFileUrl = CopyFileToSharePoint(blockBlob.Name);

            // Move trnslated file to SharePoint
            var translatedFileUrl = CopyFileToSharePoint(translatedDocumentName);

            // Delete original file
            if (System.IO.File.Exists(blockBlob.Name))
            {
                System.IO.File.Delete(blockBlob.Name);
            }

            // Delete translated file
            if(System.IO.File.Exists(translatedDocumentName))
            {
                System.IO.File.Delete(translatedDocumentName);
            }

            return new DocumentLinks
            {
                OriginalDocument = originalFileUrl,
                TranslatedDocument = translatedFileUrl
            };
        }

        public static string GetExtension(string fileName)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }
            var valuesArray = fileName.Split(new[] { "." }, StringSplitOptions.None);
            return valuesArray[valuesArray.Length - 1];
        }

        public static string CopyFileToSharePoint(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                using (var fileStream = System.IO.File.Open(fileName, FileMode.Open))
                {
                    using (var clientContext = new ClientContext(ConfigurationManager.AppSettings["SPSiteUrl"]))
                    {
                        var passWord = new SecureString();

                        foreach (char c in ConfigurationManager.AppSettings["SPPassword"].ToCharArray()) passWord.AppendChar(c);

                        clientContext.Credentials = new SharePointOnlineCredentials(ConfigurationManager.AppSettings["SPUserName"], passWord);

                        Web web = clientContext.Web;

                        clientContext.Load(web);

                        clientContext.ExecuteQuery();

                        SaveBinaryDirect(clientContext, "Documents", fileName, fileStream);

                        var filePath = string.Format("/Shared%20Documents/Forms/AllItems.aspx?id=/sites/{0}/Shared%20Documents/{1}&parent=/sites/{0}/Shared%20Documents", ConfigurationManager.AppSettings["SPSiteName"], fileName);

                        var documentUrl = string.Format("{0}{1}", ConfigurationManager.AppSettings["SPSiteUrl"], filePath);

                        return documentUrl;
                    }
                }
            }

            return string.Empty;
        }

        public static void SaveBinaryDirect(ClientContext ctx, string libraryName, string fileName, Stream memoryStream)
        {
            Web web = ctx.Web;
            //Ensure that target library exists, create if is missing
            if (!LibraryExists(ctx, web, libraryName))
            {
               CreateLibrary(ctx, web, libraryName);
            }

            List docs = ctx.Web.Lists.GetByTitle(libraryName);
            ctx.Load(docs, l => l.RootFolder);
            // Get the information about the folder that will hold the file
            ctx.Load(docs.RootFolder, f => f.ServerRelativeUrl);
            ctx.ExecuteQuery();

            Microsoft.SharePoint.Client.File.SaveBinaryDirect(ctx, string.Format("{0}/{1}", docs.RootFolder.ServerRelativeUrl, fileName, true), memoryStream, true);
        }

        private static bool LibraryExists(ClientContext ctx, Web web, string libraryName)
        {
            ListCollection lists = web.Lists;
            IEnumerable<List> results = ctx.LoadQuery<List>(lists.Where(list => list.Title == libraryName));
            ctx.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                return true;
            }

            return false;
        }

        private static void  CreateLibrary(ClientContext ctx, Web web, string libraryName)
        {
            // Create library to the web
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = libraryName;
            creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
            List list = web.Lists.Add(creationInfo);
            ctx.ExecuteQuery();
        }
    }

    public class DocumentLinks
    {
        public string OriginalDocument { get; set; }
        public string TranslatedDocument { get; set; }
    }
}
