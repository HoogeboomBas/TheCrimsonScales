public abstract class CityEventModel<TChoiceA, TChoiceB> : EventModel<TChoiceA, TChoiceB>
	where TChoiceA : EventChoiceModel
	where TChoiceB : EventChoiceModel
{
	public override EventType EventType => EventType.City;
}