using System;
using System.Threading;

namespace Fractural.Tasks
{
	public partial struct GDTask
	{
		public static GDTask Delay(float secondsDelay, PlayerLoopTiming delayTiming = PlayerLoopTiming.Process, CancellationToken cancellationToken = default)
		{
			TimeSpan delayTimeSpan = TimeSpan.FromSeconds(secondsDelay);
			return Delay(delayTimeSpan, delayTiming, cancellationToken);
		}
	}
}