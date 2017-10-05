using TestApp.Authentication;
using TestApp.DocumentManagement.Services;
using Autofac;
using MicrosoftGraph.Services;

namespace TestApp
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {

            var containerBuilder = new ContainerBuilder();

            #region Dependency Injection Setup 

            containerBuilder.Register<ILoggingService>(b => new LoggingService());
            containerBuilder.Register<IConfigurationService>(b => new ConfigurationService(b.Resolve<ILoggingService>()));
            containerBuilder.Register<IStorageManagementService>(b=>new StorageManagementService(b.Resolve<IConfigurationService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IHttpService>(b => new HttpService(b.Resolve<ILoggingService>()));
            containerBuilder.Register<IDocumentManagementService>(b=> new DocumentManagementService(b.Resolve<IStorageManagementService>(), b.Resolve<IConfigurationService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IOutlookService>(b => new OutlookService(b.Resolve<IHttpService>(), b.Resolve<ITokenService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<ITokenService>(b => new TokenService(b.Resolve<IHttpService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IRoomService>(b => new RoomService(b.Resolve<IOutlookService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IGroupService>(b => new GroupService(b.Resolve<IHttpService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IMeetingService>(b => new MeetingService(b.Resolve<IHttpService>(), b.Resolve<IOutlookService>(), b.Resolve<IRoomService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IPeopleService>(b => new PeopleService(b.Resolve<IHttpService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IEmailService>(b => new EmailService(b.Resolve<IGroupService>(), b.Resolve<IPeopleService>(), b.Resolve<ILoggingService>()));
            Container = containerBuilder.Build();

            #endregion

            using (var scope = Container.BeginLifetimeScope())
            {
                var configurationService = scope.Resolve<IConfigurationService>();

                // Authenticate 
                var userAccessToken =  AuthenticationHelper.GetTokenForUser(configurationService.GetSettingValue("AADTenant"), configurationService.GetSettingValue("AADAppClientID")).Result;

                // Find emails by name
                var emailService = scope.Resolve<IEmailService>();

                // using distributions list
                // var emails = emailService.GetEmails("Naomi Sato,jbrown@smdocs.onmicrosoft.com,dl-leaders@smdocs.onmicrosoft.com", userAccessToken).Result;
                var emails = emailService.GetEmails("Naomi Sato,jbrown@smdocs.onmicrosoft.com", userAccessToken).Result;

                // Provide Meeting Slots options by date

                // Select meeting slot and room

                // Get document links
                var documentManagementService = scope.Resolve<IDocumentManagementService>();
                var documentLinks = documentManagementService.TranslateFile("documents", "AI05.pptx", "Japanese", "English").Result;

                // Schedule meeting 
            }
        }
    }
}
