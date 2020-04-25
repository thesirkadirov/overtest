namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage
{
    
    public class ConfigurationStorage
    {

        public ConfigurationStorage() {  }

        public ConfigurationStorage(string key, string value)
        {
            Key = key;
            Value = value;
        }
        
        public string Key { get; set; }
        public string Value { get; set; }

        public class CommonKeys
        {
            public const string OvertestInstallationFinished = nameof(OvertestInstallationFinished);
        }
        
    }
    
}