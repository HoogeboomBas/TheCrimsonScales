using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SuckerPunch : ChieftainCardModel<SuckerPunch.CardTop, SuckerPunch.CardBottom>
{
	public override string Name => "Sucker Punch";
	public override int Level => 1;
	public override int Initiative => 14;
	protected override int AtlasIndex => 7;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.5089311f, 0.2920354f)))
				.WithConditions(Conditions.Immobilize)
				.Build())
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.WouldSufferDamage && ((Character)state.Performer).Summons.Contains(parameters.Figure),
						async parameters =>
						{
							int damage = parameters.CalculatedCurrentDamage;
							parameters.SetDamagePrevented();

							await AbilityCmd.SufferDamage(state, state.Performer, damage);
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Damage),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Suffer {Icons.Inline(Icons.Damage)} instead of the summon")
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}