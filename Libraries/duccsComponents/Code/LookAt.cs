using Sandbox;

namespace Duccsoft;

public class LookAt : Component
{
	[Property]
	public GameObject Target 
	{
		get => _target;
		set
		{
			_target = value;
			if ( !TargetIsMainCamera )
			{
				_targetMainCamera = false;
			}
		}
	}
	private GameObject _target;
	[Property]
	public bool TargetMainCamera 
	{
		get => _targetMainCamera;
		set
		{
			_targetMainCamera = value;
			if ( value )
			{
				Target = Scene?.Camera?.GameObject;
			}
			else if ( Target == Scene?.Camera?.GameObject )
			{
				Target = null;
			}
		}
	}
	private bool _targetMainCamera;

	[Property] public Angles RotationOffset { get; set; }

	private bool TargetIsMainCamera => Target == Scene?.Camera?.GameObject;

	protected override void OnFixedUpdate()
	{
		if ( TargetMainCamera && !TargetIsMainCamera )
		{
			Target = Scene?.Camera?.GameObject;
		}

		if ( !Target.IsValid() )
			return;

		var direction = Vector3.Direction( WorldPosition, Target.WorldPosition );
		WorldRotation = Rotation.LookAt( direction ) * RotationOffset;
	}
}
