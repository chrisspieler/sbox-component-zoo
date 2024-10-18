using Sandbox;

namespace Duccsoft;

[Title("Timer")]
public class TimerComponent : Component
{
	public delegate void TimerEvent( TimerComponent timer );

	[Property] public TimerEvent OnFire { get; set; }
	[Property] public TimerEvent OnReset { get; set; }

	[Property] public float IntervalSeconds { get; set; } = 1f;
	[Property] public bool AutoRepeat { get; set; }
	[Property] public bool UseRealTime { get; set; }
	[Property, ReadOnly, Group("State")] public bool HasFired { get; private set; }
	[Property, Group("State")] public float ElapsedSeconds => _sinceStart;
	[Property, Group( "State" ), ReadOnly] public float? LastFireSeconds { get; private set; }
	[Property, Group("State")] public float RealElapsedSeconds => _realTimeSinceStart;
	[Property, Group( "State" ), ReadOnly] public float? LastFireRealSeconds { get; private set; }

	private TimeSince _sinceStart;
	private RealTimeSince _realTimeSinceStart;

	protected override void OnStart()
	{
		_sinceStart = 0f;
		_realTimeSinceStart = 0f;
	}

	protected override void OnUpdate()
	{
		if ( HasFired )
			return;

		var shouldFire = UseRealTime
			? RealElapsedSeconds >= IntervalSeconds
			: ElapsedSeconds >= IntervalSeconds;

		if ( shouldFire )
		{
			Fire();
		}
	}

	[Button]
	public void Fire()
	{
		if ( HasFired )
			return;

		LastFireSeconds = ElapsedSeconds;
		LastFireRealSeconds = RealElapsedSeconds;
		HasFired = true;
		OnFire?.Invoke( this );

		if ( AutoRepeat )
		{
			ResetTime();
		}
	}

	[Button]
	public void ResetTime()
	{
		HasFired = false;
		_sinceStart = 0f;
		_realTimeSinceStart = 0f;
		OnReset?.Invoke( this );
	}
}
