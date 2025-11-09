using System.Collections.Generic;
using System;

public class ChieftainAMDCard02 : AMDCard
{
	public override AMDCardType Type => AMDCardType.Value;
	public override int? Value(AttackAbility.State state) => 0;

	public override List<Ability> Abilities => 
	[
		HealAbility.Builder()
			.WithHealValue(1)
			.WithCustomGetTargets((state, figures) =>
			{
				Character character = 
					state.Performer is Character performer ? performer : 
					state.Performer is Summon summon ? summon.CharacterOwner : 
					(Character)state.Authority;
				figures.AddRange(character.Summons);
			})
			.WithTarget(Target.SelfOrAllies | Target.TargetAll)
			.Build()
	];

	public ChieftainAMDCard02(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}