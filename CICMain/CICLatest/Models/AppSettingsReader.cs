using CICLatest.Contracts;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace CICLatest.Models
{
    public class AppSettingsReader : IAppSettingsReader
    {
        private readonly Dictionary<string, string> _settings;

        public AppSettingsReader(IConfiguration configuration)
        { 
            _settings = configuration.GetSection("AppSettings").GetChildren().ToDictionary(x => x.Key, x => x.Value);
        }

        public string Read(string key)
        {
            if(_settings != null && _settings.TryGetValue(key, out string value))
                return value;
            return null;
        }
    }
}
