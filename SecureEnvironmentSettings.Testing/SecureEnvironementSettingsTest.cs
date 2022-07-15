using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SecureEnvironmentSettings.Testing
{
    [TestClass]
    public class TestSecureEnvironmentSettings
    {
        [TestInitialize]
        public void setUp()
        {
            SecureEnvironmentSettings.Encrypt();
        }

        [TestCleanup]
        public void clean()
        {
            SecureEnvironmentSettings.Decrypt();
        }

        [TestMethod]
        [TestCategory("EncryptDecrypt")]
        public void TestEncrypt()
        {
            SecureEnvironmentSettings.Encrypt();

            Assert.IsTrue(SecureEnvironmentSettings.IsEncrypted());
        }

        [TestMethod]
        [TestCategory("EncryptDecrypt")]
        public void TestDecrypt()
        {
            SecureEnvironmentSettings.Decrypt();

            Assert.IsFalse(SecureEnvironmentSettings.IsEncrypted());
        }

        [TestMethod]
        [TestCategory("EnvironmentSettings")]
        public void TestGetCurrentEnvironment()
        {
            string expected = "Development";
            string wanted = SecureEnvironmentSettings.CurrentEnvironent;
            Assert.IsTrue(expected.Equals(wanted, StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("EnvironmentSettings")]
        public void TestGetCurrentEnvironmentFail()
        {
            string expected = "QA";
            string wanted = SecureEnvironmentSettings.CurrentEnvironent;
            Assert.IsFalse(expected.Equals(wanted, StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("EnvironmentSettings")]
        public void TestGetEnvironmentValue()
        {
            string expected = "DevelopmentValue";
            string wanted = SecureEnvironmentSettings.Settings["TestKey"].Value;

            Assert.IsTrue(expected.Equals(wanted, StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ConnectionStrings")]
        public void TestGetConnectionStrings()
        {
            string expected = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesDev.mdf";
            string wanted = SecureEnvironmentSettings.GetConnectionString("MovieDBContextDev");

            Assert.IsTrue(expected.Equals(wanted, StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ConnectionStrings")]
        public void TestGetConnectionStringByEnvironment()
        {
            string expected = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\MoviesDev.mdf";
            string wanted = SecureEnvironmentSettings.GetConnectionStringByEnvironment("connection", "Development").ConnectionString;

            Assert.IsTrue(expected.Equals(wanted,StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        [TestCategory("GeneralSettings")]
        public void TestGetSharedKey()
        {
            string expected = "CommonValue";
            string wanted = SecureEnvironmentSettings.GetSharedKey("CommonKey");

            Assert.IsTrue(expected.Equals(wanted, StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        [TestCategory("GeneralSettings")]
        public void TestGetKeyByEnvironment()
        {
            string expected = "ProductionValue";
            string wanted = SecureEnvironmentSettings.GetKeyByEnvironment("TestKey", "Production");

            Assert.IsTrue(expected.Equals(wanted, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
