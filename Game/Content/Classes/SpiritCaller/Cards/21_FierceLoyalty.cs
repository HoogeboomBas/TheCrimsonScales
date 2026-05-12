using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FierceLoyalty : SpiritCallerCardModel<FierceLoyalty.CardTop, FierceLoyalty.CardBottom>
{
	public override string Name => "Fierce Loyalty";
	public override int Level => 6;
	public override int Initiative => 20;
	protected override int AtlasIndex => 28 - 21;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Mimicking Sprite")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/mimicking_sprite.png")
				.WithHealth(1)
				.WithConditionalAbilityCheck(async state =>
				{
					Spirit spirit = await AbilityCmd.SelectFigure(state, list =>
					{
						foreach(Figure figure in GameController.Instance.Map.Figures)
						{
							if(figure is Spirit)
							{
								list.Add(figure);
							}
						}
					}, hintText: () => $"Select a Spirit") as Spirit;

					if(spirit != null)
					{
						state.SetCustomValue(this, "SpiritToCopy", spirit);

						return true;
					}

					return false;
				})
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Spirit spiritToCopy = state.ActionState.GetAbilityState<SpawnAbility.State>(0).GetCustomValue<Spirit>(this, "SpiritToCopy");

					Spirit spirit = state.ActionState.GetAbilityState<SpawnAbility.State>(0).Spirit;

					foreach(FigureTrait trait in spiritToCopy.Traits)
					{
						await spirit.AddTrait(trait.AbstractModel.ToMutable());
					}

					spirit.SetAttack(spiritToCopy.Attack);
					spirit.SetMove(spiritToCopy.Move);
					spirit.SetRange(spiritToCopy.Range);

					spirit.SetAsFirstSpirit();

					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == spirit &&
							parameters.AbilityState is MoveAbility.State or AttackAbility.State or HealAbility.State,
						async parameters =>
						{
							if(parameters.AbilityState is MoveAbility.State moveAbilityState)
							{
								moveAbilityState.AdjustMoveValue(2);
							}

							if(parameters.AbilityState is AttackAbility.State attackAbilityState)
							{
								attackAbilityState.AbilityAdjustAttackValue(2);
							}

							if(parameters.AbilityState is HealAbility.State healAbilityState)
							{
								healAbilityState.AbilityAdjustHealValue(2);
							}

							await GDTask.CompletedTask;
						}
					);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.WithMandatory(true)
				.WithSkipConfirmation()
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6181081f, 0.6304685f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure spirit = state.GetCustomValue<Figure>(this, "Spirit");

					await AbilityCmd.Teleport(state, spirit, state.Performer.Hex);

					ScenarioEvents.HexObjectDestroyedEvent.Subscribe(state, this,
						parameters =>
							parameters.HexObject == spirit &&
							spirit.Hex == state.Performer.Hex,
						async parameters =>
						{
							await AbilityCmd.AddShield(state.Performer, this, 2);

							if(await AbilityCmd.AskConsumeElement(state.Authority, Element.Dark))
							{
								await AbilityCmd.AddRetaliate(state.Performer, this, 1, 1);
							}
						}
					);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(state, this);
					AbilityCmd.RemoveShield(state.Performer, this);
					AbilityCmd.RemoveRetaliate(state.Performer, this);

					await GDTask.CompletedTask;
				})
				.WithSkipConfirmation()
				.WithConditionalAbilityCheck(async state =>
				{
					if(Spirit.HasSpirit(state.Performer.Hex))
					{
						return false;
					}

					Figure spirit = await Spirit.SelectSpirit(state);

					if(spirit == null)
					{
						return false;
					}

					if(!AbilityCmd.CanForceMoveTo(spirit, state.Performer.Hex))
					{
						return false;
					}

					state.SetCustomValue(this, "Spirit", spirit);

					return true;
				})
				.Build())
		];

		public override bool Round => true;
	}
}