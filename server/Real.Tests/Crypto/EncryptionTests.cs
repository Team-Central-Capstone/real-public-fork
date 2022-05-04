using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Real.Web.Areas.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Real.Tests.Crypto {

    [TestClass]
    public class EncryptionTests {

        internal static IConfigurationRoot Configuration;
        internal static EncryptionController _encryptionController;

        [ClassInitialize]
        public static void _ClassInitialize(TestContext context) {
            var contentRoot = Directory.GetCurrentDirectory();
            if (!File.Exists(Path.Combine(contentRoot, "appsettings.json"))) {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                contentRoot = Path.GetDirectoryName(pathToExe);
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile($"appsettings.json", false)
                .AddJsonFile($"appsettings.Secrets.json", false)
                .AddJsonFile($"appsettings.Development.json", true)
                .AddUserSecrets("23855cc9-cfc1-404b-ad52-26b0d631595d");
            Configuration = builder.Build();

            _encryptionController = new EncryptionController(Configuration);
        }

        [TestInitialize]
        public void TestInitialize() {

        }

        [TestMethod]
        public void GenerateKeys() {
            var algorithm = _encryptionController.GetAlgorithm();

            algorithm.GenerateKey();
            algorithm.GenerateIV();

            var key = algorithm.Key;
            var iv = algorithm.IV;

            Console.WriteLine($"key = {Convert.ToBase64String(key)}");
            Console.WriteLine($"iv  = {Convert.ToBase64String(iv)}");
        } 

        [DataTestMethod]
        public void ControllerEncrypt(string value, string expected) {
            var actual = _encryptionController.EncryptString(value);

            Assert.AreEqual(expected, actual.Value);
        }

        [DataTestMethod]
        public void ControllerDecrypt(string value, string expected) {
            var actual = _encryptionController.Decrypt(value);

            Assert.AreEqual(expected, actual.Value);
        }


        [TestMethod]
        public void Byte_to_Base64_to_Byte() {
            const string expected = "109409728062644297175";

            var bytes = Encoding.ASCII.GetBytes(expected);
            var b64 = Convert.ToBase64String(bytes);

            bytes = Convert.FromBase64String(b64);
            var actual = Encoding.ASCII.GetString(bytes);

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        public async Task ManagedEncrypt(string value, string expected) {
            var actual = String.Empty;
            
            using (var enc = new RijndaelManaged()) {
                enc.Mode = CipherMode.CBC;
                enc.Padding = PaddingMode.PKCS7;
                enc.Key = Convert.FromBase64String(Configuration["encryption:key"]);
                enc.IV = Convert.FromBase64String(Configuration["encryption:iv"]);
                
                using (var encryptor = enc.CreateEncryptor()) {  
                    using (var ms = new MemoryStream()) {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                            using (var writer = new StreamWriter(cs)) {
                                await writer.WriteAsync(value);
                            }
                        }
                        actual = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            Assert.AreEqual(expected, actual);
        }


    }
    
}
