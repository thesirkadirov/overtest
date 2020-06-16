using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage
{
    
    public class ConfigurationStorage
    {

        private readonly OvertestDatabaseContext _databaseContext;
        
        public ConfigurationStorage(OvertestDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        
        public ConfigurationKeyValuePair this[string key] {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException(nameof(key));
                
                if (!_databaseContext.SystemConfigurationStore.Any(c => c.Key == key))
                {
                    // Using null as default
                    Add(key, null);
                }
                
                return _databaseContext.SystemConfigurationStore
                    .AsNoTracking()
                    .First(pair => pair.Key.Equals(key));
            }
        }

        public void Add(string key) => Add(key, null);
        public void Add(string key, string value) => Add<string>(key, value);
        
        public void Add<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(nameof(key));
            
            if (_databaseContext.SystemConfigurationStore.Any(c => c.Key == key))
                throw new Exception();

            _databaseContext.SystemConfigurationStore.Add(new ConfigurationKeyValuePair(key, value != null ? value.ToString() : null));
            _databaseContext.SaveChanges();
        }

        public void Update(string key, string value) => Update<string>(key, value);
        
        public void Update<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(nameof(key));
            
            if (_databaseContext.SystemConfigurationStore.Any(c => c.Key == key))
            {
                
                var selectedPair = _databaseContext.SystemConfigurationStore.First(c => c.Key == key);
                selectedPair.Value = value != null ? value.ToString() : null;
                
                _databaseContext.SystemConfigurationStore.Update(selectedPair);
                _databaseContext.SaveChanges();
                
                return;
                
            }
            
            throw new KeyNotFoundException();
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(nameof(key));
            
            if (_databaseContext.SystemConfigurationStore.Any(c => c.Key == key))
            {
                _databaseContext.SystemConfigurationStore.Remove(
                    _databaseContext.SystemConfigurationStore.First(c => c.Key == key)
                );
                _databaseContext.SaveChanges();
            }
            
            throw new KeyNotFoundException();
        }
        
        public class ConfigurationKeyValuePair
        {
            
            public ConfigurationKeyValuePair() {  }
            
            public ConfigurationKeyValuePair(string key, string value)
            {
                Key = key;
                Value = value;
            }
            
            public string Key { get; set; }
            public string Value { get; set; }
            
            [NotMapped]
            public bool BoolValue
            {
                get => !string.IsNullOrWhiteSpace(Value) && Value != false.ToString();
                set => Value = value.ToString();
            }

        }
        
        public class CommonKeys
        {
            public const string OvertestInstallationFinished = "Overtest:System:Setup:Installed";
        }
        
    }
    
}