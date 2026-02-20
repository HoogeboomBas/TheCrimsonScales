using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ImperviousArmor : ChieftainCardModel<ImperviousArmor.CardTop, ImperviousArmor.CardBottom>
{
	public override string Name => "Impervious Armor";
	public override int Level => 7;
	public override int Initiative => 86;
	protected override int AtlasIndex => 23;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Battle Rhinoceros")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/battle_rhinoceros_AI.png")
				.WithHealth(7, new SummonHealthSquare(this, new Vector2(0.4311768f, 0.2107228f)))
				.WithMove(2, new SummonMoveSquare(this, new Vector2(0.62168443f, 0.2107228f)))
				.WithAttack(2, new SummonAttackSquare(this, new Vector2(0.4311768f, 0.28662243f)))
				.WithTraits(
					new ShieldTrait(1),
					new PierceTrait(3),
					new MountTrait(
						async (owner, mount) =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.Subscribe(owner, this,
								parameters => parameters.Figure == owner,
								parameters =>
								{
									parameters.AdjustShield(1);
								}
							);
							ScenarioEvents.SufferDamageEvent.Subscribe(owner, this,
								parameters => parameters.Figure == owner && parameters.FromAttack,
								async parameters =>
								{
									parameters.AdjustShield(1);
									await GDTask.CompletedTask;
								}
							);
							await GDTask.CompletedTask;
						},
						async (owner, mount) =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(owner, this);
							ScenarioEvents.SufferDamageEvent.Unsubscribe(owner, this);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => [ShieldAbility.Builder().WithShieldValue(2).Build()])
				.WithCustomGetTargets((state, figures) =>
				{
					Figure mount = Chieftain.GetMount(state.Performer);
					if(mount != null)
					{
						figures.Add(mount);
					}

					figures.Add(state.Performer);
				})
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.Build()
			)
		];

		public override bool Round => true;
	}
}