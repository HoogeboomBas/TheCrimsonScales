using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class InoxBodyguardScenario032 : InoxBodyguard
{
	// IBossMonsterModel
	public override string GetSpecial1Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Move, richTextParameters)}{monster.Stats.Move - 1}.
		 {Icons.Inline(Icons.Attack, richTextParameters)}{monster.Stats.Attack - 1}.
		 Then focus on a new target and repeat these abilities until there are no new targets.
		 """;

	public override string GetSpecial2Description(Monster monster, RichTextParameters richTextParameters) =>
		$"""
		 {Icons.Inline(Icons.Move, richTextParameters)}{monster.Stats.Move}.
		 {Icons.Inline(Icons.Attack, richTextParameters)}{monster.Stats.Attack}.
		 Grant self and all Inox allies:
		 {Icons.Inline(Icons.Retaliate, richTextParameters)}2.
		 {Icons.Inline(Icons.Shield, richTextParameters)}2.
		 If there are no other Inox alive:
		 Summon {(GetMonsterType(ModelDB.Monster<InoxArcher>())).ToString()} Inox Archer.
		 Summon {(GetMonsterType(ModelDB.Monster<InoxGuard>())).ToString()} Inox Guard.
		 """;

	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial1Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				bool exhaustedAllPotentialTargets = false;
				List<Figure> focusedFigures = [];

				ScenarioCheckEvents.CanBeFocusedCheckEvent.Subscribe(monster, this,
					parameters => focusedFigures.Contains(parameters.PotentialTarget),
					parameters =>
					{
						parameters.SetCannotBeFocused();
					}
				);

				ScenarioEvents.FigureFoundFocusEvent.Subscribe(monster, this,
					parameters => parameters.Performer == monster,
					async parameters =>
					{
						if(parameters.Focus != null)
						{
							focusedFigures.AddIfNew(parameters.Focus);
						}
						else
						{
							exhaustedAllPotentialTargets = true;
						}

						await GDTask.CompletedTask;
					});

				while(!exhaustedAllPotentialTargets)
				{
					ActionState actionState = new ActionState(state.ActionState, state.Performer,
					[
						MonsterAbilityCardModel.MoveAbility(monster, -1),
						MonsterAbilityCardModel.AttackAbility(monster, -1),
					]);

					await actionState.Perform();
				}

				ScenarioCheckEvents.CanBeFocusedCheckEvent.Unsubscribe(monster, this);
				ScenarioEvents.FigureFoundFocusEvent.Unsubscribe(monster, this);
			})
			.Build()),
	];

	public override IEnumerable<MonsterAbilityCardAbility> GetSpecial2Abilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(MonsterAbilityCardModel.AttackAbility(monster, +0)),
		new MonsterAbilityCardAbility(GrantAbility.Builder().WithGetAbilities(grantAbilityState =>
			[
				RetaliateAbility.Builder().WithRetaliateValue(2).Build(),
				ShieldAbility.Builder().WithShieldValue(2).Build(),
			])
			.WithTarget(Target.SelfOrAllies | Target.TargetAll)
			.WithCustomGetTargets((state, list) =>
			{
				list.AddRange(GetAllInox());
			})
			.Build()),
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<InoxArcher>())
			.WithMonsterType(GetMonsterType(ModelDB.Monster<InoxArcher>()))
			.WithConditionalAbilityCheck(async state =>
			{
				await GDTask.CompletedTask;
				return GetAllInox().Count == 1;
			})
			.Build()),
		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<InoxGuard>())
			.WithMonsterType(GetMonsterType(ModelDB.Monster<InoxGuard>()))
			.WithConditionalAbilityCheck(async state => await AbilityCmd.HasPerformedAbility(state, 3))
			.Build()),
	];

	private static MonsterType GetMonsterType(MonsterModel monsterModel)
	{
		int characterCount = GameController.Instance.SavedCampaign.Characters.Count;
		return monsterModel switch
		{
			InoxArcher => characterCount >= 3 ? MonsterType.Elite : MonsterType.Normal,
			InoxGuard => characterCount >= 4 ? MonsterType.Elite : MonsterType.Normal,
			_ => MonsterType.Normal
		};
	}

	private static List<Figure> GetAllInox()
	{
		return GameController.Instance.Map.Figures
			.Where(figure => figure is Monster monsterFigure &&
			                 monsterFigure.MonsterModel is InoxArcher or InoxGuard or InoxShaman or InoxBodyguardScenario032)
			.ToList();
	}
}