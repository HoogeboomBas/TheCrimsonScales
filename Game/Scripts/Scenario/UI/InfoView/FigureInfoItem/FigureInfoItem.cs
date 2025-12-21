using System.Collections.Generic;
using System.Text;
using Godot;

public abstract class FigureInfoItemParameters(Figure hexObject) : InfoItemParameters<Figure>(hexObject)
{
}

public abstract partial class FigureInfoItem<T> : InfoItem<T>
	where T : FigureInfoItemParameters
{
	protected TextureRect _portraitTexture;
	protected Control _portraitBorder;

	private Label _healthLabel;

	private Control _simpleEffectsContainer;
	private RichTextLabel _simpleEffectsLabel;

	private FigureInfoConditionsEffect _conditions;
	private FigureInfoConditionsEffect _immunities;
	private FigureInfoConditionsEffect _applies;

	private InfoExtraEffectsView _infoExtraEffectsView;

	private Figure _figure;

	public override void Init(T parameters)
	{
		base.Init(parameters);

		_portraitTexture = GetNode<TextureRect>("MarginContainer/Content/Container/Portrait/MarginContainer/Portrait/TextureRect");
		_portraitBorder = GetNode<Control>("MarginContainer/Content/Container/Portrait");

		_healthLabel = GetNode<Label>("MarginContainer/Content/Container/Stats/Health/Label");

		_simpleEffectsContainer = GetNode<Control>("MarginContainer/Content/Effects/SimpleEffects");
		_simpleEffectsLabel = GetNode<RichTextLabel>("MarginContainer/Content/Effects/SimpleEffects/RichTextLabel");

		_conditions = GetNode<FigureInfoConditionsEffect>("MarginContainer/Content/Effects/Conditions");
		_immunities = GetNode<FigureInfoConditionsEffect>("MarginContainer/Content/Effects/Immunities");
		_applies = GetNode<FigureInfoConditionsEffect>("MarginContainer/Content/Effects/Applies");

		_infoExtraEffectsView = GetNode<InfoExtraEffectsView>("MarginContainer/Content/Effects/InfoExtraEffectsView");

		_figure = parameters.HexObject;

		_titleLabel.SetText(_figure.DisplayName);

		UpdateHealth();

		_figure.MaxHealthChangedEvent += OnMaxHealthChanged;
		_figure.HealthChangedEvent += OnHealthChanged;
		_figure.ConditionsChangedEvent += OnConditionsChanged;

		OnConditionsChanged(_figure);

		ScenarioCheckEvents.TargetsCheckEvent.SubscribersChangedEvent += OnTargetsSubscriptionsChanged;
		ScenarioCheckEvents.ShieldCheckEvent.SubscribersChangedEvent += OnShieldSubscriptionsChanged;
		ScenarioCheckEvents.RetaliateCheckEvent.SubscribersChangedEvent += OnRetaliateSubscriptionsChanged;
		ScenarioCheckEvents.PierceCheckEvent.SubscribersChangedEvent += OnPierceSubscriptionsChanged;
		ScenarioCheckEvents.FlyingCheckEvent.SubscribersChangedEvent += OnFlyingSubscriptionsChanged;

		UpdateSimpleEffects();

		ScenarioCheckEvents.ImmunitiesVisualCheckEvent.SubscribersChangedEvent += OnImmunitiesVisualSubscriptionsChanged;
		ScenarioCheckEvents.AppliesVisualCheckEvent.SubscribersChangedEvent += OnAppliesVisualSubscriptionsChanged;
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.SubscribersChangedEvent += OnFigureInfoItemExtraEffectsSubscriptionsChanged;

		OnImmunitiesVisualSubscriptionsChanged();
		OnAppliesVisualSubscriptionsChanged();
		OnFigureInfoItemExtraEffectsSubscriptionsChanged();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(_figure != null)
		{
			_figure.MaxHealthChangedEvent -= OnMaxHealthChanged;
			_figure.HealthChangedEvent -= OnHealthChanged;
			_figure.ConditionsChangedEvent -= OnConditionsChanged;
		}

		if(GameController.Instance != null)
		{
			ScenarioCheckEvents.TargetsCheckEvent.SubscribersChangedEvent -= OnTargetsSubscriptionsChanged;
			ScenarioCheckEvents.ShieldCheckEvent.SubscribersChangedEvent -= OnShieldSubscriptionsChanged;
			ScenarioCheckEvents.RetaliateCheckEvent.SubscribersChangedEvent -= OnRetaliateSubscriptionsChanged;
			ScenarioCheckEvents.PierceCheckEvent.SubscribersChangedEvent -= OnPierceSubscriptionsChanged;
			ScenarioCheckEvents.FlyingCheckEvent.SubscribersChangedEvent -= OnFlyingSubscriptionsChanged;

			ScenarioCheckEvents.ImmunitiesVisualCheckEvent.SubscribersChangedEvent -= OnImmunitiesVisualSubscriptionsChanged;
			ScenarioCheckEvents.AppliesVisualCheckEvent.SubscribersChangedEvent -= OnAppliesVisualSubscriptionsChanged;
			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.SubscribersChangedEvent -= OnFigureInfoItemExtraEffectsSubscriptionsChanged;
		}
	}

	private void UpdateHealth()
	{
		_healthLabel.Text = $"{_figure.Health}/{_figure.MaxHealth}";
	}

	private void UpdateSimpleEffects()
	{
		int addedIconCount = 0;

		StringBuilder stringBuilder = new StringBuilder();

		ScenarioCheckEvents.TargetsCheck.Parameters targetsCheckParameters =
			ScenarioCheckEvents.TargetsCheckEvent.Fire(new ScenarioCheckEvents.TargetsCheck.Parameters(_figure));
		if(targetsCheckParameters.Targets > 1)
		{
			AppendIconValue(Icons.Targets, targetsCheckParameters.Targets);
		}

		ScenarioCheckEvents.ShieldCheck.Parameters shieldCheckParameters =
			ScenarioCheckEvents.ShieldCheckEvent.Fire(new ScenarioCheckEvents.ShieldCheck.Parameters(_figure));
		if(shieldCheckParameters.Shield > 0)
		{
			AppendIconValue(Icons.Shield, shieldCheckParameters.Shield);
		}

		ScenarioCheckEvents.RetaliateCheck.Parameters retaliateCheckParameters =
			ScenarioCheckEvents.RetaliateCheckEvent.Fire(new ScenarioCheckEvents.RetaliateCheck.Parameters(_figure));
		if(retaliateCheckParameters.RetaliateValues.Count > 0)
		{
			int finalRetaliate = 0;
			foreach((int retaliate, int range) in retaliateCheckParameters.RetaliateValues)
			{
				finalRetaliate += retaliate;
			}

			AppendIconValue(Icons.Retaliate, finalRetaliate);
		}

		ScenarioCheckEvents.PierceCheck.Parameters pierceCheckParameters =
			ScenarioCheckEvents.PierceCheckEvent.Fire(new ScenarioCheckEvents.PierceCheck.Parameters(_figure));
		if(pierceCheckParameters.Pierce > 0)
		{
			AppendIconValue(Icons.Pierce, pierceCheckParameters.Pierce);
		}

		_simpleEffectsContainer.SetVisible(addedIconCount > 0);
		_simpleEffectsLabel.SetText(stringBuilder.ToString());

		void AppendIconValue(string iconPath, int value)
		{
			if(addedIconCount > 0)
			{
				stringBuilder.Append(", ");
			}

			stringBuilder.Append($"{Icons.Inline(iconPath, 40)}{value}");

			addedIconCount++;
		}
	}

	private void OnMaxHealthChanged(Figure figure)
	{
		UpdateHealth();
	}

	private void OnHealthChanged(Figure figure)
	{
		UpdateHealth();
	}

	private void OnConditionsChanged(Figure figure)
	{
		_conditions.SetConditions(figure.Conditions);
	}

	private void OnTargetsSubscriptionsChanged()
	{
		UpdateSimpleEffects();
	}

	private void OnShieldSubscriptionsChanged()
	{
		UpdateSimpleEffects();
	}

	private void OnRetaliateSubscriptionsChanged()
	{
		UpdateSimpleEffects();
	}

	private void OnPierceSubscriptionsChanged()
	{
		UpdateSimpleEffects();
	}

	private void OnFlyingSubscriptionsChanged()
	{
	}

	private void OnImmunitiesVisualSubscriptionsChanged()
	{
		ScenarioCheckEvents.ImmunitiesVisualCheck.Parameters immunitiesVisualCheckParameters =
			ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Fire(new ScenarioCheckEvents.ImmunitiesVisualCheck.Parameters(_figure));
		_immunities.SetConditions(immunitiesVisualCheckParameters.Immunities);
	}

	private void OnAppliesVisualSubscriptionsChanged()
	{
		ScenarioCheckEvents.AppliesVisualCheck.Parameters appliesVisualCheckParameters =
			ScenarioCheckEvents.AppliesVisualCheckEvent.Fire(new ScenarioCheckEvents.AppliesVisualCheck.Parameters(_figure));
		_applies.SetConditions(appliesVisualCheckParameters.ConditionModels);
	}

	private void OnFigureInfoItemExtraEffectsSubscriptionsChanged()
	{
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheck.Parameters figureInfoItemExtraEffectsCheckParameters =
			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Fire(
				new ScenarioCheckEvents.FigureInfoItemExtraEffectsCheck.Parameters(_figure));

		_infoExtraEffectsView.Update(figureInfoItemExtraEffectsCheckParameters.InfoExtraEffectsParameters);
	}
}