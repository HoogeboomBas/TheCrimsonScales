using Godot;

public partial class ClassToggleButton : ToggleButton<ClassToggleButton>
{
	[Export]
	private ClassView _classView;

	public ClassModel ClassModel { get; private set; }

	public void Init(ClassModel classModel)
	{
		ClassModel = classModel;

		_classView.Init(ClassModel);

		base.Init();
	}
}