using System.Collections.Generic;
using Fractural.Tasks;

public class Road16 : RoadEventModel<Road16.ChoiceA, Road16.ChoiceB>
{
	public override int Number => 16;

	public override string Text =>
		"""
		As you venture down the road, you take notice of a vile stench seeping out from a nearby ditch. You peer down and see a Human corpse lying within.

		The corpse's bony hands are gripped around a long saber and an oddly-shaped shield. This would be the perfect opportunity to stock up on some extra gear for battle, but your bag is packed tight and space is limited.
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"At the start of the next scenario, one character gains the following benefit: During your next two single-target melee attacks, add +1{Icons.Inline(Icons.Attack, textParameters)} to the attack.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			Figure figure = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.GetCharacter(0),
				list =>
				{
					list.AddRange(GameController.Instance.CharacterManager.Characters);
				}, hintText: () => "Choose a character to receive the Road Event benefit"
			);

			if(figure != null)
			{
				int useCount = 0;

				ScenarioEvents.DuringAttackEvent.Subscribe(this,
					parameters =>
						parameters.Performer == figure &&
						parameters.AbilityState.SingleTargetRangeType == RangeType.Melee &&
						parameters.AbilityState.IsSingleTarget,
					async parameters =>
					{
						parameters.AbilityState.SingleTargetAdjustAttackValue(1);

						useCount++;

						if(useCount == 2)
						{
							ScenarioEvents.DuringAttackEvent.Unsubscribe(this);
						}

						await GDTask.CompletedTask;
					}
				);
			}
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Pry the saber out from the right hand.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You pry the saber from the corpse's hand and hold it tight. It's rusty and deteriorated but you could likely get a few good swings out of it.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceBOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"At the start of the next scenario, one character gains the following benefit: On the next three sources of {Icons.Inline(Icons.Damage, textParameters)} from attacks targeting you, gain {Icons.Inline(Icons.Shield, textParameters)}1 for the attacks.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			Figure figure = await AbilityCmd.SelectFigure(GameController.Instance.CharacterManager.GetCharacter(0),
				list =>
				{
					list.AddRange(GameController.Instance.CharacterManager.Characters);
				}, hintText: () => "Choose a character to receive the Road Event benefit"
			);

			if(figure != null)
			{
				int useCount = 0;

				ScenarioEvents.SufferDamageEvent.Subscribe(this,
					canApply: parameters => parameters.FromAttack && parameters.Figure == figure && parameters.WouldSufferDamage,
					apply: async parameters =>
					{
						parameters.AdjustShield(1);

						useCount++;

						if(useCount == 3)
						{
							ScenarioEvents.SufferDamageEvent.Unsubscribe(this);
						}

						await GDTask.CompletedTask;
					},
					EffectType.SelectableMandatory,
					effectButtonParameters: new IconEffectButton.Parameters(Icons.Shield),
					effectInfoViewParameters: new TextEffectInfoView.Parameters(
						$"On the next {3 - useCount} sources of {Icons.Inline(Icons.Damage)} from attacks targeting you, gain {Icons.Inline(Icons.Shield)}1 for the attacks as a reward from the last Road Event.")
				);
			}
		}
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Grab the shield from the left hand.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You slide the shield out from between the skeleton's bony fingers and clang your blade against it. It's in poor shape but likely to protect you from a few blows before it decays.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBOnScenarioStartedReward()
		];
	}
}