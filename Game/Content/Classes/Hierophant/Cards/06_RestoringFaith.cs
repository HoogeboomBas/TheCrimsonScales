using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RestoringFaith : HierophantCardModel<RestoringFaith.CardTop, RestoringFaith.CardBottom>
{
	public override string Name => "Restoring Faith";
	public override int Level => 1;
	public override int Initiative => 64;
	protected override int AtlasIndex => 13 - 6;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1, new HealDiamondPlus(this, new Vector2(0.43512207f, 0.17212221f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.65840006f, 0.17251107f)))
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
				[
					LootAbility.Builder()
						.WithRange(1)
						.WithCustomGetLootObtainer(state => state.ActionState.ParentActionState.Performer)
						.Build()
				])
				.WithCustomGetTargets((state, list) => list.Add(state.ActionState.GetAbilityState<HealAbility.State>(0).UniqueTargetedFigures[0]))
				.WithTarget(Target.SelfOrAllies)
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						return state.ActionState.GetAbilityState<HealAbility.State>(0).Performed;
					}
				)
				.Build())
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6194556f, 0.6955332f)))
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Character character = await AbilityCmd.SelectFigure(state, list =>
					{
						foreach(Character character in GameController.Instance.CharacterManager.Characters)
						{
							if(character != state.Performer && !character.IsDead)
							{
								list.Add(character);
							}
						}
					}) as Character;

					if(character != null)
					{
						AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard(character, CardState.Persistent, false, card =>
						{
							if(card.Top.Model is not HierophantPrayerCardSide)
							{
								return false;
							}

							foreach(ActionState activeActionState in card.ActiveActionStates)
							{
								foreach(AbilityState abilityState in activeActionState.AbilityStates)
								{
									if(abilityState is UseSlotAbility.State useSlotAbilityState)
									{
										if(useSlotAbilityState.UseSlotIndex > 0)
										{
											// This card has a prayer ability active
											return true;
										}
									}
								}
							}

							return false;
						}, hintText: "Select a prayer card to move the character token back on");

						if(abilityCard != null)
						{
							foreach(ActionState activeActionState in abilityCard.ActiveActionStates)
							{
								foreach(AbilityState abilityState in activeActionState.AbilityStates)
								{
									if(abilityState is UseSlotAbility.State useSlotAbilityState)
									{
										if(useSlotAbilityState.UseSlotIndex > 0)
										{
											// This card has a prayer ability active
											await useSlotAbilityState.MoveBackUseSlot();
											state.SetPerformed();
										}
									}
								}
							}
						}
					}
				})
				.Build())
		];
	}
}