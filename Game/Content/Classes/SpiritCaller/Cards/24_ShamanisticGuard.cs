using System.Collections.Generic;
using Fractural.Tasks;

public class ShamanisticGuard : SpiritCallerCardModel<ShamanisticGuard.CardTop, ShamanisticGuard.CardBottom>
{
	public override string Name => "Shamanistic Guard";
	public override int Level => 7;
	public override int Initiative => 18;
	protected override int AtlasIndex => 28 - 24;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Vengeful Phantasm")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/vengeful_phantasm.png")
				.WithHealth(2)
				.WithMove(1)
				.WithTraits(new RetaliateTrait(2))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Spirit spirit = state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit;

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure == spirit,
						async parameters =>
						{
							foreach(Figure figure in spirit.Hex.GetFigures())
							{
								if(state.Performer.EnemiesWith(figure))
								{
									await AbilityCmd.SufferDamage(state, figure, 2);
								}
							}
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.WithSkipConfirmation()
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Character character = await AbilityCmd.SelectFigure(state, list =>
					{
						foreach(Figure spirit in Spirit.GetAllSpirits())
						{
							foreach(Figure otherFigure in RangeHelper.GetFiguresInRange(spirit, 1, requiresLineOfSight: false))
							{
								if(otherFigure is Character character)
								{
									list.AddIfNew(character);
								}
							}
						}
					}) as Character;

					if(character == null)
					{
						return;
					}

					state.SetPerformed();

					state.SetCustomValue(this, "Character", character);

					if(!await AbilityCmd.AskConsumeElement(state.Performer, Element.Air))
					{
						return;
					}

					await AbilityCmd.RemoveAllNegativeConditions(character);
				})
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithConditions(Conditions.Bless)
				.WithCustomGetTargets((state, list) =>
				{
					list.Add(state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Character>(this, "Character"));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}
}