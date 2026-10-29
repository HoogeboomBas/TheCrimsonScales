using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

/// <summary>
/// An <see cref="ActiveAbility{T}"/> that has a number of uses before it is discarded/lost.
/// </summary>
public class CrystallizeAbility : ActiveAbility<CrystallizeAbility.State>
{
	public class State : ActiveAbilityState, IUseSlotsAbilityState
	{
		public bool DiscardOtherCrystallize { get; set; }
		public List<UseSlot> Slots { get; set; }
		public int UseSlotIndex { get; set; }

		public void SetSlots(List<UseSlot> slots)
		{
			Slots = slots;
		}

		public async GDTask AdvanceUseSlot()
		{
			UseSlot from = Slots[UseSlotIndex];

			if(from.OnExit != null)
			{
				await from.OnExit.Invoke(this);
			}

			UseSlotIndex++;

			if(UseSlotIndex >= Slots.Count)
			{
				await ScenarioEvents.CrystallizeOffLastSlotEvent.CreatePrompt(
					new ScenarioEvents.CrystallizeOffLastSlot.Parameters(Performer));
				await ActionState.RequestDiscardOrLose();
			}
		}

		public async GDTask MoveBackUseSlot()
		{
			UseSlotIndex--;

			await GDTask.CompletedTask;
		}
	}

	public List<UseSlot> UseSlots { get; private set; } = [];
	public bool DiscardOtherCrystallize { get; private set; } = true;

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in CrystallizeAbility. Enables inheritors of CrystallizeAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending CrystallizeAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IUseSlotsStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : CrystallizeAbility, new()
	{
		public interface IUseSlotsStep
		{
			TBuilder WithUseSlots(List<UseSlot> useSlots);
		}

		public TBuilder WithUseSlots(List<UseSlot> useSlots)
		{
			Obj.UseSlots = useSlots;
			return (TBuilder)this;
		}

		public TBuilder WithDiscardOtherCrystallize(bool discardOtherCrystallize)
		{
			Obj.DiscardOtherCrystallize = discardOtherCrystallize;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class CrystallizeBuilder : AbstractBuilder<CrystallizeBuilder, CrystallizeAbility>
	{
		internal CrystallizeBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of CrystallizeBuilder.
	/// </summary>
	/// <returns></returns>
	public static CrystallizeBuilder.IUseSlotsStep Builder()
	{
		return new CrystallizeBuilder();
	}

	public CrystallizeAbility() { }

	protected override async GDTask Perform(State abilityState)
	{
		await Activate(abilityState);
	}

	protected override async GDTask Activate(State abilityState)
	{
		await base.Activate(abilityState);

		abilityState.SetSlots(UseSlots);
		abilityState.DiscardOtherCrystallize = DiscardOtherCrystallize;

		if(DiscardOtherCrystallize)
		{
			ActionState actionState = ((Character)abilityState.Performer).Cards
				.SelectMany(card => card.ActiveActionStates)
				.FirstOrDefault(actionState => actionState.AbilityStates.Any(state =>
					state is State crystallizeState && crystallizeState != abilityState && crystallizeState.DiscardOtherCrystallize));

			if(actionState != null)
			{
				await actionState.RequestDiscardOrLose();
			}
		}

		//TODO: Make it so that you can choose when there are multiple
		ScenarioEvents.SufferDamageEvent.Subscribe(abilityState, this,
			parameters => parameters.WouldSufferDamage && parameters.FromAttack && parameters.Figure == abilityState.Performer &&
			              abilityState.UseSlotIndex < abilityState.Slots.Count,
			async parameters =>
			{
				parameters.AdjustFinalDamageAdjustment(-1);

				await abilityState.AdvanceUseSlot();
			}, EffectType.MandatoryAfterOptionals, 1000, canApplyMultipleTimesInEffectCollection: true);
	}

	protected override async GDTask Deactivate(State abilityState)
	{
		await base.Deactivate(abilityState);

		ScenarioEvents.SufferDamageEvent.Unsubscribe(abilityState, this);
	}
}