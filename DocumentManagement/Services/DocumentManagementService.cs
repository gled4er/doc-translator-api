using System;
using System.Configuration;
using System.Linq;
using TranslationAssistant.TranslationServices.Core;
using TranslationAssistant.Business;
using DocumentManagement.Model;
using DocumentManagement.Utils;
using MicrosoftGraph.Services;

namespace DocumentManagement.Services
{
    /// <summary>
    /// Service managing various files locally required for translating and referencing
    /// </summary>
    public class DocumentManagementService : IDocumentManagementService
    {
        private readonly IStorageManagementService _storageManagementService;
        private readonly ISharePointManagementService _sharePointManagementService;
        private readonly ILoggingService _loggingService;


        /// <summary>
        /// Creates instance <see cref="DocumentManagementService"/>
        /// </summary>
        /// <param name="storageManagementService">Instance of <see cref="IStorageManagementService"/></param>
        /// <param name="sharePointManagementService">Instance of <see cref="ISharePointManagementService"/></param>
        /// <param name="loggingService">Instance of <see cref="ILoggingService"/></param>
        public DocumentManagementService(IStorageManagementService storageManagementService, ISharePointManagementService sharePointManagementService, ILoggingService loggingService)
        {
            _storageManagementService = storageManagementService;
            _sharePointManagementService = sharePointManagementService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Translates a document and provides links to the original as well as the translated document
        /// </summary>
        /// <param name="storageContainerName">Name of the storage container</param>
        /// <param name="storageFileName">Name of the storage file (original document for translation)</param>
        /// <param name="originalLanguage">The language of the original file</param>
        /// <param name="translationLanguage">The language for translating the document</param>
        /// <returns></returns>
        public DocumentLinks TranslateFile(string storageContainerName, string storageFileName, string originalLanguage, string translationLanguage)
        {
            var localFileName = storageFileName;

            try
                {
                    TranslationServiceFacade.Initialize(ConfigurationManager.AppSettings["ApiKey"]);

                    DocumentTranslationManager.DoTranslation(localFileName, false, originalLanguage, translationLanguage);
                }
                catch (Exception ex)
                {
                    _loggingService.Error("Error in TranslationServiceFacade.Initialize or  DocumentTranslationManager.DoTranslation", ex);
                    throw;
                }

                string originalFileUrl;
                string translatedFileUrl;
                string translatedDocumentName;
                try
                {
                    var languageCode = TranslationServiceFacade.AvailableLanguages.Where(p => p.Value == translationLanguage).Select(p => p.Key).FirstOrDefault();

                    var extension = Helper.GetExtension(storageFileName);

                    translatedDocumentName = localFileName.Replace($".{extension}", $".{languageCode}.{extension}");

                    // Move original file to SharePoint
                    originalFileUrl = _sharePointManagementService.CopyFileToSharePoint(localFileName);

                    // Move translated file to SharePoint
                    translatedFileUrl = _sharePointManagementService.CopyFileToSharePoint(translatedDocumentName);
                }
                catch (Exception ex)
                {
                    _loggingService.Error("Error in TranslationServiceFacade.AvailableLanguages.Wher or  Helper.GetExtension or _sharePointManagementService.CopyFileToSharePoint", ex);
                    throw;
                }

                try
                {
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
                }
                catch (Exception ex)
                {
                    _loggingService.Error("Error in System.IO.File.Exists or System.IO.File.Delete", ex);
                    throw;
                }

                return new DocumentLinks
                {
                    OriginalDocument = originalFileUrl,
                    TranslatedDocument = translatedFileUrl
                };
        }
    }
}
