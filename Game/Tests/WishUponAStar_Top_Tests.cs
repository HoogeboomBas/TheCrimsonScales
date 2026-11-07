using System;
using System.Linq;
using System.Threading.Tasks;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace CrimsonScales.Tests;

[TestSuite]
[RequireGodotRuntime]
public class WishUponAStar_Top_Tests
{
	private const int HealAmount = 3;
	private const int DamageBelowHealAmount = HealAmount - 1;
	private const int DamageAboveHealAmount = HealAmount + 1;
	private const int NoDamage = 0;

	public enum DarkChoice { Consume, Decline, Unavailable }

	[TestCase(HealAmount, HealAmount, DarkChoice.Consume, true, true, Timeout = 30000)]
	[TestCase(DamageBelowHealAmount, DamageAboveHealAmount, DarkChoice.Consume, true, false, Timeout = 30000)]
	[TestCase(DamageAboveHealAmount, DamageAboveHealAmount, DarkChoice.Consume, false, false, Timeout = 30000)]
	[TestCase(HealAmount, HealAmount, DarkChoice.Decline, false, false, Timeout = 30000)]
	[TestCase(HealAmount, HealAmount, DarkChoice.Unavailable, false, false, Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Only_targets_at_full_health_receive_bless_when_dark_is_consumed(
		int firstHealthLoss, int secondHealthLoss, DarkChoice darkChoice,
		bool firstReceivesBless, bool secondReceivesBless) =>
		await RunScenario(firstHealthLoss, secondHealthLoss, darkChoice, firstReceivesBless, secondReceivesBless);


	private static async Task RunScenario(int firstHealthLoss, int secondHealthLoss,
		DarkChoice darkChoice, bool firstReceivesBless, bool secondReceivesBless)
	{
		bool darkAvailable = darkChoice != DarkChoice.Unavailable;
		bool consumeDark = darkChoice == DarkChoice.Consume;
		await using TestGame game = await TestGame.Start(ScenarioSetup.InTestRoom()
			.WithCharacter<StarslingerModel>(out var starslinger, new Vector2I(3, 4))
			.WithCard<WishUponAStar>(starslinger)
			.WithCharacter<BombardModel>(out var first, new Vector2I(4, 4))
			.WithCharacter<BombardModel>(out var second, new Vector2I(3, 5)));

		first.Unit.SetHealth(first.Unit.MaxHealth - firstHealthLoss);
		second.Unit.SetHealth(second.Unit.MaxHealth - secondHealthLoss);
		foreach(UnitHandle<Character> unit in new[] { first, second, starslinger })
		{
			AssertThat(BlessCount(unit)).IsEqual(0);
		}
		if(darkAvailable)
		{
			await AbilityCmd.InfuseElement(null, Element.Dark, immediately: true);
		}

		await game.PlayCard<WishUponAStar>(starslinger, AbilityCardSection.Top, choices =>
		{
			choices.SelectTarget(first).SelectTarget(second);
			if(darkAvailable)
			{
				if(consumeDark) choices.ConsumeElement(Element.Dark);
				else choices.FinishEffects(); // Decline the offered Dark consumption.
			}

			if(consumeDark)
			{
				if(firstReceivesBless) choices.SelectTarget(first);
				if(secondReceivesBless) choices.SelectTarget(second);
				choices.FinishTargetSelection(prompt =>
				{
					// A still-injured target and an unhealed full-health unit must not be eligible.
					if(!firstReceivesBless) AssertRejected(prompt, first);
					if(!secondReceivesBless) AssertRejected(prompt, second);
					AssertRejected(prompt, starslinger);
				});
			}
		});

		AssertThat(first.Unit.Health).IsEqual(first.Unit.MaxHealth - Math.Max(0, firstHealthLoss - HealAmount));
		AssertThat(second.Unit.Health).IsEqual(second.Unit.MaxHealth - Math.Max(0, secondHealthLoss - HealAmount));
		AssertThat(BlessCount(first)).IsEqual(firstReceivesBless ? 1 : 0);
		AssertThat(BlessCount(second)).IsEqual(secondReceivesBless ? 1 : 0);
		AssertThat(starslinger.Unit.Health).IsEqual(starslinger.Unit.MaxHealth);
		AssertThat(BlessCount(starslinger)).IsEqual(0);
		AssertThat(GameController.Instance.ElementManager.GetState(Element.Dark))
			.IsEqual(darkAvailable && !consumeDark ? ElementState.Strong : ElementState.Inert);
		AssertThat(starslinger.Unit.Cards.Single().CardState).IsEqual(CardState.Discarded);
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task Starslinger_can_heal_and_bless_themselves()
	{
		await using TestGame game = await TestGame.Start(ScenarioSetup.InTestRoom()
			.WithCharacter<StarslingerModel>(out var starslinger, new Vector2I(3, 4))
			.WithCard<WishUponAStar>(starslinger));
		starslinger.Unit.SetHealth(starslinger.Unit.MaxHealth - HealAmount);
		AssertThat(BlessCount(starslinger)).IsEqual(0);
		await AbilityCmd.InfuseElement(null, Element.Dark, immediately: true);

		await game.PlayCard<WishUponAStar>(starslinger, AbilityCardSection.Top, choices => choices
			.SelectTarget(starslinger)
			.FinishTargetSelection()
			.ConsumeElement(Element.Dark)
			.SelectTarget(starslinger)
			.FinishTargetSelection());

		AssertThat(starslinger.Unit.Health).IsEqual(starslinger.Unit.MaxHealth);
		AssertThat(BlessCount(starslinger)).IsEqual(1);
		AssertThat(GameController.Instance.ElementManager.GetState(Element.Dark)).IsEqual(ElementState.Inert);
		AssertThat(starslinger.Unit.Cards.Single().CardState).IsEqual(CardState.Discarded);
	}

	[TestCase(Timeout = 30000)]
	[GodotExceptionMonitor]
	public async Task An_already_full_health_ally_can_be_targeted_and_blessed() =>
		await RunScenario(NoDamage, HealAmount, DarkChoice.Consume,
			firstReceivesBless: true, secondReceivesBless: true);


	private static void AssertRejected(TargetSelectionPrompt prompt, UnitHandle<Character> unit)
	{
		prompt.Selection.Refresh();
		AssertThrown(() => prompt.Selection.Select(unit.Unit)).IsInstanceOf<InvalidOperationException>();
	}

	private static int BlessCount(UnitHandle<Character> character) =>
		character.Unit.AMDCardDeck.DrawPile.Count(card => card.Model is BlessAMDCard);
}
