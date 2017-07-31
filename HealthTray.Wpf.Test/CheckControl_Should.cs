using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthTray.Wpf.Test
{
    [TestClass]
    public class CheckControl_Should
    {
        [TestMethod]
        public void Format_Null_TimeSpans_As_Never()
        {
            Assert.AreEqual("never", CheckControl.FormatForDisplay(null));
        }

        [TestMethod]
        public void Format_Zero_TimeSpan_As_Now()
        {
            Assert.AreEqual("just now", CheckControl.FormatForDisplay(TimeSpan.Zero));
        }

        [TestMethod]
        public void Format_Future_TimeSpan_As_Now()
        {
            var ts = new TimeSpan(hours: -1, minutes: 0, seconds: 0);
            Assert.AreEqual("just now", CheckControl.FormatForDisplay(ts));
        }

        [TestMethod]
        public void Format_TimeSpan_Seconds()
        {
            var ts = new TimeSpan(hours: 0, minutes: 0, seconds: 10);
            Assert.AreEqual("10s ago", CheckControl.FormatForDisplay(ts));
        }

        [TestMethod]
        public void Format_TimeSpan_Seconds_Minutes()
        {
            var ts = new TimeSpan(hours: 0, minutes: 2, seconds: 30);
            Assert.AreEqual("2m 30s ago", CheckControl.FormatForDisplay(ts));
        }

        [TestMethod]
        public void Format_TimeSpan_Seconds_Minutes_Hours()
        {
            var ts = new TimeSpan(hours: 1, minutes: 30, seconds: 0);
            Assert.AreEqual("1h 30m ago", CheckControl.FormatForDisplay(ts));
        }

        [TestMethod]
        public void Format_TimeSpan_Seconds_Minutes_Hours_Days()
        {
            var ts = new TimeSpan(days: 2, hours: 1, minutes: 30, seconds: 5);
            Assert.AreEqual("2d 1h 30m 5s ago", CheckControl.FormatForDisplay(ts));
        }
    }
}
