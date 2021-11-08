using System.Threading;
using NUnit.Framework;
using YouTubeStreamTemplates.Helpers;

namespace YouTubeStreamTemplates.Test.Helpers
{
    public class CoolDownTimerTest
    {
        [Test]
        public void TestRunning()
        {
            var timer = new CoolDownTimer();
            Assert.False(timer.IsRunning, "StopWatch default value is wrong");

            timer.StartBlock();
            Assert.True(timer.IsRunning, "StopWatch is not starting");

            timer.Reset();
            Assert.False(timer.IsRunning, "StopWatch is not stopping");
        }

        [Test]
        public void TestRunningAsync()
        {
            var timer = new CoolDownTimer();
            Assert.False(timer.IsRunning, "StopWatch default value is wrong");

            timer.Start(1000);
            Assert.True(timer.IsRunning, "StopWatch is not starting");

            Thread.Sleep(2000);
            Assert.False(timer.IsRunning, "StopWatch is not automatically stopping");
        }

        [Test]
        public void TestRestart()
        {
            var timer = new CoolDownTimer();
            Assert.False(timer.IsRunning, "StopWatch default value is wrong");

            timer.Start(1000);
            Assert.True(timer.IsRunning, "StopWatch is not starting");

            timer.Restart();
            Assert.True(timer.IsRunning, "StopWatch is not restarting");

            Thread.Sleep(2000);
            Assert.True(timer.IsRunning, "StopWatch is not restarting with a new runtime");

            Thread.Sleep(2000);
            Assert.False(timer.IsRunning, "StopWatch is not automatically stopping");
        }
    }
}