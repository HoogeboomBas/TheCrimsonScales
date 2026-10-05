using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class EyeForAnEye : BruiserCardModel<EyeForAnEye.CardTop, EyeForAnEye.CardBottom>
{
    public override string Name => "Eye for an Eye";
    public override int Level => 1;
    public override int Initiative => 13;
    protected override int AtlasIndex => 1;

    public class CardTop : BruiserCardSide
    {
        protected override List<AbilityCardAbility> GetAbilities() =>
        [
            new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
                .Build()),
			
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == state.Performer,
						async parameters =>
						{
							await AbilityCmd.GainXP(state.Performer, 1);
							ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					await GDTask.CompletedTask;
				})
				.Build())
        ];

        public override bool Round => true;
    }

    public class CardBottom : BruiserCardSide
    {
        protected override List<AbilityCardAbility> GetAbilities() =>
        [
            new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealSquare(this, new Vector2(0.62026906f, 0.7225138f)))
				.WithTarget(Target.Self)
				.Build())
        ];

		public override IEnumerable<CardElementInfusion> Elements => [ CardElementInfusion.Infuse(Element.Earth) ];
    }
}
