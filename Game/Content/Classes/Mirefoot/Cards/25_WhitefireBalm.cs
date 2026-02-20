using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class WhitefireBalm : MirefootCardModel<WhitefireBalm.CardTop, WhitefireBalm.CardBottom>
{
	public override string Name => "Whitefire Balm";
	public override int Level => 8;
	public override int Initiative => 20;
	protected override int AtlasIndex => 25;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state,
						list => list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
							.Where(figure => figure.AlliedWith(state.Performer))),
						hintText: () => "Select a character to place a character token on");
					if(figure == null)
					{
						return;
					}

					state.SetCustomValue(this, "Figure", figure);

					await AbilityCmd.AddCharacterToken(state, figure,
						$"On the next two sources of {Icons.Inline(Icons.Damage)} from attacks targeting you, gain {Icons.Inline(Icons.Shield)}3 and {Icons.Inline(Icons.Retaliate)}3");

					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure == figure && parameters.FromAttack && parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.AdjustShield(3);

							object subscriber = new object();


							await AbilityCmd.AddRetaliate(figure, subscriber, 2, 1,
								customCanApplyParameters => customCanApplyParameters.AbilityState == parameters.PotentialAbilityState);

							ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, subscriber,
								canApplyParameters => canApplyParameters.AbilityState == parameters.PotentialAbilityState,
								async _ =>
								{
									AbilityCmd.RemoveRetaliate(figure, subscriber);
									ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, subscriber);

									await GDTask.CompletedTask;
								}
							);
						}
					);

					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == figure,
						applyParameters =>
						{
							applyParameters.AdjustShield(3);
						}
					);

					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == figure,
						applyParameters =>
						{
							applyParameters.AddRetaliate(3, 1);
						}
					);
					//TODO: Tie retliate and shield together

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
						ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
						ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);

						await AbilityCmd.RemoveCharacterToken(state, state.GetCustomValue<Figure>(this, "Figure"));

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.39700016f, 0.37199932f)),
					new UseSlot(new Vector2(0.6060007f, 0.37199932f)),
				])
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.4507118f, 0.771774f)))
				.WithConditions([Conditions.Wound2, Conditions.Stun])
				.Build())
		];
	}
}