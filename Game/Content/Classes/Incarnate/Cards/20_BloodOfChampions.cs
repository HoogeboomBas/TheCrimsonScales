using System.Collections.Generic;
using Fractural.Tasks;

public class BloodOfChampions : IncarnateCardModel<BloodOfChampions.CardTop, BloodOfChampions.CardBottom>
{
	public override string Name => "Blood of Champions";
	public override int Level => 5;
	public override int Initiative => 82;
	protected override int AtlasIndex => 20;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					bool used = false;
					ScenarioEvents.IncarnateSpiritChangedEvent.Subscribe(state, this,
						parameters => !used && state.Performer == parameters.Incarnate,
						async parameters =>
						{
							used = true;
							Ability ability = null;
							switch(parameters.Spirit)
							{
								case IncarnateSpirit.Ritualist:
									ability = ConditionAbility.Builder().WithConditions(Incarnate.Enfeeble).WithRange(2).Build();
									break;
								case IncarnateSpirit.Conqueror:
									ability = ConditionAbility.Builder().WithConditions(Incarnate.Empower).WithRange(2).Build();
									break;
								case IncarnateSpirit.Reaver:
									ability = AttackAbility.Builder().WithDamage(2).Build();
									break;
							}

							await new ActionState(state.Performer, [ability]).Perform();
						});

					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && !used,
						async _ =>
						{
							await state.ActionState.RequestDiscardOrLose();
						});

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						_ => used,
						async _ =>
						{
							used = false;

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.IncarnateSpiritChangedEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
				{
					if(InSpirit(state.Performer, IncarnateSpirit.Ritualist))
					{
						return
						[
							MoveAbility.Builder()
								.WithDistance(2)
								.WithMoveType(MoveType.Jump)
								.Build()
						];
					}

					if(InSpirit(state.Performer, IncarnateSpirit.Conqueror))
					{
						return
						[
							HealAbility.Builder()
								.WithHealValue(2)
								.WithTarget(Target.Self)
								.Build()
						];
					}

					if(InSpirit(state.Performer, IncarnateSpirit.Reaver))
					{
						return
						[
							AttackAbility.Builder()
								.WithDamage(2)
								.WithConditions(Conditions.Rupture)
								.Build()
						];
					}

					return [];
				})
				.WithTarget(Target.SelfOrAllies | Target.SelfCountsForTargets)
				.WithTargets(2)
				.WithRange(2)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(InSpirit(state.Performer, IncarnateSpirit.Ritualist))
					{
						await AbilityCmd.InfuseElement(state, Element.Air);
					}

					if(InSpirit(state.Performer, IncarnateSpirit.Conqueror))
					{
						await AbilityCmd.InfuseElement(state, Element.Earth);
					}

					if(InSpirit(state.Performer, IncarnateSpirit.Reaver))
					{
						await AbilityCmd.InfuseElement(state, Element.Fire);
					}
				})
				.Build())
		];
	}
}