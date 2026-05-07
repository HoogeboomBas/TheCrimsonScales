using System;
using System.Threading;

namespace Fractural.Tasks
{
	public partial struct GDTask
	{
		public static GDTask Delay(float secondsDelay, TimeScale timeScale = TimeScale.Other, PlayerLoopTiming delayTiming = PlayerLoopTiming.Process,
			CancellationToken cancellationToken = default)
		{
			TimeSpan delayTimeSpan = TimeSpan.FromSeconds(secondsDelay / AppController.Instance.DeviceOptions.GetTimeScale(timeScale));
			return Delay(delayTimeSpan, delayTiming, cancellationToken);
		}

		public static GDTask DelayFastForwardable(float secondsDelay, TimeScale timeScale = TimeScale.Gameplay,
			PlayerLoopTiming delayTiming = PlayerLoopTiming.Process)
		{
			if(GameController.FastForward)
			{
				return CompletedTask;
			}

			return Delay(secondsDelay, timeScale, delayTiming, cancellationToken: GameController.CancellationToken);
		}
	}
}