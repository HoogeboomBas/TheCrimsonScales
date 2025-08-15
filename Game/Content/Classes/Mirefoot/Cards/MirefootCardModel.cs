﻿using Fractural.Tasks;
using Godot;

public abstract class MirefootCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : MirefootCardSide, new()
	where TBottom : MirefootCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Mirefoot/Cards.jpg";
	protected override int ColumnCount => 6;
	protected override int RowCount => 5;
}

public abstract class MirefootCardSide : AbilityCardSide
{
	protected async GDTask CreateDifficultTerrain(Hex hex)
	{
		PackedScene scene = ResourceLoader.Load<PackedScene>(
			GameController.Instance.StateRNG.Randf() > 0.5f ? "res://Content/Classes/Mirefoot/Bog1H.tscn" : "res://Content/Classes/Mirefoot/BrokenLog1H.tscn");
		await AbilityCmd.CreateDifficultTerrain(hex, scene);
	}
}