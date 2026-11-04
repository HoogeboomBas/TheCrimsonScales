using System.Threading.Tasks;
using Fractural.Tasks;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace CrimsonScales.Tests;

[TestSuite]
[RequireGodotRuntime]
public class CardScenarioTests
{
	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Basic_attack_resolves_through_a_card_and_discards_it()
	{
		await using TestGame game = await TestGame.Start(AttackSetup(out var hero, out var guard));
		await game.PlayCard<DoubleCannons>(hero, AbilityCardSection.BasicTop, choices => choices.SelectTarget(guard));
		AssertThat(guard.Unit.Health).IsEqual(8);
		AssertThat(hero.Unit.Cards[0].CardState).IsEqual(CardState.Discarded);
		AssertThat(game.Map.Hexes.Count).IsEqual(100);
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Starting_poison_adds_damage_during_real_card_resolution()
	{
		await using TestGame game = await TestGame.Start(AttackSetup(out var hero, out var guard).WithCondition(guard, Conditions.Poison1));
		await game.PlayCard<DoubleCannons>(hero, AbilityCardSection.BasicTop, choices => choices.SelectTarget(guard));
		AssertThat(guard.Unit.Health).IsEqual(7);
		AssertThat(guard.Unit.HasCondition(Conditions.Poison1)).IsTrue();
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Printed_card_bottom_activates_its_round_shield()
	{
		await using TestGame game = await TestGame.Start(AttackSetup(out var hero, out var guard));
		await game.PlayCard<DoubleCannons>(hero, AbilityCardSection.Bottom, choices => choices.Confirm());
		AssertThat(hero.Unit.Cards[0].CardState).IsEqual(CardState.Round);
		AssertThat(hero.Unit.Cards[0].ActiveActionStates.Count).IsEqual(1);
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Basic_card_movement_uses_real_pathfinding()
	{
		await using TestGame game = await TestGame.Start(AttackSetup(out var hero, out var guard));
		await game.PlayCard<DoubleCannons>(hero, AbilityCardSection.BasicBottom,
			choices => choices.MoveTo(new Vector2I(3, 6)));
		AssertThat(hero.Unit.Hex.Coords).IsEqual(new Vector2I(3, 6));
		AssertThat(hero.Unit.Cards[0].CardState).IsEqual(CardState.Discarded);
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Programmatic_obstacles_register_on_the_board()
	{
		await using TestGame game = await TestGame.Start(AttackSetup(out var hero, out var guard)
			.WithOverlay("res://Content/OverlayTiles/Obstacles/Bush1H.tscn", new Vector2I(5, 5)));
		AssertThat(game.Map.GetHex(new Vector2I(5, 5)).HasHexObjectOfType<Obstacle>()).IsTrue();
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Sessions_can_be_recreated_without_carrying_conditions_or_card_state()
	{
		ScenarioSetup setup = AttackSetup(out var hero, out var guard);
		await using(TestGame first = await TestGame.Start(setup))
		{
			await guard.Unit.AddCondition(Conditions.Poison1, null);
			await first.PlayCard<DoubleCannons>(hero, AbilityCardSection.BasicTop, choices => choices.SelectTarget(guard));
			AssertThat(guard.Unit.Health).IsEqual(7);
		}
		AssertThrown(() => { _ = hero.Unit; }).IsInstanceOf<System.InvalidOperationException>();
		await using TestGame second = await TestGame.Start(setup);
		AssertThat(guard.Unit.HasCondition(Conditions.Poison1)).IsFalse();
		await second.PlayCard<DoubleCannons>(hero, AbilityCardSection.BasicTop, choices => choices.SelectTarget(guard));
		AssertThat(guard.Unit.Health).IsEqual(8);
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Recorded_answers_replay_without_requesting_a_new_decision()
	{
		await using TestGame game = await TestGame.Start(AttackSetup(out var hero, out var guard));
		var answers = GameController.Instance.SavedScenario.PromptAnswers;
		AssertThat(answers.Count).IsEqual(0);
		answers.Add(new TargetSelectionPrompt.Answer { FigureReferenceId = guard.Unit.ReferenceId });

		// The injected scripted source is empty and would fail if a new answer were requested.
		await game.PlayCard<DoubleCannons>(hero, AbilityCardSection.BasicTop);

		AssertThat(guard.Unit.Health).IsEqual(8);
		AssertThat(answers.Count).IsEqual(1);
	}

	[TestCase(false, Timeout = 30000)]
	[TestCase(true, Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Interactive_decisions_leave_fast_forward_and_cleanup_the_prompt(bool fail)
	{
		await using TestGame game = await TestGame.Start(AttackSetup(out var hero, out var guard));
		GameController.SetFastForward(true);
		DecisionProbePrompt prompt = new DecisionProbePrompt(fail);
		InteractiveDecisionSource decisions = new InteractiveDecisionSource();

		if(fail)
		{
			var error = await AssertThrown(decisions.Decide(prompt, hero.Unit).AsTask());
			error.IsInstanceOf<System.InvalidOperationException>();
		}
		else
		{
			ConfirmPrompt.Answer answer = await decisions.Decide(prompt, hero.Unit);
			AssertThat(answer.Confirmed).IsTrue();
		}

		AssertThat(prompt.CleanupCount).IsEqual(1);
		AssertThat(GameController.FastForward).IsFalse();
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Interactive_decision_cancellation_does_not_cleanup_a_removed_scene()
	{
		await using TestGame game = await TestGame.Start(AttackSetup(out var hero, out var guard));
		DecisionProbePrompt prompt = new DecisionProbePrompt(false, waitForSceneExit: true);
		Task<ConfirmPrompt.Answer> decision = new InteractiveDecisionSource().Decide(prompt, hero.Unit).AsTask();

		await game.DisposeAsync();

		var error = await AssertThrown(decision);
		error.IsInstanceOf<System.OperationCanceledException>();
		AssertThat(prompt.CleanupCount).IsEqual(0);
	}

	private sealed class DecisionProbePrompt(bool fail, bool waitForSceneExit = false) : Prompt<ConfirmPrompt.Answer>(null, null)
	{
		public int CleanupCount { get; private set; }

		public override async GDTask<ConfirmPrompt.Answer> Decide(Figure authority)
		{
			if(waitForSceneExit)
			{
				await GDTask.WaitUntil(() => false, cancellationToken: GameController.CancellationToken);
			}
			if(fail) throw new System.InvalidOperationException("Decision failed.");
			return new ConfirmPrompt.Answer { Confirmed = true };
		}

		protected override void Disable() => CleanupCount++;
	}

	private static ScenarioSetup AttackSetup(out UnitHandle<Character> hero, out UnitHandle<Monster> guard) => ScenarioSetup.InTestRoom()
		.WithCharacter<BombardModel>(out hero, new Vector2I(3, 4))
		.WithCard<DoubleCannons>(hero)
		.WithMonster<BanditGuard>(out guard, new Vector2I(4, 4), health: 10)
		.WithNeutralAttackModifiers();
}
