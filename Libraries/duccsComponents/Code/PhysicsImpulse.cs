using Sandbox;

namespace Duccsoft;

public class PhysicsImpulse : Component
{
	public enum LinearImpulseMode
	{
		Random,
		Fixed,
		Follow
	}

	[Property] public bool AutoFire { get; set; } = true;
	[Property] public float Delay { get; set; } = 0f;
	[Property] public bool DestroyAfterFire { get; set; } = true;
	[Property] public Rigidbody Rigidbody { get; set; }

	[Property, Group( "Linear" )]
	public LinearImpulseMode LinearMode { get; set; } = LinearImpulseMode.Random;
	[Property, Group( "Linear" ), HideIf( nameof( LinearMode ), LinearImpulseMode.Fixed )]
	public RangedFloat LinearImpulseMagnitude { get; set; } = 2000f;
	[Property, Group( "Linear" ), ShowIf( nameof( LinearMode ), LinearImpulseMode.Fixed )]
	public Vector3 LinearImpulse { get; set; } = Vector3.Up * 2000f;
	[Property, Group( "Linear" ), ShowIf( nameof( LinearMode ), LinearImpulseMode.Follow )]
	public GameObject MoveTowards { get; set; } = null;


	[Property, Group( "Angular" )]
	public bool RandomizeAngularImpulse { get; set; } = true;
	[Property, Group( "Angular" ), ShowIf( nameof( RandomizeAngularImpulse ), true )]
	public RangedFloat AngularImpulseMagnitude { get; set; } = 2000f;
	[Property, Group( "Angular" ), ShowIf( nameof( RandomizeAngularImpulse ), false )]
	public Vector3 AngularImpulse { get; set; }

	private TimeSince _sinceStart = 0f;

	protected override void OnFixedUpdate()
	{
		if ( !AutoFire )
			return;

		if ( _sinceStart < Delay )
			return;

		ApplyImpulse();
	}

	[Button]
	public void ApplyImpulse()
	{
		var body = Rigidbody?.PhysicsBody;
		if ( !body.IsValid() )
			return;

		var linearImpulse = GetLinearImpulse();
		if ( linearImpulse != default )
		{
			body.ApplyImpulse( linearImpulse );
		}
		var angularImpulse = GetAngularImpulse();
		if ( angularImpulse != default )
		{
			body.ApplyAngularImpulse( angularImpulse );
		}
		if ( DestroyAfterFire )
		{
			Destroy();
		}
	}

	private Vector3 GetLinearImpulse()
	{
		if ( LinearMode == LinearImpulseMode.Fixed )
			return LinearImpulse;

		var dir = Vector3.Random;
		if ( LinearMode == LinearImpulseMode.Follow && MoveTowards.IsValid() )
		{
			dir = Vector3.Direction( WorldPosition, MoveTowards.WorldPosition );
		}
		return dir *= LinearImpulseMagnitude.GetValue();
	}

	private Vector3 GetAngularImpulse()
	{
		if ( !RandomizeAngularImpulse )
			return AngularImpulse;

		var dir = Vector3.Random;
		return dir *= AngularImpulseMagnitude.GetValue();
	}
}
