using Fractural.Tasks;
using System.Collections.Generic;
using Godot;

public class BattleAxe : Prosperity2Item
{
	public override string Name => "Battle-Axe";
	public override int ItemNumber => 18;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 6;

	protected override void Subscribe()
	{
		base.Subscribe();


		SubscribeDuringAttackAbilityStarted(
			canApply: state => state.Performer == Owner &&
			                   state.AbilityTargets == 1 &&
			                   state.AbilityRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					Dictionary<Vector2I, AOEHexType> aoeHexes = new Dictionary<Vector2I, AOEHexType>();

					List<AOEHex> hexes =
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
					];
					AOEPattern aoePattern = new AOEPattern(hexes);

					// TODO varadski 24.08.2025: common logic used for aoe prompts. Should be moved to a single place to remove duplicates
					if(state.Authority is Character)
					{
						AOEPrompt.Answer aoeAnswer =
							await PromptManager.Prompt(new AOEPrompt(state, aoePattern, null, null, () => "Select where to target"),
								state.Authority);

						if(aoeAnswer.Skipped)
						{
							return;
						}

						for(int i = 0; i < aoeAnswer.HexCoords.Count; i++)
						{
							aoeHexes.Add(aoeAnswer.HexCoords[i], aoeAnswer.HexTypes[i]);
						}
					}
					state.AOEHexes = aoeHexes;
					state.AbilityTargets = 2;

					await GDTask.CompletedTask;
				});
			}
		);

	}
}