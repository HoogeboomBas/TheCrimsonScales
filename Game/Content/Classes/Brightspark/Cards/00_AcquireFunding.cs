using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class AcquireFunding : BrightsparkCardModel<AcquireFunding.CardTop, AcquireFunding.CardBottom>
{
	public override string Name => "Acquire Funding";
	public override int Level => 1;
	public override int Initiative => 61;
	protected override int AtlasIndex => 0;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(3, new RangeSquare(this, new Vector2(0.61454064f, 0.22757141f)))
				.WithOnAbilityStarted(async abilityState =>
				{
					int coinsToLoot = 0;
					ScenarioCheckEvents.SpawnCoinCheckEvent.Subscribe(abilityState, this,
						canApplyParameters => abilityState.Target == canApplyParameters.Dropper && canApplyParameters.CoinsToSpawn > 0,
						applyParameters =>
						{
							coinsToLoot = applyParameters.CoinsToSpawn;
							applyParameters.SetCoinsToSpawn(0);
						}, order: 100
					);
					ScenarioEvents.AfterAttackPerformedEvent.Subscribe(abilityState, this,
						parameters => parameters.AbilityState == abilityState && parameters.AbilityState.Target.IsDead && coinsToLoot > 0,
						async parameters =>
						{
							List<Coin> coins = [];

							for(int i = 0; i < coinsToLoot; i++)
							{
								coins.AddRange(await AbilityCmd.SpawnCoin(abilityState.Target.Hex));
							}

							foreach(Coin coin in coins)
							{
								await coin.Loot(abilityState.Performer);
							}

							coinsToLoot = 0;
						});

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
					{
						ScenarioCheckEvents.SpawnCoinCheckEvent.Unsubscribe(abilityState, this);
						ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(abilityState, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							ActionState actionState = new ActionState(parameters.Figure,
							[
								MoveAbility.Builder().WithDistance(1).Build(),
								LootAbility.Builder().WithRange(1).Build()
							]);
							await actionState.Perform();
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29199997f, 0.7944986f)),
					new UseSlot(new Vector2(0.4999998f, 0.7944986f)),
					new UseSlot(new Vector2(0.7079987f, 0.7944986f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}