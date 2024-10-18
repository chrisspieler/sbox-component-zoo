using Sandbox;

namespace Duccsoft;

public class WorldPlaneDebug : Component
{
	[Property] public Gizmo.GridAxis Axis { get; set; }
	[Property] public float Alpha { get; set; } = 0.2f;

	protected override void OnUpdate()
	{
		Gizmo.Draw.Color = Axis switch
		{
			Gizmo.GridAxis.XY	=> Color.Blue.WithAlpha( Alpha ),
			Gizmo.GridAxis.ZX	=> Color.Green.WithAlpha( Alpha ),
			_					=> Color.Red.WithAlpha( Alpha )
		};
		Gizmo.Draw.Grid( Axis, spacing: 8f, opacity: Alpha );
	}
}
