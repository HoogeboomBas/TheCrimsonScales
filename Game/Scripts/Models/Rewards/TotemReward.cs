using System;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public abstract class TotemReward : SavedReward, IEventSubscriber
{
	protected abstract string Name { get; }

	public override RewardType Type => RewardType.ScenarioStart;

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"Once, during the next scenario, a character may place a {Name} Totem obstacle in an adjacent empty hex during their turn. {GetDescriptionLabelText(textParameters)}";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		AbilityCmd.SubscribeDuringCharacterTurn(this, EffectType.Selectable,
			character => true,
			async character =>
			{
				Hex hex = await AbilityCmd.SelectHex(GameController.Instance.CharacterManager.GetCharacter(0),
					list =>
					{
						foreach(Hex hex in RangeHelper.GetHexesInRange(character.Hex, 1, false))
						{
							if(hex.IsEmpty())
							{
								list.Add(hex);
							}
						}
					}, mandatory: true, hintText: $"Select a hex to place the {Name} Totem"
				);

				if(hex != null)
				{
					Obstacle obstacle = await AbilityCmd.CreateOverlayTile<Obstacle>(hex,
						ResourceLoader.Load<PackedScene>("res://Content/OverlayTiles/Obstacles/Totem1H.tscn"));
					OnTotemPlaced(obstacle);

					ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.Subscribe(this,
						parameters => parameters.HexObject == obstacle,
						parameters =>
						{
							parameters.Add(new InfoTextExtraEffect.Parameters(GetDescriptionLabelText));
						}
					);

					ScenarioEvents.HexObjectDestroyedEvent.Subscribe(this,
						parameters => parameters.HexObject == obstacle,
						async parameters =>
						{
							ScenarioCheckEvents.GenericInfoItemExtraEffectsCheckEvent.Unsubscribe(this);
							ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(this);

							OnTotemDestroyed(obstacle);

							await GDTask.CompletedTask;
						}
					);
				}

				AbilityCmd.UnsubscribeDuringCharacterTurn(this);
			}, new IconEffectButton.Parameters(Icons.Obstacle),
			new TextEffectInfoView.Parameters(
				$"Place a {Name} Totem obstacle in an adjacent empty hex as a reward from the last Road Event.")
		);
	}

	protected abstract string GetDescriptionLabelText(RichTextParameters textParameters);

	protected virtual void OnTotemPlaced(Obstacle obstacle)
	{
	}

	protected virtual void OnTotemDestroyed(Obstacle obstacle)
	{
	}
}