using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;
using Godot;

public class FearsomeTaunt : BruiserCardModel<FearsomeTaunt.CardTop, FearsomeTaunt.CardBottom>
{
	public override string Name => "Fearsome Taunt";
	public override int Level => 1;
	public override int Initiative => 10;
	protected override int AtlasIndex => 10;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithPush(3, new PushSquare(this, new Vector2(0.5f, 0.3f)))
				.Build()),
		];

		public override int XP => 1;
	}

	public class CardBottom : BruiserCardSide
	{
		public static Character Taunting { get; private set; }

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Taunting = (Character)state.Performer;
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					Taunting = null;
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}