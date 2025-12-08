using System.Collections.Generic;

public class ChieftainAMDCard01 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 1;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override List<Ability> GetAbilities(AttackAbility.State state) => 
	[
		HealAbility.Builder()
			.WithHealValue(1)
			.WithConditionalAbilityCheck(async state =>
			{
				Character character = 
					state.Performer is Character performer ? performer : 
					state.Performer is Summon summon ? summon.CharacterOwner : null;

				return character != null ? ScenarioCheckEvents.IsMountedCheckEvent.Fire(
						new ScenarioCheckEvents.IsMountedCheck.Parameters(character)).IsMounted : false;
			})
			.WithCustomGetTargets((state, figures) =>
			{
				Character character = 
					state.Performer is Character performer ? performer : 
					state.Performer is Summon summon ? summon.CharacterOwner : null;

				if(character != null)
				{
					figures.Add(ScenarioCheckEvents.IsMountedCheckEvent.Fire(
						new ScenarioCheckEvents.IsMountedCheck.Parameters(character)).Mount);
				}
			})
			.WithTarget(Target.Allies)
			.Build()
	];
}