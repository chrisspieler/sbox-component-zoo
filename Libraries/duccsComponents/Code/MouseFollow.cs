using Sandbox;

namespace Duccsoft;

public class MouseFollow : Component
{
	public enum FollowMode
	{
		Screen,
		WorldPlane
	}

	[Property] public FollowMode Mode { get; set; } = FollowMode.WorldPlane;

	[Property]
	public float Distance { get; set; } = 100f;

	[Property, ShowIf( nameof( Mode ), FollowMode.WorldPlane )]
	public Vector3 PlaneNormal
	{
		get => _planeNormal;
		set
		{
			_planeNormal = value.Normal;
		}
	}
	private Vector3 _planeNormal = Vector3.Up;
	[Property, ShowIf( nameof( Mode ), FollowMode.WorldPlane )]
	public Vector3 PlaneOffset { get; set; } = Vector3.Zero;

	protected override void OnUpdate()
	{
		TickFollow();
	}

	private void TickFollow()
	{
		var camera = Scene?.Camera;
		if ( !camera.IsValid() )
			return;

		WorldPosition = Mode switch
		{
			FollowMode.WorldPlane => GetWorldPlanePosition( camera ),
			_ => GetScreenPosition( camera ),
		};
	}

	private Vector3 GetScreenPosition( CameraComponent camera )
	{
		return camera.ScreenPixelToRay( Mouse.Position ).Project( Distance );
	}

	private Vector3 GetWorldPlanePosition( CameraComponent camera )
	{
		var origin = WorldPosition + PlaneOffset;
		var plane = new Plane( origin, PlaneNormal );
		var mouseRay = camera.ScreenPixelToRay( Mouse.Position );

		if ( !plane.TryTrace( mouseRay, out var hitPos, true, Distance ) )
			return WorldPosition;

		return hitPos;
	}
}
