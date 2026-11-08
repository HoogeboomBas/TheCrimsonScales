using Fractural.Tasks;

public class Curse : ConditionModel
{
	public override string Name => "Curse";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Curse.svg";
	public override ConditionPolarity ConditionPolarity => ConditionPolarity.Negative;
	public override bool CanBeAppliedMultipleTimesOnSingleTarget => true;
	public override bool ImmediatelyRemovedOnApply => true;
	public override bool ShouldShowOnFigure => false;
	protected override string ConditionAnimationScenePath => "res://Scenes/Scenario/ConditionAnimations/CurseAnimation.tscn";

	public override async GDTask OnAdded(Condition condition)
	{
		await base.OnAdded(condition);

		GameController.Instance.AMDManager.Curse(condition.Owner);
	}

	protected override string GetTooltipText(RichTextParameters textParameters) =>
		$"The affected figure must shuffle a curse card into their attack modifier deck. If the figure does not use an attack modifier deck, this condition has no effect. When a curse card is drawn, it acts as a {Icons.Inline(Icons.GetAMDValue("null"), textParameters, true)} modifier and is returned to the supply once resolved, instead of placed in the discard pile. There are 10 curse cards with the which can only be added to character decks, and 10 curse cards which can only be added to the monster deck. If there are no applicable curse cards available, this condition has no effect. Immunity to this condition prevents a figure from adding a curse card to their deck, but does not prevent a drawn curse card from taking effect.";
}