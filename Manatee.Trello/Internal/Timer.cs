using System;
using System.Threading;
using System.Threading.Tasks;

#if IOS || CORE

namespace Manatee.Trello.Internal
{
	internal class Timer : IDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource;

		private bool _isDisposed;

		public bool AutoReset { get; set; }
		public int Interval { get; set; }
		public bool Enabled { get; set; }

		public event EventHandler<ElapsedEventArgs> Elapsed;

		public Timer()
		{
			Interval = TimeSpan.MaxValue.Milliseconds;
			_cancellationTokenSource = new CancellationTokenSource();
		}
		~Timer()
		{
			Dispose(false);
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start()
		{
			if (Enabled)
				Stop();

			Enabled = true;
#pragma warning disable 4014
			RunTimer(_cancellationTokenSource.Token);
#pragma warning restore 4014
		}

		public void Stop()
		{
			Enabled = false;
			_cancellationTokenSource.Cancel(false);
		}

		private async Task RunTimer(CancellationToken token)
		{
			while (Enabled)
			{
				await Task.Delay(Interval, token);

				if (!Enabled) continue;

				Elapsed?.Invoke(this, new ElapsedEventArgs());

				if (!AutoReset)
					Stop();
			}
		}
		private void Dispose(bool isDisposing)
		{
			if (_isDisposed) return;
			_isDisposed = true;

			Stop();
		}
	}

	internal class ElapsedEventArgs : EventArgs
	{
		
	}
}

#endif