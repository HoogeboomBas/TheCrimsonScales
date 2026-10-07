using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class ThornreaperLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ThornreaperCardSide
	where TBottom : ThornreaperCardSide
{
	protected override string TexturePath => "res://Content/Classes/Thornreaper/Cards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 4;
}

public abstract class ThornreaperCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ThornreaperCardSide
	where TBottom : ThornreaperCardSide
{
	protected override string TexturePath => "res://Content/Classes/Thornreaper/Cards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 4;
}

public abstract class ThornreaperCardSide : AbilityCardSideModel<Thornreaper>
{
	protected async GDTask CreateHazardousTerrain(Hex hex)
	{
		PackedScene scene = SceneLoader.LoadPackedScene("res://Content/OverlayTiles/HazardousTerrain/Thorns1H.tscn");
		await AbilityCmd.CreateHazardousTerrain(hex, scene);
	}
}