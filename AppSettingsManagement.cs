using Microsoft.Extensions.Configuration;
using System.IO;

namespace QuartzNetJobFactoryTest
{
    public static class AppSettingsManagement
    {
        private static IConfigurationRoot _config;

        static AppSettingsManagement()
        {
            if (_config == null)
            {
                _config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appSettings.json").Build();
            }
        } 
        public static ImConfiguration ImConfiguration()
        {
            return _config.GetSection("ImConfiguration").Get<ImConfiguration>();
        } 
    }
}