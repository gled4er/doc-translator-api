using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Autofac;
using MicrosoftGraph.Services;
using DocumentManagement.Services;
using TranslatorApp.Model;

namespace TranslatorApp
{
    public static class HelloSequence
    {
        [FunctionName("Translator")]
        public static async Task<string> Run(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {

            var request = context.GetInput<TranslatorRequest>();
            var output = await context.CallFunctionAsync<string>("DocumentTranslator", request);

            return output;
        }

        [FunctionName("DocumentTranslator")]
        public static string DocumentTranslator([ActivityTrigger] TranslatorRequest request)
        {

            var containerBuilder = new ContainerBuilder();

            #region Dependency Injection Setup 

            containerBuilder.Register<ILoggingService>(b => new LoggingService());
            containerBuilder.Register<IStorageManagementService>(b => new StorageManagementService(b.Resolve<ILoggingService>()));
            containerBuilder.Register<ISharePointManagementService>(b => new SharePointManagementService(b.Resolve<ILoggingService>()));
            containerBuilder.Register<IHttpService>(b => new HttpService(b.Resolve<ILoggingService>()));
            containerBuilder.Register<IDocumentManagementService>(b => new DocumentManagementService(b.Resolve<IStorageManagementService>(), b.Resolve<ISharePointManagementService>(), b.Resolve<ILoggingService>()));
            var container = containerBuilder.Build();

            #endregion

            string result;

            using (var scope = container.BeginLifetimeScope())
            {
                var documentManagementService = scope.Resolve<IDocumentManagementService>();

                var documentLinks = documentManagementService.TranslateFile(request.ContainerName, request.FileName, request.OriginalLanguage, request.TranslationLanguage);

                result = $"Original doc - {documentLinks.OriginalDocument}, translated doc - {documentLinks.TranslatedDocument}";

            }

            return result;
        }
    }
}
