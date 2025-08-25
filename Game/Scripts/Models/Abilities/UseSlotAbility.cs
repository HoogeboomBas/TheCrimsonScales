using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// An <see cref="ActiveAbility{T}"/> that has a number of uses before it is discarded/lost.
/// </summary>
public class UseSlotAbility : ActiveAbility<UseSlotAbility.State>
{
	public class State : ActiveAbilityState
	{
		public List<UseSlot> Slots { get; private set; }
		public int UseSlotIndex { get; private set; }

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
				await ActionState.RequestDiscardOrLose();
			}
		}

		public async GDTask MoveBackUseSlot()
		{
			UseSlotIndex--;

			await GDTask.CompletedTask;
		}
	}

	public List<UseSlot> UseSlots { get; protected set; } = [];
	// TODO varadski 21.08.2025: seen in OtherActiveAbility as well; move to ActiveAbility.cs?
	public Func<State, GDTask> OnActivate { get; protected set; }
	public Func<State, GDTask> OnDeactivate { get; protected set; }

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in UseSlotAbility. Enables inheritors of UseSlotAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending UseSlotAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IOnActivateStep,
		AbstractBuilder<TBuilder, TAbility>.IOnDeactivateStep,
		AbstractBuilder<TBuilder, TAbility>.IUseSlotsStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : UseSlotAbility, new()
	{
		public interface IOnActivateStep
		{
			IOnDeactivateStep WithOnActivate(Func<State, GDTask> onActivate);
		}

		public interface IOnDeactivateStep
		{
			IUseSlotsStep WithOnDeactivate(Func<State, GDTask> onDeactivate);
		}

		public interface IUseSlotsStep
		{
			TBuilder WithUseSlot(UseSlot useSlot);
			TBuilder WithUseSlots(List<UseSlot> useSlots);
		}

		public TBuilder WithUseSlot(UseSlot useSlot)
		{
			Obj.UseSlots.Add(useSlot);
			return (TBuilder)this;
		}

		public TBuilder WithUseSlots(List<UseSlot> useSlots)
		{
			Obj.UseSlots = useSlots;
			return (TBuilder)this;
		}

		public IOnDeactivateStep WithOnActivate(Func<State, GDTask> onActivate)
		{
			Obj.OnActivate = onActivate;
			return (TBuilder)this;
		}

		public IUseSlotsStep WithOnDeactivate(Func<State, GDTask> onDeactivate)
		{
			Obj.OnDeactivate = onDeactivate;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class UseSlotBuilder : AbstractBuilder<UseSlotBuilder, UseSlotAbility>
	{
		internal UseSlotBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of UseSlotBuilder.
	/// </summary>
	/// <returns></returns>
	public static AbstractBuilder<UseSlotBuilder, UseSlotAbility>.IOnActivateStep Builder()
	{
		return new UseSlotBuilder();
	}

	public UseSlotAbility() { }

	public UseSlotAbility(List<UseSlot> useSlots,
		Func<State, GDTask> onActivate, Func<State, GDTask> onDeactivate,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getHintText = null,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed, conditionalAbilityCheck, getHintText, abilityStartedSubscriptions,
			abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		UseSlots = useSlots;
		OnActivate = onActivate;
		OnDeactivate = onDeactivate;
	}

	protected override async GDTask Perform(State abilityState)
	{
		await AskConfirmAndActivate(abilityState);
	}

	protected override async GDTask Activate(State abilityState)
	{
		await base.Activate(abilityState);

		abilityState.SetSlots(UseSlots);

		await OnActivate(abilityState);
	}

	protected override async GDTask Deactivate(State abilityState)
	{
		await base.Deactivate(abilityState);

		await OnDeactivate(abilityState);
	}
}