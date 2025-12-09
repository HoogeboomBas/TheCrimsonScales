using System.Collections.Generic;

public class ChieftainAMDCard01 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 1;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override List<Ability> GetAbilities(AttackAbility.State state) => 
	[
		HealAbility.Builder()
			.WithHealValue(1)
			.WithCustomGetTargets((healState, figures) =>
			{
				Character character = 
					state.Performer is Character performer ? performer : 
					state.Performer is Summon summon ? summon.CharacterOwner : 
					(Character)state.Authority;

				figures.Add(character);
			})
			.WithTarget(Target.SelfOrAllies)
			.Build()
	];
}