using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Collections;

namespace SecureEnvironmentSettings
{
    public static class SecureEnvironmentSettings
    {
        #region Labels

        /// <summary>
        /// Environment settings section Name label
        /// </summary>
        private const string EnvironmentSectionGroupName = "EnvironmentSettings";

        /// <summary>
        /// Current environment label
        /// </summary>
        private const string currentEnvironmentKey = "CurrentEnvironment";

        /// <summary>
        /// Provider to encrypt config sections
        /// </summary>
        private const string protectionProvider = "DataProtectionConfigurationProvider";

        #endregion

        #region EncryptDecrypt

        /// <summary>
        /// Encrypt the connection string and secureSections
        /// </summary>
        /// <example language="xml">
        /// <code>
        /// <configSections>
        ///     <section name = "secureConfig" type="System.Configuration.AppSettingsSection"/>
        /// </configSections>
        /// 
        /// <Production>
        ///     <add key="url" value="https://femsa.csod.com"/>
        /// </Production>
        /// 
        /// </code>
        /// </example>   
        public static void Encrypt()
        {
            try
            {
                // Get the application configuration file.
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //Encrypt connectionStrings
                ConnectionStringsSection sectionConnectionStrings = config.GetSection("connectionStrings") as ConnectionStringsSection;
                if (!sectionConnectionStrings.SectionInformation.IsProtected)
                    sectionConnectionStrings.SectionInformation.ProtectSection(protectionProvider);

                //Get every section from EnvironmentSettings and protect
                ConfigurationSectionGroup group = config.SectionGroups[EnvironmentSectionGroupName];
                foreach (ConfigurationSection section in group.Sections)
                {
                    if (!section.SectionInformation.IsProtected)
                    {
                        section.SectionInformation.ProtectSection(protectionProvider);
                    }
                }

                config.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An errror ocurring during encrypting section \n{e.Message}");
            }
        }

        /// <summary>
        /// Decrypt the config file
        /// </summary>
        public static void Decrypt()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //Encrypt connectionStrings
                ConnectionStringsSection sectionConnectionStrings = config.GetSection("connectionStrings") as ConnectionStringsSection;
                if (sectionConnectionStrings.SectionInformation.IsProtected)
                    sectionConnectionStrings.SectionInformation.UnprotectSection();

                //Get every section from EnvironmentSettings and unprotect
                ConfigurationSectionGroup group = config.SectionGroups[EnvironmentSectionGroupName];
                foreach (ConfigurationSection section in group.Sections)
                {
                    if (section.SectionInformation.IsProtected)
                    {
                        section.SectionInformation.UnprotectSection();
                    }
                }

