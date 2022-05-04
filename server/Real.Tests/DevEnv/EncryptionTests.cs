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

namespace Real.Tests.DevEnv {

    [TestClass]
    public class DansComputerIsPossessedTests {

        [DataTestMethod]
        [DataRow("", "")]
        public void CheckFileSystem(string pattern) {
            Assert.Fail();
        }

    }
    
}
