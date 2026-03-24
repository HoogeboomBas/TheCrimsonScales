using System.Collections.Generic;
using System.Linq;
using Godot;

public class TaintedWaters : MirefootCardModel<TaintedWaters.CardTop, TaintedWaters.CardBottom>
{
	public override string Name => "Tainted Waters";
	public override int Level => 6;
	public override int Initiative => 61;
	protected override int AtlasIndex => 21;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(4)
				.WithConditions([Conditions.Wound2, Conditions.Poison2])
				.WithFilterTargets((state, figure) =>
					figure.Hex.HasHexObjectOfType<DifficultTerrain>())
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.6209408f, 0.64953536f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithRange(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Hex hex in state.UniqueTargetedFigures.Select(figure => figure.Hex).Where(hex => hex.IsFeatureless()))
					{
						Hex selectedHex =
							await AbilityCmd.SelectHex(state, list => list.Add(hex), hintText: "Place difficult terrain?");

						await CreateDifficultTerrain(selectedHex);
					}
				})
				.Build())
		];
	}
}