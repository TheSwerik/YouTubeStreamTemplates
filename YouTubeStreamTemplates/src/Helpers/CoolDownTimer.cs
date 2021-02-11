using System.Diagnostics;
using System.Threading.Tasks;

namespace YouTubeStreamTemplates.Helpers
{
    public class CoolDownTimer
    {
        private readonly int _checkSteps;
        private readonly Stopwatch _stopwatch;
        private Task? _stopTask;

        public CoolDownTimer(int checkSteps = 100)
        {
            _stopwatch = new Stopwatch();
            _checkSteps = checkSteps;
        }

        public bool IsRunning => _stopwatch.IsRunning;

        public void StartBlock() { _stopwatch.Start(); }

        public void Start(long runtime = 3000)
        {
            _stopwatch.Start();
            _stopTask = Task.Run(async () => await StopStopWatch(runtime));
        }

        public void Reset()
        {
            _stopTask?.Dispose();
            _stopwatch.Reset();
        }

        public void ReStart(long runtime = 3000)
        {
            Reset();
            Start(runtime);
        }

        private async Task StopStopWatch(long runtime)
        {
            while (_stopwatch.IsRunning && _stopwatch.ElapsedMilliseconds < runtime) await Task.Delay(_checkSteps);
            _stopwatch.Reset();
        }
    }
}