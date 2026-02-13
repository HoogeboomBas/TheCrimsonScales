using System.Collections.Generic;
using Fractural.Tasks;

public class ElementManager
{
	private readonly ElementState[] _states = new ElementState[6];
	private readonly List<Element> _infusing = new List<Element>();

	public ElementState GetState(Element element)
	{
		return _states[(int)element];
	}

	public void StartInfuse(Element element)
	{
		_infusing.AddIfNew(element);
		GameController.Instance.ElementsView.SetElementInfusing(element, true);
	}

	public async GDTask FinishInfusing()
	{
		foreach(Element element in _infusing)
		{
			SetState(element, ElementState.Strong);
			GameController.Instance.ElementsView.SetElementInfusing(element, false);
		}

		await ScenarioEvents.FinishElementInfusedEvent.CreatePrompt(
			new ScenarioEvents.FinishElementInfused.Parameters());

		_infusing.Clear();
	}

	public async GDTask InfuseImmediately(Element element)
	{
		SetState(element, ElementState.Strong);

		await ScenarioEvents.FinishElementInfusedEvent.CreatePrompt(
			new ScenarioEvents.FinishElementInfused.Parameters());
	}

	public void Consume(Element element)
	{
		SetState(element, ElementState.Inert);
	}

	public void WaneAll()
	{
		for(int i = 0; i < _states.Length; i++)
		{
			ElementState state = _states[i];

			if(state == ElementState.Waning)
			{
				SetState((Element)i, ElementState.Inert);
			}

			if(state == ElementState.Strong)
			{
				SetState((Element)i, ElementState.Waning);
			}
		}
	}

	public void SetState(Element element, ElementState state)
	{
		_states[(int)element] = state;
		GameController.Instance.ElementsView.SetElementState(element, state);
	}
}