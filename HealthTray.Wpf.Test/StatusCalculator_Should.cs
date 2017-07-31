using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthTray.Service.Model;

namespace HealthTray.Wpf.Test
{
    [TestClass]
    public class StatusCalculator_Should
    {
        [TestMethod]
        public void Calculate_New_Overall_Status_For_Null_Checks()
        {
            Assert.AreEqual(CheckStatus.@new, StatusCalculator.CalculateOverallStatusFrom(null));
        }

        [TestMethod]
        public void Calculate_New_Overall_Status_For_No_Checks()
        {
            var checks = new List<Check>();
            Assert.AreEqual(CheckStatus.@new, StatusCalculator.CalculateOverallStatusFrom(checks));
        }

        [TestMethod]
        public void Calculate_Up_Overall_Status_For_Single_Up_Check()
        {
            var checks = new List<Check>() { new Check() { status = CheckStatus.up } };
            Assert.AreEqual(CheckStatus.up, StatusCalculator.CalculateOverallStatusFrom(checks));
        }

        [TestMethod]
        public void Calculate_Down_Overall_Status_For_Checks_Of_All_Statuses()
        {
            //test that "down" has the highest priority of any status
            var allStatuses = Enum.GetValues(typeof(CheckStatus)).Cast<CheckStatus>();
            var checks = allStatuses.Select(s => new Check() { status = s }).ToList();
            Assert.AreEqual(CheckStatus.down, StatusCalculator.CalculateOverallStatusFrom(checks));
        }

        [TestMethod]
        public void Calculate_Late_Overall_Status_For_Late_Check()
        {
            var checks = new List<Check>() { new Check() { status = CheckStatus.late }, new Check() { status = CheckStatus.paused }, new Check() { status = CheckStatus.up } };
            Assert.AreEqual(CheckStatus.late, StatusCalculator.CalculateOverallStatusFrom(checks));
        }

        [TestMethod]
        public void Calculate_Paused_Overall_Status_For_Paused_Check()
        {
            var checks = new List<Check>() { new Check() { status = CheckStatus.paused }, new Check() { status = CheckStatus.up } };
            Assert.AreEqual(CheckStatus.paused, StatusCalculator.CalculateOverallStatusFrom(checks));
        }

        [TestMethod]
        public void Calculate_New_Overall_Status_For_New_Checks()
        {
            var checks = new List<Check>() { new Check() { status = CheckStatus.@new }, new Check() { status = CheckStatus.up } };
            Assert.AreEqual(CheckStatus.@new, StatusCalculator.CalculateOverallStatusFrom(checks));
        }
    }
}
