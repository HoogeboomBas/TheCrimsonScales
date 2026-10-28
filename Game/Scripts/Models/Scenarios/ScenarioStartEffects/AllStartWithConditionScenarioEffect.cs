using System.Linq;
using Fractural.Tasks;

public class AllStartWithConditionScenarioEffect : ScenarioEffect
{
	private readonly ConditionModel[] _conditionModels;

	public AllStartWithConditionScenarioEffect(params ConditionModel[] conditionModels)
	{
		_conditionModels = conditionModels;
	}

	public override string GetDescription(RichTextParameters textParameters) =>
		$"All characters gain {Icons.InlineConditions(_conditionModels, textParameters)}.";

	public override async GDTask Activate(Character character)
	{
		await AbilityCmd.AddConditions(null, character, _conditionModels.ToList());
	}
}