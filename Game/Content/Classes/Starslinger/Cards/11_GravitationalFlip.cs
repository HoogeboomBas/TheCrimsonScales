using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class GravitationalFlip : StarslingerCardModel<GravitationalFlip.CardTop, GravitationalFlip.CardBottom>
{
	public override string Name => "Gravitational Flip";
	public override int Level => 1;
	public override int Initiative => 13;
	protected override int AtlasIndex => 11;

	public class CardTop : StarslingerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithCustomGetTargets((state, targets) =>
				{
					LootAbility.State lootAbilityState = state.ActionState.GetAbilityState<LootAbility.State>(0);
					targets.AddRange(
						lootAbilityState.LootedHexes
							.SelectMany(hex => RangeHelper.GetFiguresInRange(hex, 1))
							.Where(f => f.EnemiesWith(state.Performer))
					);
				})
				.Build())
		];
	}

	public class CardBottom : StarslingerCardSide
	{
		private MoveEnhancementMark _enhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_enhancementMark = new MoveCircle(this, new Vector2(0.6215662f, 0.82212216f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(2, _enhancementMark)
						.Build()
				])
				.WithRange(3)
				.WithOnAbilityStarted(async state =>
				{
					await AbilityCmd.GenericChoice(state.Performer,
					[
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetCustomValue(this, "ChoseGrant", true);
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform grant ability"),
							effectType: EffectType.SelectableMandatory
						),
						ScenarioEvents.GenericChoice.Subscription.New(
							applyFunction: async applyParameters =>
							{
								state.SetBlocked();
								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.Move),
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"Perform control ability"),
							effectType: EffectType.SelectableMandatory
						)
					], hintText: "Select an ability to perform:");
				})
				.Build()),

			new AbilityCardAbility(ControlAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(2, _enhancementMark)
						.Build()
				])
				.WithRange(3)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return !state.ActionState.GetAbilityState<GrantAbility.State>(0).GetCustomValue<bool>(this, "ChoseGrant");
				})
				.Build())
		];
	}
}