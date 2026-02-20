using Godot;

public partial class EnhancementMarkToggleButton : ToggleButton<EnhancementMarkToggleButton>
{
	private Vector2 _normalizedPosition;

	public EnhancementMark EnhancementMark { get; private set; }
	public bool Top { get; private set; }
	public int Index { get; private set; }

	public void Init(EnhancementMark enhancementMark, bool top, int index)
	{
		base.Init();

		EnhancementMark = enhancementMark;
		Top = top;
		Index = index;

		Control parent = GetParent<Control>();
		_normalizedPosition = enhancementMark.NormalizedPosition;
		SetPosition(_normalizedPosition * parent.Size - 0.5f * Size);
	}
}