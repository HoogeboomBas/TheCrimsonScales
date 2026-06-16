using System.Linq;
using Fractural.Tasks;

public class GlowActiveAbility : ActiveAbility<GlowActiveAbility.State>
{
	public class State : ActiveAbilityState
	{
		public GlowAbilityModel[] GlowAbilityModels { get; set; }
	}

	public GlowAbilityModel[] GlowAbilities;

	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IGlowAbilityStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : GlowActiveAbility, new()
	{
		public interface IGlowAbilityStep
		{
			TBuilder WithGlowAbility(params GlowAbilityModel[] glowAbilities);
		}

		public TBuilder WithGlowAbility(params GlowAbilityModel[] glowAbilities)
		{
			Obj.GlowAbilities = glowAbilities;
			return (TBuilder)this;
		}
	}

	public class GlowBuilder : AbstractBuilder<GlowBuilder, GlowActiveAbility>
	{
		internal GlowBuilder() { }
	}

	public static GlowBuilder Builder()
	{
		return new GlowBuilder();
	}


	protected override async GDTask Perform(State abilityState)
	{
		await AskConfirmAndActivate(abilityState);
	}

	protected override async GDTask Activate(State abilityState)
	{
		await base.Activate(abilityState);

		AbilityCmd.SubscribeDuringCharacterTurn(ScenarioEvents.GetSubscriberPair(abilityState, this), EffectType.Selectable,
			character => character == abilityState.Performer &&
			             GlowAbilities.Any(glowAbility =>
				             glowAbility.Elements.All(e =>
					             GameController.Instance.ElementManager.GetState(e) > ElementState.Inert)),
			async character =>
			{
				await LuminaryCardSide.GlowAbility(character, GlowAbilities);
				await GDTask.CompletedTask;
			},
			effectButtonParameters: new IconEffectButton.Parameters(LuminaryCardSide.GlowIconPath),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Perform {Icons.Inline(LuminaryCardSide.GlowIconPath)}"));

		abilityState.GlowAbilityModels = GlowAbilities;
		await GDTask.CompletedTask;
	}

	protected override async GDTask Deactivate(State abilityState)
	{
		await base.Deactivate(abilityState);

		AbilityCmd.UnsubscribeDuringCharacterTurn(ScenarioEvents.GetSubscriberPair(abilityState, this));
	}
}