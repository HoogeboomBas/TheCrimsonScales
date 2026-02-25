using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using GTweens.Easings;
using GTweensGodot.Extensions;

public class SignalCaster : ArtificerCardModel<SignalCaster.CardTop, SignalCaster.CardBottom>
{
	public override string Name => "Signal Caster";
	public override int Level => 1;
	public override int Initiative => 31;
	protected override int AtlasIndex => 10;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						//TODO: Change to directly selecting the overlay tile
						Hex trapHex = await AbilityCmd.SelectHex(state,
							hexes => hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 2)
								.Where(hex => hex.HasHexObjectOfType<Trap>())));
						if(trapHex == null)
						{
							return;
						}

						Trap trap = trapHex.GetHexObjectOfType<Trap>();
						for(int i = 0; i < 2; i++)
						{
							Hex moveToHex = await AbilityCmd.SelectHex(state,
								hexes => hexes.AddRange(RangeHelper.GetHexesInRange(trap.Hex, 1).Where(hex => hex.IsFeatureless())));
							if(moveToHex == null)
							{
								return;
							}

							await trap.TweenGlobalPosition(moveToHex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine)
								.PlayFastForwardableAsync();
							await GDTask.DelayFastForwardable(0.03f);
							trap.SetOriginHexAndRotation(moveToHex);
							state.SetPerformed();

							if(!trap.Hex.IsUnoccupied())
							{
								await trap.Trigger(state, trap.Hex.GetHexObjectOfType<Figure>());
								await GainScrapToken(state);
								await AbilityCmd.GainXP(state.Performer, 1);
								return;
							}
						}
					}
				)
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder().WithDistance(2).Build(),
					AbilityCmd.SummonFixedAttackRangePlusX(2).Build()
				])
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons);
				})
				.Build())
		];
	}
}