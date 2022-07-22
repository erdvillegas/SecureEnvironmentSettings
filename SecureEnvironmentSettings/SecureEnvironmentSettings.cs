using System;
using System.Configuration;
using System.Linq;

///<summary>
///This library helps you to keep your config files secure because once you call the Encrypt method the config sections and connections strings will be encrypted 
///using a specific value from the machine (or devices) running your code.
///</summary>
namespace SecureEnvironmentSettings
{
    /// <summary>
    /// Library to handle config files based on environment
    /// </summary>
    public static class SecureEnvironmentSettings
    {
        #region Labels

        /// <summary>
        /// Environment settings section Name label.
        /// You can set your config parameters based on your environment and keep it secure, when you need to change all your parameters, 
        /// you can change a configuration environment , CurrentEnvironment
        /// The following example show how you need to setup your main app config file, also how to split on different files
        /// </summary>
        /// <example>
        /// <codelanguage="xml" title="app.config">
        /// <![CDATA[
        /// <?xml version = "1.0" encoding="utf-8" ?>
        /// <configuration>
        /// <configSections>
        ///     <sectionGroup name = "EnvironmentSettings" type="System.Configuration.ApplicationSettingsGroup">
        ///	        <section name = "Production" type="System.Configuration.AppSettingsSection"/>
        ///	        <section name = "Development" type="System.Configuration.AppSettingsSection"/>
        ///	        <section name = "QA" type="System.Configuration.AppSettingsSection"/>
        ///     </sectionGroup>
        /// </configSections>
        /// <connectionStrings>
        ///     <add name = "MovieDBContextPrd"
        ///     connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesPrd.mdf"
        ///     providerName="System.Data.SqlClient">
        ///     <add name = "MovieDBContextDev"
        ///     connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesDev.mdf"
        ///     providerName="System.Data.SqlClient">
        ///    <add name = "MovieDBContextQA"
        ///     connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesQA.mdf"
        ///     providerName="System.Data.SqlClient">
        /// </connectionStrings>
        /// <EnvironmentSettings>
        ///     <Production file = "Config\Production.config" ></ Production >
        ///     <Development file="Config\Development.config"></Development>
        ///     <QA>
        ///	        <add key = "TestKey" value="QAValue"/>
        ///	        <add key = "connection" value="MovieDBContextQA"/>
        ///     </QA>
        /// </EnvironmentSettings>
        /// <appSettings>
        ///     <add key = "CurrentEnvironment" value="Development"/>
        ///     <add key = "CommonKey" value="CommonValue"/>
        /// </appSettings>
        /// </configuration>
        /// ]]>
        /// </code>
        /// </example>
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
        /// Encrypt the connection string and secureSections, you need to run this method for the very first time to ensure your config files will be encrypted
        /// </summary>
        /// <example>
        /// <code language="cs">
        /// SecureEnvironmentSettings.Encrypt();
        /// </code>
        /// </example>
        /// <note type="caution">
        ///     Once the file is encrypted you can not be able to read it again until you decrypt your file again, 
        ///     since the file is encrypted using a specific value from the machine that run the encrypt method, 
        ///     you can not share your config file encrypted
        /// </note>
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
        /// Decrypt the config files
        /// </summary>
        /// <example>
        /// <code language="cs">
        /// SecureEnvironmentSettings.Decrypt();
        /// </code>
        /// </example>
        /// <note type="tip">
        /// You can use this method to decrypt your file, once the files are unencrypted you would be able to share your file betwen machines
        /// </note>
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

