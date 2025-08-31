using Fractural.Tasks;
using System.Collections.Generic;
using Godot;

public class LongSpear : Prosperity9Item
{
	public override string Name => "Long-Spear";
	public override int ItemNumber => 26;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 8; // TODO varadski 24.08.2025: no clue what this does

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

						AOEHex grayHex = new AOEHex(Vector2I.Zero, AOEHexType.Gray);
						AOEHex adjacentHex = new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red);
						AOEHex distantHex = new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red);
						List<AOEHex> hexes =
						[
							grayHex,
							adjacentHex,
							distantHex
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