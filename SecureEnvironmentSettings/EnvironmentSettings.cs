using System;
using System.Collections.Specialized;
using System.Configuration;

namespace SecureEnvironmentSettings
{
    public class EnvironmentSettings
    {
        /// <summary>
        /// Environment settings section Name label
        /// </summary>
        private const string EnvironmentSettingSectionName = "EnvironmentSettings";

        /// <summary>
        /// Current environment label
        /// </summary>
        private const string currentEnvironmentKey = "CurrentEnvironment";

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
        public static NameValueCollection Settings
        {
            get
            {
                NameValueCollection CurrentSettings = (NameValueCollection)
                    ConfigurationManager.GetSection(EnvironmentSettingSectionName + "/" + CurrentEnvironent);
                return CurrentSettings;
            }
        }

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
                {
                    section.SectionInformation.UnprotectSection();
                }
                connectionString = section.ConnectionStrings[name].ToString();

                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            }
            catch (Exception e)
            {
                //TODO: Change Message
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

            NameValueCollection config = (NameValueCollection)
                ConfigurationManager.GetSection(EnvironmentSettingSectionName + "/" + Environment);

            ConnectionStringSettings connectionString = (ConnectionStringSettings)
                ConfigurationManager.ConnectionStrings[config[name]];

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

            NameValueCollection config = (NameValueCollection)
                ConfigurationManager.GetSection(EnvironmentSettingSectionName + "/" + Environment);
            return config[Key].ToString();
        }

        #endregion
    }
}
