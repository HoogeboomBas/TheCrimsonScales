using System.Collections.Generic;
using System;

public class ChieftainAMDCard01 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;

	public override List<Ability> Abilities => 
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

	public ChieftainAMDCard01(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}