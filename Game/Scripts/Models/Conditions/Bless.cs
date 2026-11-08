using Fractural.Tasks;

public class Bless : ConditionModel
{
	public override string Name => "Bless";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Bless.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Positive;
	public override bool CanBeAppliedMultipleTimesOnSingleTarget => true;
	public override bool ImmediatelyRemovedOnApply => true;
	public override bool ShouldShowOnFigure => false;
	protected override string ConditionAnimationScenePath => "res://Scenes/Scenario/ConditionAnimations/BlessAnimation.tscn";

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		GameController.Instance.AMDManager.Bless(condition.Owner);
	}

	protected override string GetTooltipText(RichTextParameters textParameters) =>
		$"The affected figure must shuffle a bless card into their attack modifier deck. If the figure does not use an attack modifier deck, bless has no effect. When a bless card is drawn, it acts as a {Icons.Inline(Icons.GetAMDValue("2x"), textParameters, true)} modifier and is returned to the supply once resolved, instead of placed in the discard pile. There are 10 bless cards, which can be added to any deck. If there are no bless cards available, this condition has no effect.";
}