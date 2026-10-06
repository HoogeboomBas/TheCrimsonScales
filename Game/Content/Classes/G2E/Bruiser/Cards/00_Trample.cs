using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Trample : BruiserCardModel<Trample.CardTop, Trample.CardBottom>
{
	public override string Name => "Trample";
	public override int Level => 1;
	public override int Initiative => 72;
	protected override int AtlasIndex => 0;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.486666667f, 0.27333333f)))
				.WithPierce(2)
				.Build()),
		];
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					List<Figure> movedThrough = new();

					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && !parameters.ForcedMovement,
						async parameters =>
						{
							Figure enemy = parameters.Hex.GetHexObjectOfType<Figure>();
							if(enemy != null && state.Performer.EnemiesWith(enemy) && !movedThrough.Contains(enemy))
							{
								movedThrough.Add(enemy);
							}

							await GDTask.CompletedTask;
						});

					ActionState move = new ActionState(state.Performer,
					[
						MoveAbility.Builder()
							.WithDistance(4, new MoveSquare(this, new Vector2(0.5026666667f, 0.647619047f)))
							.WithMoveType(MoveType.Jump)
							.Build()
					]);
					await move.Perform();

					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

					Figure landedOn = state.Performer.Hex.GetHexObjectOfType<Figure>();
					movedThrough.Remove(landedOn);

					foreach(Figure enemy in movedThrough)
					{
						if(enemy.IsDestroyed)
						{
							continue;
						}

						ActionState attack = new ActionState(state.Performer,
						[
							AttackAbility.Builder()
								.WithDamage(3)
								.WithCustomGetTargets((abilityState, list) => list.Add(enemy))
								.Build()
						]);
						await attack.Perform();
						await AbilityCmd.GainXP(state.Performer, 1);
					}
				})
				.Build())
		];
	}
}
