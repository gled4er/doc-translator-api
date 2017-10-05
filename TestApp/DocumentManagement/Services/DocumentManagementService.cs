using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using Microsoft.SharePoint.Client;
using System.Security;
using TranslationAssistant.TranslationServices.Core;
using TranslationAssistant.Business;
using Autofac;
using TestApp.DocumentManagement.Model;
using TestApp.Utils;
using MicrosoftGraph.Services;
using System.Threading.Tasks;

namespace TestApp.DocumentManagement.Services
{
    /// <summary>
    /// Service managing various files locally required for translating and referencing
    /// </summary>
    public class DocumentManagementService : IDocumentManagementService
    {
        private readonly IStorageManagementService _storageManagementService;
        private readonly IConfigurationService _configurationService;
        private readonly ILoggingService _loggingService;


        /// <summary>
        /// Creates instance <see cref="DocumentManagementService"/>
        /// </summary>
        /// <param name="storageManagementService">Isntance of <see cref="IStorageManagementService"/></param>
        /// <param name="configurationService">Instance of <see cref="IConfigurationService"/></param>
        /// <param name="loggingService">Instance of <see cref="ILoggingService"/></param>
        public DocumentManagementService(IStorageManagementService storageManagementService, IConfigurationService configurationService, ILoggingService loggingService)
        {
            _storageManagementService = storageManagementService;
            _configurationService = configurationService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Tranlslates a document and provides links to the original as well as the translated document
        /// </summary>
        /// <param name="storageContainerName">Name of the storage container</param>
        /// <param name="storageFileName">Name of the storage file (original document for translation)</param>
        /// <param name="originalLanguage">The language of the originial file</param>
        /// <param name="translationLanguage">The language for translating the document</param>
        /// <returns></returns>
        public  async Task<DocumentLinks> TranslateFile(string storageContainerName, string storageFileName, string originalLanguage, string translationLanguage)
        {
            var localFileName = await _storageManagementService.DownloadBlob(storageContainerName, storageFileName);

            // Translate File
            TranslationServiceFacade.Initialize(_configurationService.GetSettingValue("ApiKey"));

            DocumentTranslationManager.DoTranslation(localFileName, false, originalLanguage, translationLanguage);

            var languageCode = TranslationServiceFacade.AvailableLanguages.Where(p => p.Value == translationLanguage).Select(p => p.Key).FirstOrDefault();

            var extension = Helper.GetExtension(storageFileName);

            var translatedDocumentName = localFileName.Replace(string.Format(".{0}", extension), string.Format(".{0}.{1}", languageCode, extension));

            // Move original file to SharePoint
            var originalFileUrl = CopyFileToSharePoint(localFileName);

            // Move trnslated file to SharePoint
            var translatedFileUrl = CopyFileToSharePoint(translatedDocumentName);

            // Delete original file
            if (System.IO.File.Exists(localFileName))
            {
                System.IO.File.Delete(localFileName);
            }

            // Delete translated file
            if (System.IO.File.Exists(translatedDocumentName))
            {
                System.IO.File.Delete(translatedDocumentName);
            }

            return new DocumentLinks
            {
                OriginalDocument = originalFileUrl,
                TranslatedDocument = translatedFileUrl
            };
        }

      
        private  string CopyFileToSharePoint(string fileName)
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

        private  void SaveBinaryDirect(ClientContext ctx, string libraryName, string fileName, Stream memoryStream)
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

        private  bool LibraryExists(ClientContext ctx, Web web, string libraryName)
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

        private  void CreateLibrary(ClientContext ctx, Web web, string libraryName)
        {
            // Create library to the web
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = libraryName;
            creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
            List list = web.Lists.Add(creationInfo);
            ctx.ExecuteQuery();
        }
    }
}
