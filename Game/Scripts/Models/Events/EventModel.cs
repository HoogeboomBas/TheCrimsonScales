public abstract class EventModel<TChoiceA, TChoiceB> : EventModel
	where TChoiceA : EventChoiceModel
	where TChoiceB : EventChoiceModel
{
	public override EventChoiceModel[] EventChoiceModels => [ModelDB.EventChoice<TChoiceA>(), ModelDB.EventChoice<TChoiceB>()];
}

public abstract class EventModel : AbstractModel<EventModel>
{
	public abstract EventType EventType { get; }
	public abstract int Number { get; }
	public abstract string Text { get; }

	public abstract EventChoiceModel[] EventChoiceModels { get; }
}