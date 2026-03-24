using System.Collections.Generic;
using System.Linq;

public class ComplexToxicology : MirefootCardModel<ComplexToxicology.CardTop, ComplexToxicology.CardBottom>
{
	public override string Name => "Complex Toxicology";
	public override int Level => 9;
	public override int Initiative => 35;
	protected override int AtlasIndex => 28;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(GameController.Instance.Map.Figures.Where(figure => figure.HasWound()));
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Stun)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(GameController.Instance.Map.Figures.Where(figure => figure.HasCondition(Conditions.Immobilize)));
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(GameController.Instance.Map.Figures.Where(figure => figure.HasCondition(Conditions.Muddle)));
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(GameController.Instance.Map.Figures.Where(figure => figure.HasPoison()));
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Room performerRoom = state.Performer.Hex.Room;

					if(performerRoom == null)
					{
						return;
					}

					foreach(Figure figure in performerRoom.Figures.Where(figure =>
						        figure.EnemiesWith(state.Performer) &&
						        figure.Conditions.Count(condition => condition.ConditionModel.IsNegative) >= 2))
					{
						await AbilityCmd.SufferDamage(state, figure, 2);
						state.SetPerformed();
					}
				})
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithTargets(2)
				.WithRange(3)
				.WithConditions([Conditions.Wound1, Conditions.Poison2, Conditions.Muddle])
				.Build())
		];
	}
}