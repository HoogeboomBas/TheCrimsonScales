using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class PersonalQuestProgressUpdateViewItem : Control
{
	private const float InitialPosX = 600f;

	[Export]
	private Control _container;
	[Export]
	private PersonalQuestProgressView _personalQuestProgressView;

	public void Init(ClassModel classModel, PersonalQuestModel personalQuestModel, PersonalQuestData data)
	{
		_personalQuestProgressView.Init(classModel, personalQuestModel, data);

		_container.SetPosition(new Vector2(InitialPosX, _container.Position.Y));

		GTweenSequenceBuilder.New()
			.Append(_container.TweenPositionX(0f, 0.8f).SetEasing(Easing.OutBack))
			.AppendTime(2f)
			.Append(_container.TweenPositionX(InitialPosX, 0.5f).SetEasing(Easing.InBack))
			.AppendCallback(QueueFree)
			.Build().Play();
	}
}