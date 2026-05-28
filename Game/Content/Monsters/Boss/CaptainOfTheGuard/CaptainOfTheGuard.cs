using System.Collections.Generic;
using Fractural.Tasks;

public class CaptainOfTheGuard : MonsterModel, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 7 * CharacterCount,
			Move = 2,
			Attack = 3,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Muddle), ConditionImmunityTrait.WoundImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 9 * CharacterCount,
			Move = 2,
			Attack = 3,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Muddle), ConditionImmunityTrait.WoundImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 11 * CharacterCount,
			Move = 2,
			Attack = 4,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Muddle), ConditionImmunityTrait.WoundImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 14 * CharacterCount,
			Move = 3,
			Attack = 4,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Muddle), ConditionImmunityTrait.WoundImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 16 * CharacterCount,
			Move = 3,
			Attack = 5,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Muddle), ConditionImmunityTrait.WoundImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 20 * CharacterCount,
			Move = 3,
			Attack = 5,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Muddle), ConditionImmunityTrait.WoundImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 21 * CharacterCount,
			Move = 4,
			Attack = 6,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Muddle), ConditionImmunityTrait.WoundImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 25 * CharacterCount,
			Move = 4,
			Attack = 6,
			Traits =
			[
				new ApplyConditionTrait(Conditions.Disarm), new ConditionImmunityTrait(Conditions.Stun),
				new ConditionImmunityTrait(Conditions.Muddle), ConditionImmunityTrait.WoundImmunityTrait()
			]
		},
	];

	public override string Name => "Captain of the Guard";

	public override string AssetPath => "res://Content/Monsters/Boss/CaptainOfTheGuard";
	public override string PortraitTexturePath => $"{AssetPath}/Portrait.tres";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	// IBossMonsterModel
	public string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Heal, richTextParameters)}{2}, {Icons.Inline(Icons.Targets, richTextParameters)}all.
		 """;

	public string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 All allies add +{1}{Icons.Inline(Icons.Attack, richTextParameters)} to all attacks this round.
		 {Icons.Inline(Icons.Attack, richTextParameters)}{monster.Stats.Attack + 1}.
		 """;

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.TargetAll | Target.SelfOrAllies).Build())
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherActiveAbility.Builder()
			.WithOnActivate(async state =>
			{
				ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
					parameters => parameters.Performer.AlliedWith(state.Performer),
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						await GDTask.CompletedTask;
					}
				);

				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async state =>
			{
				ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

				await GDTask.CompletedTask;
			})
			.Build()),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +1)),
	];
}