using Sandbox;

namespace Duccsoft;

public class Mover : Component
{
	public enum MoveType
	{
		/// <summary>
		/// This object is translated by Velocity in world space.
		/// </summary>
		World,
		/// <summary>
		/// This object is translated by Velocity in local space.
		/// </summary>
		Local,
		/// <summary>
		/// This object is translated by Velocity relative to the direction to another object.
		/// </summary>
		Relative
	}

	[Property] public MoveType Mode { get; set; }
	[Property] public Vector3 Velocity { get; set; }
	[Property] public Rigidbody Rigidbody { get; set; }

	[Property, Group( "Follow" ), ShowIf( nameof( Mode ), MoveType.Relative )]
	public GameObject Target { get; set; }

    protected override void OnFixedUpdate()
    {
		var movement = Mode switch
		{
			MoveType.Local		=> GetLocalMoveVector(),
			MoveType.Relative	=> GetRelativeMoveVector(),
			_					=> GetWorldMoveVector()
		};

		if ( movement.IsNearZeroLength )
			return;

		if ( Rigidbody.IsValid() )
		{
			Rigidbody.SmoothMove( WorldPosition + movement, 1f, Time.Delta );
		}
		else
		{
			WorldPosition += movement * Time.Delta;
		}
    }

	private Vector3 GetWorldMoveVector() => Velocity;

	private Vector3 GetLocalMoveVector()
	{
		var targetPosition = Transform.World.PointToWorld( Velocity );
		return Velocity.Length * Vector3.Direction( WorldPosition, targetPosition );
	}
	
	private Vector3 GetRelativeMoveVector()
	{
		if ( !Target.IsValid() )
			return Vector3.Zero;

		var length = Velocity.Length;
		var offsetDir = Rotation.LookAt( Velocity.Normal );
		var targetDir = Vector3.Direction( WorldPosition, Target.WorldPosition );
		return length * targetDir * offsetDir;
	}
}
