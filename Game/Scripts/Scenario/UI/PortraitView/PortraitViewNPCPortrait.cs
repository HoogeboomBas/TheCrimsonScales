public partial class PortraitViewNPCPortrait : PortraitViewPortrait
{
	public NPC NPC { get; private set; }

	public override Initiative Initiative => NPC.Initiative;

	public void Init(NPC npc)
	{
		base.Init();

		NPC = npc;

		_portraitTexture.Texture = NPC.PortraitTexture;

		NPC.InitiativeChangedEvent += OnInitiativeChanged;

		OnInitiativeChanged(NPC);
	}

	public override void Destroy()
	{
		base.Destroy();

		if(NPC != null)
		{
			NPC.InitiativeChangedEvent -= OnInitiativeChanged;
		}
	}

	protected override void OnTurnTakerChanged(Figure figure)
	{
		base.OnTurnTakerChanged(figure);

		SetSelected(figure == NPC);
	}

	private void OnInitiativeChanged(Figure figure)
	{
		_initiativeLabel.Text = figure.Initiative.ToString();

		GameController.Instance.PortraitView.Reorder();
	}
}