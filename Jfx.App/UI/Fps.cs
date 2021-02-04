using System;
using System.Diagnostics;

namespace Jfx.App.UI
{
    internal class Fps : IDisposable
    {
        private int frameCount;
        private TimeSpan elapsed;
        private TimeSpan updateRate;
        private Stopwatch stopwatchUpdate;
        private Stopwatch stopwatchFrame;

        public double FpsRender { get; private set; }
        public double FpsGlobal { get; private set; }

        public Fps(TimeSpan updateRate)
        {
            this.updateRate = updateRate;
            stopwatchUpdate = new Stopwatch();
            stopwatchFrame = new Stopwatch();

            stopwatchUpdate.Start();
        }

        public void Dispose()
        {
            stopwatchUpdate?.Stop();
            stopwatchUpdate = default;

            stopwatchFrame?.Stop();
            stopwatchFrame = default;
        }

        public void StartFrame()
        {
            stopwatchFrame.Restart();
        }

        public void EndFrame()
        {
            stopwatchFrame.Stop();
            elapsed += stopwatchFrame.Elapsed;
            frameCount++;

            var updateElapsed = stopwatchUpdate.Elapsed;
            if (updateElapsed >= updateRate)
            {
                FpsRender = frameCount / elapsed.TotalSeconds;
                FpsGlobal = frameCount / updateElapsed.TotalSeconds;

                stopwatchUpdate.Restart();
                elapsed = TimeSpan.Zero;
                frameCount = 0;
            }
        }

        public override string ToString() => $"FPS = {FpsRender:0} ({FpsGlobal:0})";
    }
}
