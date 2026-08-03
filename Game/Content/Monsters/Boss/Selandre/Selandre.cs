using System.Collections.Generic;
using Fractural.Tasks;

public class Selandre : MonsterModel, IBossMonsterModel
{
	public override MonsterStats[] BossLevelStats =>
	[
		new MonsterStats()
		{
			Health = 6 * CharacterCount,
			Move = 2,
			Attack = 2,
			Traits =
			[
				new AllNegativeConditionImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 7 * CharacterCount,
			Move = 2,
			Attack = 3,
			Traits =
			[
				new AllNegativeConditionImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 9 * CharacterCount,
			Move = 3,
			Attack = 3,
			Traits =
			[
				new AllNegativeConditionImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 12 * CharacterCount,
			Move = 4,
			Attack = 4,
			Traits =
			[
				new AllNegativeConditionImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 13 * CharacterCount,
			Move = 4,
			Attack = 5,
			Traits =
			[
				new AllNegativeConditionImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 15 * CharacterCount,
			Move = 5,
			Attack = 5,
			Traits =
			[
				new AllNegativeConditionImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 18 * CharacterCount,
			Move = 5,
			Attack = 5,
			Traits =
			[
				new AllNegativeConditionImmunityTrait()
			]
		},
		new MonsterStats()
		{
			Health = 22 * CharacterCount,
			Move = 5,
			Attack = 5,
			Traits =
			[
				new AllNegativeConditionImmunityTrait()
			]
		},
	];

	public override string Name => "Selandre";

	public override string AssetPath => "res://Content/Monsters/Boss/Selandre";

	public override string PortraitTexturePath => $"{AssetPath}/Portrait.tres";

	public override int MaxStandeeCount => 1;

	public override IEnumerable<MonsterAbilityCardModel> Deck => BossAbilityCard.Deck;

	public string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Move, richTextParameters)}2 towards the hex marked {Icons.InlineMarker(Marker.Type.b, richTextParameters)}.
		 Grant the Ancient Artillery:
		 “{Icons.Inline(Icons.Move, richTextParameters)}2, {Icons.Inline(Icons.Push, richTextParameters, ignoreParametersColor: true)}2, {Icons.Inline(Icons.Targets, richTextParameters)}all enemies within {Icons.Inline(Icons.Range, richTextParameters)}2, Attack +0, {Icons.Inline(Icons.Targets, richTextParameters)}all enemies within range.”
		 """;

	public string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Heal, richTextParameters)}{CharacterCount + 1}, Self.
		 Grant the Ancient Artillery:
		 “{Icons.Inline(Icons.Attack, richTextParameters)}{GetAncientArtillery().Stats.Attack}, {Icons.Inline(Icons.Targets, richTextParameters)}all adjacent enemies, {Icons.Inline(Icons.Attack, richTextParameters)}+X, {Icons.Inline(Icons.Targets, richTextParameters)}all enemies at {Icons.Inline(Icons.Range, richTextParameters)}2 or higher, where X is the number of hexes between the Artillery and the target.”
		 """;

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility.Builder()
			.WithDistance(2)
			.WithOnAbilityStarted(async state =>
			{
				ScenarioEvents.FigureFoundFocusEvent.Subscribe(monster, this,
					parameters =>
						parameters.Performer == state.Performer &&
						parameters.AbilityState is MoveAbility.State,
					async parameters =>
					{
						parameters.SetNewFocus(null);
						parameters.SetFocusHex(GetMarkerBHex());

						ScenarioCheckEvents.AIMoveParametersCheckEvent.Subscribe(monster, this,
							moveParameters => moveParameters.Performer == monster,
							moveParameters =>
							{
								moveParameters.SetRange(0);
								moveParameters.SetRangeType(RangeType.Melee);
								moveParameters.SetTargets(1);
								moveParameters.SetAOEPattern(null);
							}
						);

						ScenarioEvents.AbilityEndedEvent.Subscribe(monster, this,
							abilityEndedParameters => abilityEndedParameters.Performer == monster,
							async _ =>
							{
								ScenarioEvents.AbilityEndedEvent.Unsubscribe(monster, this);
								ScenarioCheckEvents.AIMoveParametersCheckEvent.Unsubscribe(monster, this);

								await GDTask.CompletedTask;
							}
						);

						await GDTask.CompletedTask;
					}
				);

				await GDTask.CompletedTask;
			})
			.WithOnAbilityEnded(async state =>
			{
				ScenarioEvents.FigureFoundFocusEvent.Unsubscribe(monster, this);

				await GDTask.CompletedTask;
			})
			.Build()),

		new MonsterAbilityCardAbility(GrantAbility.Builder()
			.WithGetAbilities(state =>
				[
					MoveAbility.Builder()
						.WithDistance(2)
						.Build(),
					PushAbility.Builder()
						.WithPush(2)
						.WithRange(2)
						.WithTarget(Target.Enemies | Target.TargetAll)
						.Build(),
					MonsterAbilityCardModel.AttackAbility(state.Target as Monster, +0, target: Target.Enemies | Target.TargetAll)
				]
			)
			.WithCustomGetTargets((state, list) =>
			{
				list.Add(GetAncientArtillery());
			})
			.WithRequiresLineOfSight(false)
			.Build())
	];

	public IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(new DynamicInt<HealAbility.State>(state => CharacterCount + 1))
			.WithTarget(Target.Self)
			.Build()),

		new MonsterAbilityCardAbility(GrantAbility.Builder()
			.WithGetAbilities(state =>
				[
					MonsterAbilityCardModel.AttackAbility(state.Target as Monster, +0,
						range: 1,
						rangeType: RangeType.Melee,
						target: Target.Enemies | Target.TargetAll),

					MonsterAbilityCardModel.AttackAbility(state.Target as Monster, +0,
						target: Target.Enemies | Target.TargetAll,
						minRange: 2,
						afterTargetConfirmedSubscriptions:
						[
							ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
								parameters => true,
								async parameters =>
								{
									parameters.AbilityState.SingleTargetAdjustAttackValue(
										RangeHelper.Distance(parameters.Performer.Hex, parameters.AbilityState.Target.Hex));

									await GDTask.CompletedTask;
								}
							)
						]
					),
				]
			)
			.WithCustomGetTargets((state, list) =>
			{
				list.Add(GetAncientArtillery());
			})
			.WithRequiresLineOfSight(false)
			.Build())
	];

	private Monster GetAncientArtillery()
	{
		Scenario032 scenario = (Scenario032)GameController.Instance.ScenarioModel;
		return scenario.AncientArtillery;
	}

	private Hex GetMarkerBHex()
	{
		Scenario032 scenario = (Scenario032)GameController.Instance.ScenarioModel;
		return scenario.MarkerB.Hex;
	}
}