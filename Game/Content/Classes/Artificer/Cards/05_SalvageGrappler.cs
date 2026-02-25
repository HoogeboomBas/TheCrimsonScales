using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SalvageGrappler : ArtificerCardModel<SalvageGrappler.CardTop, SalvageGrappler.CardBottom>
{
	public override string Name => "Salvage Grappler";
	public override int Level => 1;
	public override int Initiative => 46;
	protected override int AtlasIndex => 5;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(3, new RangeSquare(this, new Vector2(0.5074074f, 0.20052908f)))
				.WithPull(1, new PullSquare(this, new Vector2(0.72050005f, 0.20105818f)))
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
						parameters => parameters.AbilityState.Target.IsDead && coinsToLoot > 0,
						async _ =>
						{
							for(int i = 0; i < coinsToLoot; i++)
							{
								PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/CoinStack.tscn");
								Coin coinStack = scene.Instantiate<Coin>();
								GameController.Instance.Map.AddChild(coinStack);
								await coinStack.Init(abilityState.Target.Hex);

								await coinStack.Loot(abilityState.Performer);
							}
							await GainScrapToken(abilityState);
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

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(2)
				.WithRange(3)
				.WithAbilityStartedSubscription(
					LoseScrapTokenSubscription<ScenarioEvents.AbilityStarted.Parameters>(1,
						async parameters =>
						{
							CreateTrapAbility.State state = (CreateTrapAbility.State)parameters.AbilityState;
							state.AdjustAbilityDamage(1);
							state.AddConditions(Conditions.Immobilize);
							await AbilityCmd.GainXP(state.Performer, 1);
						},
						new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Damage)}, {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}")))
				.Build())
		];
	}
}