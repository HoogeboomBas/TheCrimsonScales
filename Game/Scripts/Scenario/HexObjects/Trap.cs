using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public partial class Trap : OverlayTile
{
	[Export]
	public bool ScaledDamage { get; set; }

	[Export]
	public int CustomDamage { get; set; }

	[Export]
	public ConditionModelResource[] ConditionModels { get; set; }

	private readonly List<ConditionNode> _conditionNodes = new List<ConditionNode>();

	private TrapViewComponent _trapViewComponent;

	public int Damage => ScaledDamage ? 2 + GameController.Instance.SavedScenario.ScenarioLevel : CustomDamage;

	public override void _Ready()
	{
		base._Ready();

		_trapViewComponent = GetViewComponent<TrapViewComponent>();
	}

	public void SetTrapDamage(int damage)
	{
		ScaledDamage = false;
		CustomDamage = damage;
	}

	public void SetTrapValues(int damage, ConditionModel[] conditionModels)
	{
		SetTrapDamage(damage);

		ConditionModels = new ConditionModelResource[conditionModels.Length];
		for(int i = 0; i < conditionModels.Length; i++)
		{
			ConditionModel conditionModel = conditionModels[i];
			ConditionModelResource resource = new ConditionModelResource();
			resource.ModelIdString = conditionModel.Id.ToString();
			ConditionModels[i] = resource;
		}
	}

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		UpdateVisuals();
	}

	public async GDTask Trigger(AbilityState state, Figure figure)
	{
		int damage = Damage;
		if(damage > 0)
		{
			await AbilityCmd.SufferDamage(null, figure, damage);
		}

		if(ConditionModels != null)
		{
			foreach(ConditionModelResource conditionModelResource in ConditionModels)
			{
				await AbilityCmd.AddCondition(null, figure, conditionModelResource.Model);
			}
		}

		await Destroy();
	}

	public void UpdateVisuals()
	{
		int damage = Damage;
		_trapViewComponent.DamageContainer.SetVisible(damage > 0);
		_trapViewComponent.DamageLabel.Text = damage.ToString();
		_trapViewComponent.DamageLabel.Scale = (damage >= 10 ? 0.8f : 1f) * Vector2.One;
		_trapViewComponent.DamageContainer.Position = new Vector2(0f, ConditionModels == null || ConditionModels.Length == 0 ? 0f : 10f);

		foreach(ConditionNode conditionNode in _conditionNodes)
		{
			conditionNode.QueueFree();
		}

		_conditionNodes.Clear();

		if(ConditionModels != null)
		{
			foreach(ConditionModelResource conditionModelResource in ConditionModels)
			{
				ConditionNode conditionNode = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/Condition.tscn").Instantiate<ConditionNode>();
				_trapViewComponent.ConditionContainer.AddChild(conditionNode);
				conditionNode.Init(conditionModelResource.Model, true);
				_conditionNodes.Add(conditionNode);
			}
		}

		int conditionCount = _conditionNodes.Count;
		const float maxOffset = 80f;
		for(int i = 0; i < _conditionNodes.Count; i++)
		{
			ConditionNode conditionNode = _conditionNodes[i];
			float progress = (i + 1f) / (conditionCount + 1);
			float posX = Mathf.Lerp(-maxOffset, maxOffset, progress);
			conditionNode.Move(new Vector2(posX, 0f));
			_trapViewComponent.ConditionContainer.MoveChild(conditionNode, i);
		}

		_trapViewComponent.ConditionContainer.Position = new Vector2(0f, damage > 0 ? -60f : 0f);
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new TrapInfoItem.Parameters(this));
	}
}