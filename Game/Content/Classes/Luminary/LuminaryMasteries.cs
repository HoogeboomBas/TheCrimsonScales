using System.Collections.Generic;
using Fractural.Tasks;

public class LuminaryMasteries
{
	public class Mastery1 : MasteryModel
	{
		public override string Description =>
			$"For an entire scenario, on each of your turns except the first, perform at least one {Icons.Inline(LuminaryCardSide.GlowIconPath)} ability.";

		public override int MaxProgress => 1;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			mastery.AdjustProgress();

			bool performedGlow = true;

			ScenarioEvents.AbilityPerformedEvent.Subscribe(this,
				parameters => parameters.AbilityState.GetCustomValue<bool>(parameters.Performer, "Glow Ability") && parameters.Performer == character,
				async _ =>
				{
					performedGlow = true;
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(character, this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(character, this);
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
						parameters => parameters.Figure == character,
						async _ =>
						{
							performedGlow = false;
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character && !performedGlow,
				async _ =>
				{
					mastery.ResetProgress();
					mastery.SetTemporarilyFailed(true);
					await GDTask.CompletedTask;
				});
		}
	}

	public class Mastery2 : MasteryModel
	{
		public override string Description =>
			"In a single scenario, infuse 4 different elements in a single turn and consume 4 different elements in a single turn";

		public override int MaxProgress => 2;

		public override async GDTask OnScenarioSetupPhaseCompleted(Mastery mastery, Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(mastery, character);

			List<Element> infused = [];
			bool completedInfuse = false;
			List<Element> consumed = [];
			bool completedConsume = false;

			ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					consumed.Clear();
					infused.Clear();
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FinishElementConsumedEvent.Subscribe(this,
				parameters => !completedConsume && parameters.PotentialConsumer == character,
				async parameters =>
				{
					consumed.AddIfNew(parameters.ConsumedElement);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.ElementInfusedEvent.Subscribe(this,
				parameters => !completedInfuse && parameters.PotentialInfuser == character,
				async parameters =>
				{
					infused.AddIfNew(parameters.Element);
					await GDTask.CompletedTask;
				});

			ScenarioEvents.FigureTurnEndedEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async _ =>
				{
					if(infused.Count >= 4)
					{
						mastery.AdjustProgress();
						completedInfuse = true;
					}

					if(consumed.Count >= 4)
					{
						mastery.AdjustProgress();
						completedConsume = true;
					}

					await GDTask.CompletedTask;
				});
		}
	}
}