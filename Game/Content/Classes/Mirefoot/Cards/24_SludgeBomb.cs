using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SludgeBomb : MirefootCardModel<SludgeBomb.CardTop, SludgeBomb.CardBottom>
{
	public override string Name => "Sludge Bomb";
	public override int Level => 7;
	public override int Initiative => 07;
	protected override int AtlasIndex => 24;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.35703704f, 0.19851269f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.5674074f, 0.19788358f)))
				.WithConditions([Conditions.Wound1, Conditions.Immobilize])
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						parameters => true,
						async parameters =>
						{
							List<Hex> selectedHexes = [];
							Hex targetHex = parameters.AbilityState.Target.Hex;
							if(targetHex.IsFeatureless())
							{
								selectedHexes.Add(await AbilityCmd.SelectHex(parameters.Performer, list => list.Add(targetHex),
									hintText: "Place difficult terrain in the hex occupied by the target?"));
							}

							selectedHexes.AddRange(await AbilityCmd.SelectHexes(parameters.Performer,
								list => list.AddRange(RangeHelper.GetHexesInRange(targetHex, 1, false).Where(hex => hex.IsFeatureless())),
								0, 3, false, "Place up to 3 difficult terrain tiles in hexes adjacent to the target"));

							foreach(Hex hex in selectedHexes)
							{
								await CreateDifficultTerrain(hex);
							}
						}))
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					//TODO: Change to selecting the actual overlay tiles as opposed to the hexes
					List<Hex> selectedHexes = await AbilityCmd.SelectHexes(state,
						list => list.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 1)
							.Where(hex => hex.HasHexObjectOfType<DifficultTerrain>())), 0, 4, false,
						"Destroy up to 4 adjacent difficult terrain tiles");
					foreach(Hex hex in selectedHexes)
					{
						await AbilityCmd.DestroyDifficultTerrain(hex.GetHexObjectOfType<DifficultTerrain>());
						state.SetPerformed();
					}

					state.SetCustomValue(this, "DestroyedDifficultTerrain", selectedHexes.Count);
				})
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer,
						async parameters =>
						{
							ConditionModel conditionModel = state.ActionState.GetAbilityState<OtherAbility.State>(0)
									.GetCustomValue<int>(this, "DestroyedDifficultTerrain") switch
								{
									1 => Conditions.Poison1,
									2 => Conditions.Poison2,
									3 => Conditions.Poison3,
									4 => Conditions.Poison4,
									_ => throw new ArgumentOutOfRangeException()
								};

							parameters.AbilityState.SingleTargetAddCondition(conditionModel);
							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.4989973f, 0.88249975f), GainXP))
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override bool Persistent => true;
	}
}