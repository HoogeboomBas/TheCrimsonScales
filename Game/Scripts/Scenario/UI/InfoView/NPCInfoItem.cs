public partial class NPCInfoItem : FigureInfoItem<NPCInfoItem.Parameters>
{
	public class Parameters(NPC hexObject) : FigureInfoItemParameters(hexObject)
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/NPCInfoItem.tscn";

		public NPC NPC { get; } = hexObject;
	}

	private NPC _npc;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_npc = parameters.NPC;

		_portraitTexture.SetTexture(_npc.PortraitTexture);
		_portraitBorder.SetSelfModulate(_npc.OutlineColor);
	}
}