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
        public void CalculateHoursFromVariousDurationFormats()
        {
            //Arrange
            string duration1 = "08:00-08:30";
            string duration2 = "08.00-09.00";
            string duration3 = "08,00-09,30";
            string duration4 = "0800-1000";
            string duration5 = "2.5";
            string duration6 = "3,0";
            var timereg1 = new Timeregistration();
            var timereg2 = new Timeregistration();
            var timereg3 = new Timeregistration();
            var timereg4 = new Timeregistration();
            var timereg5 = new Timeregistration();
            var timereg6 = new Timeregistration();

            //Act
            var result1 = timereg1.ConvertDurationToHours(duration1);
            var result2 = timereg2.ConvertDurationToHours(duration2);
            var result3 = timereg3.ConvertDurationToHours(duration3);
            var result4 = timereg4.ConvertDurationToHours(duration4);
            var result5 = timereg5.ConvertDurationToHours(duration5);
            var result6 = timereg6.ConvertDurationToHours(duration6);

            //Assert
            Assert.IsTrue(result1);
            Assert.AreEqual(0.5, timereg1.Hours);

            Assert.IsTrue(result2);
            Assert.AreEqual(1.0, timereg2.Hours);

            Assert.IsTrue(result3);
            Assert.AreEqual(1.5, timereg3.Hours);

            Assert.IsTrue(result4);
            Assert.AreEqual(2.0, timereg4.Hours);

            Assert.IsTrue(result5);
            Assert.AreEqual(2.5, timereg5.Hours);

            Assert.IsTrue(result6);
            Assert.AreEqual(3.0, timereg6.Hours);
        }
    }
}