        /// <summary>
        /// Check if your current environment sections and connectionString are encrypted
        /// </summary>
        /// <returns>True if any secureSections or connectionStrings are encrypted</returns>
        /// <example>
        /// <code language="cs">
        ///     bool isEncrypted = SecureEnvironmentSettings.IsEncrypted();
        ///     if (isEncrypted)
        ///         Console.WriteLine("The config sections are encrypted");
        /// </code>
        /// </example>
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
        /// Update config values directly on your current environment section
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="environment">Environment to save your configuration value</param>
        public static void UpdateConfiguracion(string key, string value, string environment)
        {
            try
            {
                if (string.IsNullOrEmpty(environment))
                    environment = CurrentEnvironent;

                string section = $"{EnvironmentSectionGroupName}/{environment}";

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection secureConfigSecction = config.GetSection(section) as AppSettingsSection;

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
                ConfigurationManager.RefreshSection(section);
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
        /// Get the current environment value
        /// </summary>
        /// <example language="xml">
        /// <code>
        /// <appSettings>
        ///     <add key="CurrentEnvironment" value="Development" />
        /// </appSettings>
        /// </code>
        /// <code language="cs">
        /// string currentEnv = SecureEnvironmentSettings.CurrentEnvironent;
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
        /// <example>
        /// <code language="cs">
        ///     string expected = "DevelopmentValue";
        ///     string wanted = SecureEnvironmentSettings.Settings["TestKey"].Value;
        /// </code>
        /// </example>
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
        /// Get the connection String from de encrypted file based on your environment settings
        /// </summary>
        /// <param name="name">Connection string Name</param>
        /// <returns>Decrypted connection String</returns>
        /// <example>
        /// <code language="xml" title="app.config">
        /// <![CDATA[
        /// <?xml version = "1.0" encoding="utf-8" ?>
        /// <configuration>
        /// <configSections>
        ///     <sectionGroup name = "EnvironmentSettings" type="System.Configuration.ApplicationSettingsGroup">
        ///	        <section name = "QA" type="System.Configuration.AppSettingsSection"/>
        ///     </sectionGroup>
        /// </configSections>
        /// <connectionStrings>
        ///    <add name = "MovieDBContextQA"
        ///     connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesQA.mdf"
        ///     providerName="System.Data.SqlClient">
        /// </connectionStrings>
        /// <EnvironmentSettings>
        ///     <QA>
        ///	        <add key = "connection" value="MovieDBContextQA"/>
        ///     </QA>
        /// </EnvironmentSettings>
        /// <appSettings>
        ///     <add key = "CurrentEnvironment" value="QA"/>
        /// </appSettings>
        /// </configuration>
        /// ]]>
        /// </code>
        /// <code language="cs">
        /// string connection = SecureEnvironmentSettings.GetConnectionString("MovieDBContextQA");
        /// </code>
        /// </example>
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
        /// <example>
        /// <code language="xml" title="app.config">
        /// <![CDATA[
        /// <?xml version = "1.0" encoding="utf-8" ?>
        /// <configuration>
        /// <configSections>
        ///     <sectionGroup name = "EnvironmentSettings" type="System.Configuration.ApplicationSettingsGroup">
        ///	        <section name = "Development" type="System.Configuration.AppSettingsSection"/>
        ///	        <section name = "QA" type="System.Configuration.AppSettingsSection"/>
        ///     </sectionGroup>
        /// </configSections>
        /// <connectionStrings>
        ///     <add name = "MovieDBContextDev"
        ///     connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesDev.mdf"
        ///     providerName="System.Data.SqlClient">
        ///    <add name = "MovieDBContextQA"
        ///     connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesQA.mdf"
        ///     providerName="System.Data.SqlClient">
        /// </connectionStrings>
        /// <EnvironmentSettings>
        ///     <Development>
        ///	        <add key = "connection" value="MovieDBContextQA"/>
        ///     </Development>
        ///     <QA>
        ///	        <add key = "connection" value="MovieDBContextQA"/>
        ///     </QA>
        /// </EnvironmentSettings>
        /// <appSettings>
        ///     <add key = "CurrentEnvironment" value="QA"/>
        /// </appSettings>
        /// </configuration>
        /// ]]>
        /// </code>
        /// <code language="cs">
        ///     string environment = SecureEnvironmentSettings.GetConnectionStringByEnvironment("connection", "Development").ConnectionString;
        /// </code>
        /// </example>
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
        /// Get a Shared Key environment value from a shared Key
        /// </summary>
        /// <param name="Key">Key to find</param>
        /// <returns>The value from that environment</returns>
        /// <example>
        /// <code language="cs">
        ///     string expected = "CommonValue";
        ///     string wanted = SecureEnvironmentSettings.GetSharedKey("CommonKey");
        /// </code>
        /// </example>
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