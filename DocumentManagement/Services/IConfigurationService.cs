namespace DocumentManagement.Services
{
    /// <summary>
    /// Configuration Service Interface
    /// </summary>
    public interface IConfigurationService
    {

        /// <summary>
        /// Retrieves value of app setting by its name
        /// </summary>
        /// <param name="settingName">Name of app setting</param>
        /// <returns></returns>
        string GetSettingValue(string settingName);
    }
}
