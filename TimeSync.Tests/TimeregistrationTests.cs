using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.Tests
{
    [TestClass]
    public class TimeregistrationTests
    {
        [TestInitialize]
        public void Init()
        {
            
        }

        [TestMethod]
        public void CalculateDuration()
        {
            //Arrange
            string duration1 = "08:00-08:30";
            string duration2 = "08.00-09.00";
            string duration3 = "08,00-09,30";
            string duration4 = "0800-1000";
            var timereg1 = new Timeregistration();
            var timereg2 = new Timeregistration();
            var timereg3 = new Timeregistration();
            var timereg4 = new Timeregistration();

            //Act
            var result1 = timereg1.AddHours(duration1);
            var result2 = timereg2.AddHours(duration2);
            var result3 = timereg3.AddHours(duration3);
            var result4 = timereg4.AddHours(duration4);

            //Assert
            Assert.IsTrue(result1);
            Assert.AreEqual(timereg1.Hours, 0.5);

            Assert.IsTrue(result2);
            Assert.AreEqual(timereg2.Hours, 1.0);

            Assert.IsTrue(result3);
            Assert.AreEqual(timereg3.Hours, 1.5);

            Assert.IsTrue(result4);
            Assert.AreEqual(timereg4.Hours, 2);
        }
    }
}
