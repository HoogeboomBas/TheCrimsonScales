using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class WarbornsShout : IncarnateCardModel<WarbornsShout.CardTop, WarbornsShout.CardBottom>
{
	public override string Name => "Warborn's Shout";
	public override int Level => 1;
	public override int Initiative => 88;
	protected override int AtlasIndex => 8;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Enfeeble)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
						]
					),
					new AOEHexMark(Vector2I.Zero.Add(Direction.NorthWest), this, new Vector2(0.61930263f, 0.33518007f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.SouthEast), this, new Vector2(0.7194155f, 0.45706376f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Empower)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithCustomGetTargets((abilityState, list) =>
				{
					ConditionAbility.State attackAbilityState = abilityState.ActionState.GetAbilityState<ConditionAbility.State>(0);

					list.AddRange(attackAbilityState
						.GetRedAOEHexes()
						.SelectMany(hex => hex.GetHexObjectsOfType<Figure>()));
				})
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						ConditionAbility.State attackAbilityState = state.ActionState.GetAbilityState<ConditionAbility.State>(0);

						return attackAbilityState.TargetedAOEHexes != null && attackAbilityState.TargetedAOEHexes.Count != 0;
					}
				)
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.61795056f, 0.766305f)))
				.Build())
		];
	}
}