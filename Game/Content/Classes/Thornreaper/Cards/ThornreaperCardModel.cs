using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class ThornreaperLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ThornreaperCardSide
	where TBottom : ThornreaperCardSide
{
	protected override string TexturePath => "res://Content/Classes/Thornreaper/Cards.jpg";
	protected override int ColumnCount => 7;
	protected override int RowCount => 5;
}

public abstract class ThornreaperCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ThornreaperCardSide
	where TBottom : ThornreaperCardSide
{
	protected override string TexturePath => "res://Content/Classes/Thornreaper/Cards.jpg";
	protected override int ColumnCount => 7;
	protected override int RowCount => 5;
}

public abstract class ThornreaperCardSide : AbilityCardSideModel<Thornreaper>
{
	protected async GDTask CreateHazardousTerrain(Hex hex)
	{
		PackedScene scene = SceneLoader.LoadPackedScene("res://Content/OverlayTiles/HazardousTerrain/Thorns1H.tscn");
		await AbilityCmd.CreateHazardousTerrain(hex, scene);
	}

	protected async GDTask CreateHazardousTerrain(Hex hex, Figure creator)
	{
		PackedScene scene = SceneLoader.LoadPackedScene("res://Content/OverlayTiles/HazardousTerrain/Thorns1H.tscn");
		hex.Creator = creator;
		await AbilityCmd.CreateHazardousTerrain(hex, scene);
	}

	protected AbilityCardAbility ConsumeEarthOrSkipAction()
	{
		if (Thornreaper.GetIgnoreEarthConsume())
		{
			return new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Thornreaper.SetIgnoreEarthConsume(false);
					state.SetPerformed();
				})
				.Build());
		}
		else
		{
			return new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ConfirmPrompt.Answer answer = null;
					if (GetEarthElementState() == ElementState.Inert)
					{
						answer = await PromptManager.Prompt(
						new ConfirmPrompt(null, () => "Earth is inert. Skip ability?", true),
						state.Authority);
					}
					else {
						state.SetPerformed();
						await AbilityCmd.TryConsumeElement(Element.Earth);
					}
				})
				.Build());
		}
	}

	protected ElementState GetEarthElementState()
	{
		return GameController.Instance.ElementManager.GetState(Element.Earth);
	}

	protected ElementState GetLightElementState()
	{
		return GameController.Instance.ElementManager.GetState(Element.Light);
	}
}