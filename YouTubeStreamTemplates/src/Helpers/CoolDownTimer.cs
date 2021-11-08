using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace YouTubeStreamTemplates.Helpers
{
    public class CoolDownTimer
    {
        private readonly int _checkSteps;
        private readonly Stopwatch _stopwatch;
        private CancellationTokenSource? _cancellationTokenSource;

        public CoolDownTimer(int checkSteps = 100)
        {
            _stopwatch = new Stopwatch();
            _checkSteps = checkSteps;
        }

        public bool IsRunning => _stopwatch.IsRunning;

        ~CoolDownTimer()
        {
            _stopwatch.Reset();
            if (_cancellationTokenSource == null) return;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public void StartBlock() { _stopwatch.Start(); }

        public void Start(long runtime = 3000)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            _stopwatch.Start();
            Task.Run(async () => await StopStopWatch(runtime, token), token);
        }

        public void Reset()
        {
            _cancellationTokenSource?.Cancel();
            _stopwatch.Reset();
        }

        public void Restart(long runtime = 3000)
        {
            Reset();
            Start(runtime);
        }

        private async Task StopStopWatch(long runtime, CancellationToken token)
        {
            while (_stopwatch.IsRunning && _stopwatch.ElapsedMilliseconds < runtime && !token.IsCancellationRequested)
                await Task.Delay(_checkSteps, token);

            token.ThrowIfCancellationRequested();
            _stopwatch.Reset();
        }
    }
}