using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Duccsoft;

public class Cycler : Component
{
	[Property] public List<GameObject> Options { get; set; } = new();
	[Property] public int StartingIndex 
	{
		get => _startingIndex;
		set
		{
			_startingIndex = value;
			if ( !Game.IsPlaying )
			{
				GoTo( value );
				_startingIndex = CurrentIndex;
			}
		}
	}
	private int _startingIndex = 0;
	[Property, Group( "Input" )] public string ActionNext { get; set; }
	[Property, Group( "Input" )] public string ActionPrevious { get; set; }

	[Property, ReadOnly] private int CurrentIndex { get; set; }

	protected override void OnStart()
	{
		GoTo( StartingIndex );
	}

	protected override void OnUpdate()
	{
		if ( Options is null || !Options.Any() )
			return;

		var nextIndex = CurrentIndex;
		if ( !string.IsNullOrWhiteSpace( ActionNext ) && Input.Pressed( ActionNext ) )
		{
			Next();
		}
		else if ( !string.IsNullOrWhiteSpace( ActionPrevious ) && Input.Pressed( ActionPrevious ) )
		{
			Previous();
		}
	}

	[Button]
	public void Next()
	{
		GoTo( CurrentIndex + 1 );
	}

	[Button]
	public void Previous()
	{
		GoTo( CurrentIndex - 1 );
	}

	public void GoTo( int index )
	{
		if ( Options is null || !Options.Any() )
			return;

		index = index.UnsignedMod( Options.Count );
		CurrentIndex = index;
		for ( int i = 0; i < Options.Count; i++ )
		{
			var option = Options[i];
			if ( !option.IsValid() )
				continue;

			option.Enabled = i == index;
		}
	}
}
