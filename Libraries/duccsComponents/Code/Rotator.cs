using Sandbox;

namespace Duccsoft;

public class Rotator : Component
{
	[Property] public CoordinateSpace CoordinateSpace { get; set; } = CoordinateSpace.Local;
	[Property] public Angles RotationPerSecond { get; set; }
	[Property] public Rigidbody Rigidbody { get; set; }

	protected override void OnFixedUpdate()
	{
		if ( Rigidbody.IsValid() )
		{
			ApplyRigidbodyRotation();
		}
		else
		{
			ApplyRotation();
		}
	}

	private void ApplyRigidbodyRotation()
	{
		if ( !Rigidbody.IsValid() || !Rigidbody.IsValid() )
			return;

		var deltaRotation = RotationPerSecond * Time.Delta;
		Rigidbody.SmoothRotate( LocalRotation * deltaRotation, Time.Delta, Time.Delta );
	}

	private void ApplyRotation()
	{
		var deltaRotation = RotationPerSecond * Time.Delta;
		switch ( CoordinateSpace )
		{
			case CoordinateSpace.Local:
				LocalRotation *= deltaRotation;
				break;
			default:
				WorldRotation *= deltaRotation;
				break;
		}
	}
}
