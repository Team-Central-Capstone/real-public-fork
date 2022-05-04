using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Real.Tests.Model {

    [TestClass]
    public class UserTests {

        [ClassInitialize]
        public static void _ClassInitialize(TestContext context) {
            
        }

        [TestInitialize]
        public void TestInitialize() {

        }

        [DataTestMethod]
        [DataRow("1983-08-19", Real.Model.ZodiacSign.Leo)]
        [DataRow("1982-06-01", Real.Model.ZodiacSign.Gemini)]
        public void ZodiacTest(string birthdate, Real.Model.ZodiacSign expected) {
            var user = new Real.Model.User {
                Birthdate = DateTime.Parse(birthdate),
            };

            var actual = user.ZodiacSign;

            Assert.AreEqual(expected, actual);
        }
        

    }
    
}
