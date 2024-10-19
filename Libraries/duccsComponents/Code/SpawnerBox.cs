using Sandbox;
using System;
using System.Collections.Generic;

namespace Duccsoft;

public sealed class SpawnerBox : Component
{
	[Property] public Action<GameObject> OnSpawn { get; set; }

	[Property] public List<GameObject> Prefabs { get; set; } = new();
	[Property] public BBox Bounds { get; set; } = BBox.FromPositionAndSize( 0f, 1000f );
	[Property] public RangedFloat InitialSpawnCount { get; set; } = 5f;
	[Property] public Vector3 RotationRandomness { get; set; } = Vector3.One;
	[Property] public RangedFloat RandomScale { get; set; } = 1f;
	[Property] public GameObject Parent { get; set; }

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Color.Yellow;
		Gizmo.Draw.LineBBox( Bounds );
	}

	protected override void OnStart()
	{
		InitialSpawn();
	}

	private void InitialSpawn()
	{
		var spawnCount = InitialSpawnCount.GetValue();
		if ( spawnCount <= 0 )
			return;

		for ( int i = 0; i < spawnCount; i++ )
		{
			SpawnOne();
		}
	}

	[Button]
	public GameObject SpawnOne()
	{
		var prefab = Random.Shared.FromList( Prefabs );
		if ( !prefab.IsValid() )
			return null;

		var pos = Transform.World.PointToWorld( Bounds.RandomPointInside );
		var rot = Rotation.Identity;
		if ( RotationRandomness != default )
		{
			var randomRotation = Rotation.Random.Angles().AsVector3();
			rot = new Angles( randomRotation * RotationRandomness );
		}
		var spawned = prefab.Clone( Parent, pos, rot, new Vector3( RandomScale.GetValue() ) );
		OnSpawn?.Invoke( spawned );
		return spawned;
	}
}
