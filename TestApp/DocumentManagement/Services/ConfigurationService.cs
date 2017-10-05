using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MicrosoftGraph.Services;

namespace TestApp.DocumentManagement.Services
{
    /// <summary>
    /// Configuration Service providing access to app settings 
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Create instane of <see cref="ConfigurationService"/>  
        /// </summary>
        /// <param name="loggingService">Logging service <see cref="ILoggingService"/></param>
        public ConfigurationService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// Retrieves value of app setting by its name
        /// </summary>
        /// <param name="settingName">Name of app setting</param>
        /// <returns></returns>
        public string GetSettingValue(string settingName)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[settingName];
                return value;
            }
            catch(Exception ex)
            {
                _loggingService.Error("Error in ConfigurationService.GetSettingValue", ex);
                throw ex;
            }
        }
    }
}
