using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class Colorless : MonsterModel, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 9 * CharacterCount,
			Move = 3,
			Attack = 2,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				ConditionImmunityTrait.PoisonImmunityTrait(), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 10 * CharacterCount,
			Move = 3,
			Attack = 3,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				ConditionImmunityTrait.PoisonImmunityTrait(), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 11 * CharacterCount,
			Move = 4,
			Attack = 3,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				ConditionImmunityTrait.PoisonImmunityTrait(), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 12 * CharacterCount,
			Move = 4,
			Attack = 4,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				ConditionImmunityTrait.PoisonImmunityTrait(), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 14 * CharacterCount,
			Move = 4,
			Attack = 4,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				ConditionImmunityTrait.PoisonImmunityTrait(), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 15 * CharacterCount,
			Move = 4,
			Attack = 5,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				ConditionImmunityTrait.PoisonImmunityTrait(), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 17 * CharacterCount,
			Move = 4,
			Attack = 6,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				ConditionImmunityTrait.PoisonImmunityTrait(), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
		new MonsterStats()
		{
			Health = 19 * CharacterCount,
			Move = 5,
			Attack = 7,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Muddle), new ConditionImmunityTrait(Conditions.Stun),
				ConditionImmunityTrait.PoisonImmunityTrait(), ConditionImmunityTrait.WoundImmunityTrait(),
				new ConditionImmunityTrait(Conditions.Curse)
			]
		},
	];

	public override string Name => "Colorless";

	public override string AssetPath => "res://Content/Monsters/Boss/Colorless";
	public override string PortraitTexturePath => $"{AssetPath}/Portrait.tres";
	public override string MapIconTexturePath => $"{AssetPath}/Icon.tres";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	public abstract string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters);
	public abstract string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters);
	public abstract IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster);
	public abstract IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster);
}