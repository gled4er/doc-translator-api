using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using MicrosoftGraph.Services;

namespace TestApp.DocumentManagement.Services
{
    /// <summary>
    /// SharePoint Document Management Service
    /// </summary>
    public class SharePointManagementService : ISharePointManagementService
    {

        private readonly IConfigurationService _configurationService;
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Create an instance of <see cref="ISharePointManagementService"/>
        /// </summary>
        /// <param name="configurationService">Instance of <see cref="IConfigurationService"/></param>
        /// <param name="loggingService">Instance of <see cref="ILoggingService"/></param>
        public SharePointManagementService(IConfigurationService configurationService, ILoggingService loggingService)
        {
            _configurationService = configurationService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Copies file to Share Point and provides a link to the newly created file
        /// </summary>
        /// <param name="fileName">Name of the file to be copied in SharePoint</param>
        /// <returns>Link to file in SharePoint</returns>
        public string CopyFileToSharePoint(string fileName)
        {
            try
            {


                if (System.IO.File.Exists(fileName))
                {
                    using (var fileStream = System.IO.File.Open(fileName, FileMode.Open))
                    {
                        using (var clientContext = new ClientContext(_configurationService.GetSettingValue("SPSiteUrl")))
                        {
                            var passWord = new SecureString();

                            foreach (char c in _configurationService.GetSettingValue("SPPassword").ToCharArray()) passWord.AppendChar(c);

                            clientContext.Credentials = new SharePointOnlineCredentials(_configurationService.GetSettingValue("SPUserName"), passWord);

                            Web web = clientContext.Web;

                            clientContext.Load(web);

                            clientContext.ExecuteQuery();

                            SaveBinaryDirect(clientContext, "Documents", fileName, fileStream);

                            var filePath = string.Format("/{0}/Forms/AllItems.aspx?id=/sites/{1}/{0}/{2}&parent=/sites/{1}/{0}", _configurationService.GetSettingValue("SPLibraryName"), _configurationService.GetSettingValue("SPSiteName"), fileName);

                            var documentUrl = string.Format("{0}{1}", _configurationService.GetSettingValue("SPSiteUrl"), filePath);

                            return documentUrl;
                        }
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _loggingService.Error("Error in SharePointManagementService.CopyFileToSharePoint", ex);
                throw ex;
            }
        }

        private void SaveBinaryDirect(ClientContext ctx, string libraryName, string fileName, Stream memoryStream)
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

        private bool LibraryExists(ClientContext ctx, Web web, string libraryName)
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

        private void CreateLibrary(ClientContext ctx, Web web, string libraryName)
        {
            // Create library to the web
            ListCreationInformation creationInfo = new ListCreationInformation
            {
                Title = libraryName,
                TemplateType = (int)ListTemplateType.DocumentLibrary
            };
            List list = web.Lists.Add(creationInfo);
            ctx.ExecuteQuery();
        }
    }
}