                config.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An errro ocurre during decrypt section \n{e.Message}");
            }
        }

        public static bool IsEncrypted()
        {

            // Get the application configuration file.
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSectionGroup group = config.SectionGroups[EnvironmentSectionGroupName];

            //Get every section from EnvironmentSettings and protect

            bool[] isEncrypted = new bool[group.Sections.Count + 1];

            int i = 0;
            foreach (ConfigurationSection section in group.Sections)
            {
                isEncrypted[i] = section.SectionInformation.IsProtected;
                i++;
            }

            //Encrypt connectionStrings
            ConnectionStringsSection sectionConnectionStrings = config.GetSection("connectionStrings") as ConnectionStringsSection;
            isEncrypted[i] = sectionConnectionStrings.SectionInformation.IsProtected;

            return isEncrypted.All(e => e == true);
        }

        /// <summary>
        /// Update config values
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public static void UpdateConfiguracion(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection secureConfigSecction = config.GetSection(CurrentEnvironent) as AppSettingsSection;

                if (secureConfigSecction.SectionInformation.IsProtected)
                {
                    secureConfigSecction.SectionInformation.UnprotectSection();
                }

                KeyValueConfigurationCollection _settings = secureConfigSecction.Settings;
                if (_settings[key] != null)
                {
                    _settings[key].Value = value;
                }
                else
                {
                    _settings.Add(key, value);
                }

                config.Save(ConfigurationSaveMode.Full, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            finally
            {
                Encrypt();
            }
        }

        #endregion

        #region EnvironmentVariables

        /// <summary>
        /// current working environemtn
        /// </summary>
        /// <example language="xml">
        /// <code>
        /// <appSettings>
        ///     <add key="CurrentEnvironment" value="Development" />
        /// </appSettings>
        /// </code>
        /// </example>   
        public static string CurrentEnvironent
        {
            get
            {
                return ConfigurationManager.AppSettings[currentEnvironmentKey].ToString();
            }
        }

        /// <summary>
        /// Get access to all environmentSettings according to <seealso cref="CurrentEnvironent"/>
        /// </summary>
        public static KeyValueConfigurationCollection Settings
        {
            get
            {
                KeyValueConfigurationCollection _settings = null;
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    AppSettingsSection secureConfigSecction = config.GetSection(EnvironmentSectionGroupName + "/" + CurrentEnvironent) as AppSettingsSection;
                    if (secureConfigSecction.SectionInformation.IsProtected)
                        secureConfigSecction.SectionInformation.UnprotectSection();

                    _settings = secureConfigSecction.Settings;
                    if (!secureConfigSecction.SectionInformation.IsProtected)
                        secureConfigSecction.SectionInformation.ProtectSection(protectionProvider);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return _settings;
            }
        }

        #endregion

        #region ConnectionStrings

        /// <summary>
        /// Get the connection String from de encrypted file
        /// </summary>
        /// <param name="name">Connection string Name</param>
        /// <returns>Decrypted connection String</returns>
        public static string GetConnectionString(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            string connectionString = string.Empty;
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;

                if (section.SectionInformation.IsProtected)
                    section.SectionInformation.UnprotectSection();

                connectionString = section.ConnectionStrings[name].ToString();

                if (!section.SectionInformation.IsProtected)
                    section.SectionInformation.ProtectSection(protectionProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return connectionString;
        }

        /// <summary>
        /// Get the connection String from de encrypted file by the current environment
        /// </summary>
        /// <param name="name">Name of the connection string</param>
        /// <param name="Environment">Specific Environment</param>
        /// <returns>Connection String</returns>
        public static ConnectionStringSettings GetConnectionStringByEnvironment(string name, string Environment)
        {
            if (string.IsNullOrEmpty(Environment))
                Environment = CurrentEnvironent;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection secureConfigSecction = config.GetSection(EnvironmentSectionGroupName + "/" + Environment) as AppSettingsSection;

            if (secureConfigSecction.SectionInformation.IsProtected)
                secureConfigSecction.SectionInformation.UnprotectSection();

                ConnectionStringSettings connectionString = (ConnectionStringSettings)ConfigurationManager.ConnectionStrings[secureConfigSecction.Settings[name].Value];

            return connectionString;
        }

        #endregion

        #region Keys

        /// <summary>
        /// Get a Shared Key environment value
        /// </summary>
        /// <param name="Key">Key to find</param>
        /// <returns>The value from that environment</returns>
        public static string GetSharedKey(string Key)
        {
            return ConfigurationManager.AppSettings[Key].ToString();
        }

        /// <summary>
        /// Get a Shared Key environment value by an specific environment
        /// </summary>
        /// <param name="Key">Key to find</param>
        /// <param name="Environment">Environment to find</param>
        /// <returns>Value of specific environment</returns>
        public static string GetKeyByEnvironment(string Key, string Environment)
        {
            if (string.IsNullOrEmpty(Environment))
                Environment = CurrentEnvironent;

            KeyValueConfigurationCollection _settings = null;
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                AppSettingsSection secureConfigSecction = config.GetSection(EnvironmentSectionGroupName + "/" + Environment) as AppSettingsSection;
                if (secureConfigSecction.SectionInformation.IsProtected)
                    secureConfigSecction.SectionInformation.UnprotectSection();

                _settings = secureConfigSecction.Settings;
                if (!secureConfigSecction.SectionInformation.IsProtected)
                    secureConfigSecction.SectionInformation.ProtectSection(protectionProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine($"There was an issue while getting the value: {e.Message}");
            }

            return _settings[Key].Value;
        }

        #endregion
    }
}
