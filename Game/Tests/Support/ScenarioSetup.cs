using System;
using System.Collections.Generic;
using Godot;

namespace CrimsonScales.Tests;

/// <summary>A scenario-owned reference resolved against the active gameplay session.</summary>
public abstract class UnitHandle
{
	internal ScenarioSetup Setup { get; }
	internal string Name { get; }

	internal UnitHandle(ScenarioSetup setup, string name)
	{
		Setup = setup;
		Name = name;
	}

	internal Figure Resolve() => (Setup.ActiveGame
		?? throw new InvalidOperationException("Start the scenario before accessing its units; disposed units are unavailable."))
		.Resolve(this);
}

public sealed class UnitHandle<T> : UnitHandle where T : Figure
{
	internal UnitHandle(ScenarioSetup setup, string name) : base(setup, name) { }
	public T Unit => (T)Resolve();
}

public sealed class ScenarioSetup
{
	internal TestGame ActiveGame { get; set; }
	internal sealed record CharacterSpec(string Name, ClassModel Model, Vector2I Position)
	{
		public List<AbilityCardModel> Cards { get; } = [];
	}
	internal List<CharacterSpec> Characters { get; } = [];
	public static ScenarioSetup InTestRoom() => new();

	public ScenarioSetup WithCharacter<TClass>(string name, Vector2I at) where TClass : ClassModel
	{
		ValidateFigure(name, at);
		Characters.Add(new CharacterSpec(name, ModelDB.Class<TClass>(), at));
		return this;
	}

	public ScenarioSetup WithCharacter<TClass>(out UnitHandle<Character> character, Vector2I at) where TClass : ClassModel
	{
		string name = $"character {Characters.Count + 1}";
		WithCharacter<TClass>(name, at);
		character = new UnitHandle<Character>(this, name);
		return this;
	}

	public ScenarioSetup WithCard<TCard>(UnitHandle<Character> character) where TCard : AbilityCardModel
	{
		ValidateHandle(character);
		return WithCard<TCard>(character.Name);
	}

	public ScenarioSetup WithCard<TCard>(string character) where TCard : AbilityCardModel
	{
		CharacterSpec spec = Characters.Find(spec => spec.Name == character)
			?? throw new ArgumentException($"Declare character '{character}' before adding its cards.");
		spec.Cards.Add(ModelDB.AbilityCard<TCard>());
		return this;
	}

	private void ValidateHandle(UnitHandle handle)
	{
		if(handle == null || !ReferenceEquals(handle.Setup, this))
		{
			throw new ArgumentException("The unit handle belongs to a different scenario.", nameof(handle));
		}
	}

	private void ValidateFigure(string name, Vector2I at)
	{
		ValidatePosition(at);
		if(string.IsNullOrWhiteSpace(name) || Characters.Exists(spec => spec.Name == name))
		{
			throw new ArgumentException($"Figure name '{name}' must be nonempty and unique.");
		}
		if(Characters.Exists(spec => spec.Position == at))
		{
			throw new ArgumentException($"Hex {at} is already occupied by a figure.");
		}
	}

	private static void ValidatePosition(Vector2I at)
	{
		if(at.X < 0 || at.X >= 10 || at.Y < 0 || at.Y >= 10)
		{
			throw new ArgumentOutOfRangeException(nameof(at), "Test-room coordinates range from (0,0) to (9,9).");
		}
	}
}
