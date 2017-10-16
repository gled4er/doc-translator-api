using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using MicrosoftGraph.Services;

namespace DocumentManagement.Services
{
    /// <summary>
    /// SharePoint Document Management Service
    /// </summary>
    public class SharePointManagementService : ISharePointManagementService
    {

        //private read only IConfigurationService _configurationService;
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Create an instance of <see cref="ISharePointManagementService"/>
        /// </summary>
        /// <param name="loggingService">Instance of <see cref="ILoggingService"/></param>
        public SharePointManagementService(ILoggingService loggingService)
        {
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
                        using (var clientContext = new ClientContext(ConfigurationManager.AppSettings["SPSiteUrl"]))
                        {
                            var passWord = new SecureString();

                            foreach (var c in ConfigurationManager.AppSettings["SPPassword"].ToCharArray()) passWord.AppendChar(c);

                            clientContext.Credentials = new SharePointOnlineCredentials(ConfigurationManager.AppSettings["SPUserName"], passWord);

                            var web = clientContext.Web;

                            clientContext.Load(web);

                            clientContext.ExecuteQuery();

                            SaveBinaryDirect(clientContext, "Documents", fileName, fileStream);

                            var filePath = string.Format("/{0}/Forms/AllItems.aspx?id=/sites/{1}/{0}/{2}&parent=/sites/{1}/{0}", ConfigurationManager.AppSettings["SPLibraryName"], ConfigurationManager.AppSettings["SPSiteName"], fileName);

                            var documentUrl = string.Format("{0}{1}", ConfigurationManager.AppSettings["SPSiteUrl"], filePath);

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
            var web = ctx.Web;
            //Ensure that target library exists, create if is missing
            if (!LibraryExists(ctx, web, libraryName))
            {
                CreateLibrary(ctx, web, libraryName);
            }

            var docs = ctx.Web.Lists.GetByTitle(libraryName);
            ctx.Load(docs, l => l.RootFolder);
            // Get the information about the folder that will hold the file
            ctx.Load(docs.RootFolder, f => f.ServerRelativeUrl);
            ctx.ExecuteQuery();

            if (fileName.Contains(@"\"))
            {
                fileName = fileName.Split(new[] { @"\" }, StringSplitOptions.None).LastOrDefault();
            }

            Microsoft.SharePoint.Client.File.SaveBinaryDirect(ctx, $"{docs.RootFolder.ServerRelativeUrl}/{fileName}", memoryStream, true);
        }

        private bool LibraryExists(ClientContext ctx, Web web, string libraryName)
        {
            var lists = web.Lists;
            var results = ctx.LoadQuery(lists.Where(list => list.Title == libraryName));
            ctx.ExecuteQuery();
            var existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                return true;
            }

            return false;
        }

        private void CreateLibrary(ClientContext ctx, Web web, string libraryName)
        {
            // Create library to the web
            var creationInfo = new ListCreationInformation
            {
                Title = libraryName,
                TemplateType = (int)ListTemplateType.DocumentLibrary
            };
            web.Lists.Add(creationInfo);
            ctx.ExecuteQuery();
        }
    }
}
