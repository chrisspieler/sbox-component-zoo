using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Duccsoft;

public class Cycler : Component
{
	[Property] public Action<GameObject> OnSwitchFrom { get; set; }
	[Property] public Action<GameObject> OnSwitchTo { get; set; }
	[Property] public Action OnRollOver { get; set; }
	[Property] public Action OnRollUnder { get; set; }

	[Property, MakeDirty] public List<GameObject> Options { get; set; } = new();
	[Property] public bool DisablePreviousOnSwitch { get; set; } = true;
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

	private bool _hasInitialized;

	protected override void OnStart()
	{
		GoTo( StartingIndex );
		_hasInitialized = true;
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

	public GameObject GetOption( int index )
	{
		if ( Options is null || index < 0 || index > Options.Count )
			return null;

		return Options[index];
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

	private void HandleSwitchFromCurrent()
	{
		var previous = GetOption( CurrentIndex );
		if ( previous.IsValid() )
		{
			if ( DisablePreviousOnSwitch )
			{
				previous.Enabled = false;
			}
			OnSwitchFrom?.Invoke( previous );
		}
	}

	public void GoTo( int index )
	{
		if ( Options is null || !Options.Any() )
			return;

		if ( _hasInitialized )
		{
			HandleSwitchFromCurrent();
		}

		var rollOver = index >= Options.Count;
		var rollUnder = index < 0;

		index = index.UnsignedMod( Options.Count );
		CurrentIndex = index;
		var option = Options[index];
		if ( !option.IsValid() )
		{
			GoTo( 0 );
			return;
		}
		option.Enabled = true;
		OnSwitchTo?.Invoke( option );

		if ( rollOver )
		{
			OnRollOver?.Invoke();
		}
		else if ( rollUnder )
		{
			OnRollUnder?.Invoke();
		}
	}

	[Button]
	public void PopulateFromChildren()
	{
		Options ??= new();
		Options.Clear();
		Options.AddRange( GameObject.Children );
		GoTo( CurrentIndex );
	}
}
