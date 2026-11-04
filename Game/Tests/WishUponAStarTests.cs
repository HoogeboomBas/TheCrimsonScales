using System.Linq;
using System.Threading.Tasks;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace CrimsonScales.Tests;

[TestSuite]
[RequireGodotRuntime]
public class WishUponAStarTests
{
	// Regression for deaca9b5: the Bless follow-up must affect both fully healed targets.
	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Consuming_dark_blesses_both_targets_healed_to_full_health()
	{
		await using TestGame game = await TestGame.Start(ScenarioSetup.InTestRoom()
			.WithCharacter<StarslingerModel>(out var starslinger, new Vector2I(3, 4))
			.WithCard<WishUponAStar>(starslinger)
			.WithCharacter<BombardModel>(out var first, new Vector2I(4, 4))
			.WithCharacter<BombardModel>(out var second, new Vector2I(3, 5)));

		first.Unit.SetHealth(first.Unit.MaxHealth - 3);
		second.Unit.SetHealth(second.Unit.MaxHealth - 3);
		AssertThat(BlessCount(first)).IsEqual(0);
		AssertThat(BlessCount(second)).IsEqual(0);
		await AbilityCmd.InfuseElement(null, Element.Dark, immediately: true);

		await game.PlayCard<WishUponAStar>(starslinger, AbilityCardSection.Top, choices => choices
			.SelectTarget(first)
			.SelectTarget(second)
			// Select the offered Dark consumption, then finish the effects prompt.
			.Answer<ScenarioEventPrompt>(prompt => new ScenarioEventPrompt.Answer
			{
				SelectedEffectIndex = prompt.EffectCollection.ApplicableEffects.Single().Index
			})
			.Answer<ScenarioEventPrompt>(_ =>
			{
				AssertThat(first.Unit.Health).IsEqual(first.Unit.MaxHealth);
				AssertThat(second.Unit.Health).IsEqual(second.Unit.MaxHealth);
				AssertThat(GameController.Instance.ElementManager.GetState(Element.Dark)).IsEqual(ElementState.Inert);
				return new ScenarioEventPrompt.Answer();
			})
			.SelectTarget(first)
			.SelectTarget(second)
			.Answer<TargetSelectionPrompt>(_ => new TargetSelectionPrompt.Answer { Skipped = true }));

		AssertThat(first.Unit.Health).IsEqual(first.Unit.MaxHealth);
		AssertThat(second.Unit.Health).IsEqual(second.Unit.MaxHealth);
		AssertThat(BlessCount(first)).IsEqual(1);
		AssertThat(BlessCount(second)).IsEqual(1);
		AssertThat(BlessCount(starslinger)).IsEqual(0);
		AssertThat(GameController.Instance.ElementManager.GetState(Element.Dark)).IsEqual(ElementState.Inert);
		AssertThat(starslinger.Unit.Cards.Single().CardState).IsEqual(CardState.Discarded);
	}

	private static int BlessCount(UnitHandle<Character> character) => character.Unit.AMDCardDeck.DrawPile.Count(card => card.Model is BlessAMDCard);
}
