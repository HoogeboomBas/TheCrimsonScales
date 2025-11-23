using System.Collections.Generic;
using Fractural.Tasks;

public class WarPaint : ChieftainCardModel<WarPaint.CardTop, WarPaint.CardBottom>
{
	public override string Name => "War Paint";
	public override int Level => 4;
	public override int Initiative => 28;
	protected override int AtlasIndex => 17;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Invisible);

					Figure mount = Chieftain.GetMount(state.Performer);
					if(mount != null)
					{
						await AbilityCmd.AddCondition(state,mount, Conditions.Invisible);
						state.SetCustomValue(this, "Mount", mount);
					}

					state.SetCustomValue(this, "IsMounted", mount != null);
				})
				.WithOnDeactivate(async state => 
				{
					await AbilityCmd.RemoveCondition(state.Performer, Conditions.Invisible);

					if(state.GetCustomValue<bool>(this, "IsMounted"))
                    {
						// The figure might not be mounted at this moment, still remove the invisibility
                        await AbilityCmd.RemoveCondition(state.GetCustomValue<Figure>(this, "Mount"), Conditions.Invisible);
                    }
				})
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];

		protected override bool Round => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					// If targeted by an enemy while mounted, reduce own sorting initiative for targeting purposes
					ScenarioCheckEvents.PotentialTargetCheckEvent.Subscribe(state, this,
						parameters => parameters.PotentialTarget == state.Performer,
						parameters =>
						{
							if(Chieftain.GetIsMounted(state.Performer))
							{
								parameters.AdjustTargetSortingInitiative(-10);
							}
						}
					);

					ScenarioEvents.NextActiveFigureEvent.Subscribe(state, this,
						parameters =>
						{
							// If owner already acted then this effect was already applied
							if(parameters.PreviousActiveFigure == state.Performer)
                            {
                                return false;
                            }

							return Chieftain.GetMount(state.Performer) == parameters.NextActiveFigure;
						},
						async parameters =>
						{
							Figure mount = Chieftain.GetMount(state.Performer);

							// Choose to act before the mount, mount's initiative increased to owner + 1
							ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(state, this,
								parameters => parameters.Figure == mount,
								parameters => parameters.SetSortingInitiative(state.Performer.Initiative.SortingInitiative + 1),
								order: 10
							);

							mount.UpdateInitiative();
							parameters.SetSortingRequired();

							ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(state, this);

							await GDTask.CompletedTask;
						},
						effectType: EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters("res://Content/Classes/Chieftain/Icon.svg"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters("Act before your mounted summon.")
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					state.Performer.UpdateInitiative();

					ScenarioCheckEvents.PotentialTargetCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.NextActiveFigureEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}