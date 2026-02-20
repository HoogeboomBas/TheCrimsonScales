public abstract class TheCrimsonScalesPersonalQuest<T> : PersonalQuestModel<T>
	where T : PersonalQuestData, new()
{
	protected sealed override string TexturePath => "res://Content/PersonalQuests/PersonalQuests.jpg";
	protected sealed override int ColumnCount => 7;
	protected sealed override int RowCount => 4;
}