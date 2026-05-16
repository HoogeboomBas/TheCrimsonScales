using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class MajesticMalevolence : HollowpactLevelUpCardModel<MajesticMalevolence.CardTop, MajesticMalevolence.CardBottom>
{
	public override string Name => "Majestic Malevolence";
	public override int Level => 3;
	public override int Initiative => 89;
	protected override int AtlasIndex => 3;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							if(parameters.AbilityState is TargetedAbilityState targetedAbilityState)
							{
								await targetedAbilityState.SetPerformHex(hexes =>
								{
									hexes.AddRange(GameController.Instance.Map.GetChildrenOfType<Obstacle>()
										.Where(obstacle => obstacle is VoidPit)
										.Select(obstacle => obstacle.Hex));
								});
							}
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters("res://Content/Classes/Hollowpact/cs-void-pit.png"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters("Perform the ability as if you were occupying a hex with a Void Pit"));

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{	
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.44718847f, 0.19982125f)))
				.WithRange(3)
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealDiamondPlus(this, new Vector2(0.44718847f, 0.19982125f)))
				.WithRange(1)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark,
					effectInfoText: $"{Icons.Inline(Icons.Heal)}3{Icons.Inline(Icons.Range)}1"))
				.WithOnAbilityEndedPerformed(GainXP)
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateVoidPitObstacleAbilityBuilder()
				.WithRange(3)
				.WithOnAbilityEndedPerformed(GainVoidEnergy)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditionalAbilityCheck(async state =>
				{
					return await AbilityCmd.HasPerformedAbility(state, 0) && await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 2, 
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Attack)}3{Icons.Inline(Icons.GetCondition(Conditions.Muddle))}, {Icons.Inline(Icons.Targets)}1 enemy adjacent to the created Void Pit."));
				})
				.WithCustomGetTargets((state, hexes) =>
				{
					Hex obstacleHex = state.ActionState.GetAbilityState<CreateObstacleAbility.State>(0).CreatedObstacles.First().Hex;
					hexes.AddRange(obstacleHex.Neighbours.SelectMany(hex => hex.GetFigures()));
				})
				.Build()),
		];
	}
}