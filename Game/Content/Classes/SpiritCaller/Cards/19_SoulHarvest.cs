using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SoulHarvest : SpiritCallerCardModel<SoulHarvest.CardTop, SoulHarvest.CardBottom>
{
	public override string Name => "Soul Harvest";
	public override int Level => 5;
	public override int Initiative => 15;
	protected override int AtlasIndex => 28 - 19;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters =>
							parameters.AbilityState is AttackAbility.State &&
							Spirit.CountsAsSpirit(parameters.Performer),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilitySetHasAdvantage();

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					int spiritCount = 1;
					List<ScenarioEvents.GenericChoice.Subscription> subscriptions = new List<ScenarioEvents.GenericChoice.Subscription>();
					subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async applyParameters =>
						{
							await AbilityCmd.AddCondition(state, state.Performer, Conditions.Curse);

							spiritCount++;

							await GainXP(state);
						},
						effectType: EffectType.Selectable,
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Remove a damage counter from two Spirits instead.")
					));

					EffectCollection effectCollection = AbilityCmd.GenericChoiceCollection(state.Performer, subscriptions);

					List<Figure> selectedSpirits = new List<Figure>();

					for(int i = 0; i < spiritCount; i++)
					{
						Figure spirit = await AbilityCmd.SelectFigure(state, list =>
						{
							foreach(Figure figure in GameController.Instance.Map.Figures)
							{
								if(Spirit.CountsAsSpirit(figure) && !selectedSpirits.Contains(figure))
								{
									list.Add(figure);
								}
							}
						}, effectCollection: i == 0 ? effectCollection : null, hintText: () => $"Select a Spirit");

						selectedSpirits.Add(spirit);

						if(spirit != null)
						{
							await Spirit.RemoveDamageCounters(spirit, 1);
							state.SetPerformed();
						}
					}

					AbilityCmd.ClearGenericChoiceCollection(subscriptions);
				})
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.61863244f, 0.67363274f)))
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(new DynamicInt<HealAbility.State>(state =>
					state.ActionState.GetAbilityState<MoveAbility.State>(0).Hexes.Count(hex => Spirit.HasSpirit(hex)) + 1))
				.WithRange(2)
				.Build())
		];
	}
}