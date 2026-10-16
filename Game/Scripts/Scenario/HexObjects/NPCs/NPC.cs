using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public partial class NPC : Figure, IEventSubscriber
{
	private Sprite2D _sprite;
	private string _name;
	private readonly List<Ability> _abilities = new List<Ability>();
	private AMDCardDeck _amdCardDeckOverride;

	private ActionState _turnActionState;

	public string AssetPath { get; private set; }
	public Initiative PermanentInitiative { get; private set; }

	public override string DisplayName => _name;
	public override string DebugName => _name;
	public override AMDCardDeck AMDCardDeck => _amdCardDeckOverride ?? GameController.Instance.MonsterAMDCardDeck;
	public virtual Texture2D PortraitTexture => ResourceLoader.Load<Texture2D>($"{AssetPath}/Portrait.tres");
	public override Texture2D MapIconTexture => ResourceLoader.Load<Texture2D>($"{AssetPath}/MapIcon.tres");
	public override Node2D Visual => _sprite;

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		_sprite = GetNode<Sprite2D>("Mask/Sprite2D");
	}

	public async GDTask Spawn(int health, string name, string assetPath, int initiative, List<Ability> abilities,
		TextHelper.LabelTextDelegate actionText, Alignment alignment)
	{
		_name = $"{name} - NPC";
		PermanentInitiative = new Initiative()
		{
			MainInitiative = initiative,
			SortingInitiative = initiative * 10000000 + 8000000
		};
		AssetPath = assetPath;

		_outline.SetSelfModulate(Color.FromHtml("1778ff"));
		FigureViewComponent.TurnStartPS.SetSelfModulate(Color.FromHtml("1778ff"));
		FigureViewComponent.ActivePS.SetModulate(Color.FromHtml("1778ff"));

		_sprite.SetTexture(MapIconTexture);
		float textureWidth = MapIconTexture.GetWidth();
		_sprite.SetScale(250f / textureWidth * Vector2.One);

		SetMaxHealth(health);
		SetHealth(health);

		SetAlignment(alignment);

		await GameController.Instance.Map.RegisterFigure(this);

		UpdateInitiative();

		_abilities.AddRange(abilities);

		if(actionText != null)
		{
			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
				parameters => parameters.Figure == this,
				parameters =>
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(textParameters => $"Performs:\n{actionText(textParameters)}"));
				}
			);
		}
	}

	protected override async GDTask TakeTurn()
	{
		await base.TakeTurn();

		_turnActionState = new ActionState(this, this, _abilities);
		await _turnActionState.Perform();
	}

	public async GDTask RemoveTurnActionFromActive()
	{
		if(_turnActionState != null)
		{
			await _turnActionState.RemoveFromActive();
		}
	}

	public override async GDTask Destroy(AbilityState potentialAbilityState = null,
		Figure potentialDestroyer = null, bool immediately = false, bool forceDestroy = false)
	{
		await RemoveTurnActionFromActive();
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(this);

		await base.Destroy(potentialAbilityState, potentialDestroyer, immediately, forceDestroy);
	}

	protected override Initiative GetInitiative()
	{
		return PermanentInitiative;
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new NPCInfoItem.Parameters(this));
	}

	public void SetAMDCardDeck(AMDCardDeck amdCardDeck)
	{
		_amdCardDeckOverride = amdCardDeck;
	}
}