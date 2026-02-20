using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class FireKnightScrollOfProtection : FireKnightItem
{
	public override string Name => "Scroll of Protection";
	public override int ItemNumber => 9;
	protected override int AtlasIndex => 10 - 9;

	private object _subscriber;

	protected override List<ItemUseSlot> GetUseSlots() =>
	[
		new ItemUseSlot(new Vector2(0.32149974f, 0.7880109f), async item => await AbilityCmd.InfuseElement(null, Element.Fire, item.Owner)),
		new ItemUseSlot(new Vector2(0.6015022f, 0.7880109f))
	];

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.InflictConditionsEvent.Subscribe(this, _subscriber,
			parameters => parameters.Target == Owner && parameters.ConditionModels.Count > 0 &&
			              parameters.ConditionModels.Any(conditionModel => conditionModel.IsNegative),
			async parameters =>
			{
				await Use(async user =>
				{
					List<ScenarioEvents.GenericChoice.Subscription> subscriptions =
						new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();
					foreach(ConditionModel conditionModel in parameters.ConditionModels)
					{
						if(conditionModel.IsNegative)
						{
							subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
								applyFunction: async applyParameters =>
								{
									parameters.PreventCondition(conditionModel);

									await GDTask.CompletedTask;
								},
								effectType: EffectType.SelectableMandatory,
								effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(conditionModel)),
								effectInfoViewParameters: new TextEffectInfoView.Parameters(
									$"Prevent {Icons.Inline(Icons.GetCondition(conditionModel))}")
							));
						}
					}

					await AbilityCmd.GenericChoice(user, subscriptions, hintText: "Select a condition to prevent");
				});
			},
			canApplyMultipleTimesInEffectCollection: true
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.InflictConditionsEvent.Unsubscribe(this, _subscriber);
	}
}