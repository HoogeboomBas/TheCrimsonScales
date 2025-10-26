using System.Collections.Generic;
using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class ScenarioFlowchart : BetweenScenariosAction
{
	private static readonly Vector2I[] DirectionOffsets = [Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left];

	[Export]
	public Node OutlineParent { get; private set; }

	[Export]
	private GridContainer _gridContainer;

	[Export]
	private SubViewportContainer _3dContainer;
	[Export]
	private MeshInstance3D _scrollMesh;
	[Export]
	private SubViewportContainer _subViewportContainer;
	[Export]
	private SubViewport _subViewport;

	[Export]
	private Node3D _3dRoot;

	private bool _animating;

	private Dictionary<int, ScenarioButton> _scenarioButtons = new Dictionary<int, ScenarioButton>();
	private Dictionary<Vector2I, ScenarioButton> _scenarioButtonsByCoords = new Dictionary<Vector2I, ScenarioButton>();

	public int ColumnCount => _gridContainer.Columns;
	public float GridScale => _gridContainer.Scale.X;

	protected override bool SelectCharacter => false;
	protected override bool CustomTransitioning => _animating;

	public override void _EnterTree()
	{
		base._EnterTree();

		foreach(ScenarioButton scenarioButton in this.GetChildrenOfType<ScenarioButton>())
		{
			if(scenarioButton.Model != null)
			{
				_scenarioButtons.Add(scenarioButton.Model.ScenarioNumber, scenarioButton);
			}
		}
	}

	public override void _Ready()
	{
		base._Ready();

		SavedCampaign savedCampaign = BetweenScenariosController.Instance.SavedCampaign;

		// Make sure scenario 1 is unlocked
		// if(!savedCampaign.SavedScenarioProgresses.ScenarioProgresses.TryGetValue(ModelDB.GetId<Scenario001>().ToString(), out SavedScenarioProgress savedScenarioProgress))
		// {
		// 	savedScenarioProgress = new SavedScenarioProgress()
		// 	{
		// 		Discovered = true,
		// 		Unlocked = true,
		// 		//Completed = true
		// 	};
		// 	savedCampaign.SavedScenarioProgresses.ScenarioProgresses.Add(ModelDB.GetId<Scenario001>().ToString(), savedScenarioProgress);
		// }

		_subViewportContainer.SetVisible(false);

		CallDeferred(MethodName.Init);
	}

	private void Init()
	{
		foreach((int number, ScenarioButton scenarioButton) in _scenarioButtons)
		{
			int index = scenarioButton.GetIndex();
			//int index = number - 1;
			Vector2I coords = new Vector2I(index % ColumnCount, index / ColumnCount);
			scenarioButton.Init(coords);
			_scenarioButtonsByCoords.Add(coords, scenarioButton);
		}

		foreach((int number, ScenarioButton scenarioButton) in _scenarioButtons)
		{
			foreach(ScenarioFlowchartArrow arrow in scenarioButton.Arrows)
			{
				arrow.SetVisible(arrow.To.SavedScenarioProgress.Discovered);
			}
		}

		foreach((int key, ScenarioButton scenarioButton) in _scenarioButtons)
		{
			if(!scenarioButton.SavedScenarioProgress.Discovered)
			{
				continue;
			}

			for(int i = 0; i < DirectionOffsets.Length; i++)
			{
				Vector2I directionOffset = DirectionOffsets[i];
				Vector2I neighbourCoords = scenarioButton.Coords + directionOffset;
				if(_scenarioButtonsByCoords.TryGetValue(neighbourCoords, out ScenarioButton neighbour) &&
				   neighbour.SavedScenarioProgress.Discovered &&
				   scenarioButton.Model.ScenarioChain.BaseScenarioChain == neighbour.Model.ScenarioChain.BaseScenarioChain)
				{
					scenarioButton.ScenarioButtonOutline.AnimateDirectionalExtension(i, true);

					Vector2I otherDirectionOffset = DirectionOffsets[(i + 1) % 4];
					Vector2I otherNeighbourCoords = scenarioButton.Coords + otherDirectionOffset;
					Vector2I diagonalNeighbourCoords = neighbourCoords + otherDirectionOffset;

					if(_scenarioButtonsByCoords.TryGetValue(otherNeighbourCoords, out ScenarioButton otherNeighbour) &&
					   otherNeighbour.SavedScenarioProgress.Discovered &&
					   scenarioButton.Model.ScenarioChain.BaseScenarioChain == otherNeighbour.Model.ScenarioChain.BaseScenarioChain &&
					   _scenarioButtonsByCoords.TryGetValue(diagonalNeighbourCoords, out ScenarioButton diagonalNeighbour) &&
					   diagonalNeighbour.SavedScenarioProgress.Discovered &&
					   scenarioButton.Model.ScenarioChain.BaseScenarioChain == diagonalNeighbour.Model.ScenarioChain.BaseScenarioChain)
					{
						scenarioButton.ScenarioButtonOutline.AnimateDiagonalExtension(i, true);
					}
				}
			}
		}
	}

	public ScenarioButton GetScenarioButton(int scenarioNumber)
	{
		return _scenarioButtons[scenarioNumber];
	}

	protected override void AnimateIn(GTweenSequenceBuilder sequenceBuilder)
	{
		_3dRoot.SetVisible(true);

		AppController.Instance.AudioController.Play("res://Audio/SFX/ScenarioFlowchart/UnfurlMap.wav", delay: 0.4f);

		_subViewportContainer.SetVisible(false);
		_3dContainer.SetVisible(true);
		_3dContainer.Position = new Vector2(-1000f, 0f);
		_3dContainer.RotationDegrees = 50;

		base.AnimateIn(sequenceBuilder);

		_subViewport.SetUpdateMode(SubViewport.UpdateMode.Once);
		//_subViewportContainer.SetVisible(true);

		ShaderMaterial shaderMaterial = (ShaderMaterial)_scrollMesh.MaterialOverride;
		shaderMaterial.SetShaderParameter("progress", 0f);

		sequenceBuilder
			.Append(_3dContainer.TweenPosition(new Vector2(0f, 0f), 0.6f))
			.Join(_3dContainer.TweenRotationDegrees(0f, 0.6f))
			.Append(CustomGTweenExtensions.Tween(t => shaderMaterial.SetShaderParameter("progress", t), 0.7f))
			.AppendCallback(() =>
			{
				_subViewport.SetUpdateMode(SubViewport.UpdateMode.WhenVisible);
				_subViewportContainer.SetVisible(true);
				_3dContainer.SetVisible(false);

				_animating = true;
				AnimationSequence().Forget();
			});
	}

	protected override void AfterAnimateIn()
	{
		base.AfterAnimateIn();
	}

	protected override void AnimateOut(GTweenSequenceBuilder sequenceBuilder)
	{
		AppController.Instance.AudioController.Play("res://Audio/SFX/ScenarioFlowchart/FurlMap.wav", delay: 0.0f);

		_subViewportContainer.SetVisible(false);
		_3dContainer.SetVisible(true);

		base.AnimateOut(sequenceBuilder);

		ShaderMaterial shaderMaterial = (ShaderMaterial)_scrollMesh.MaterialOverride;

		sequenceBuilder
			.AppendCallback(() => _subViewport.SetUpdateMode(SubViewport.UpdateMode.Once))
			.Append(CustomGTweenExtensions.Tween(t => shaderMaterial.SetShaderParameter("progress", 1 - t), 0.5f))
			.Append(_3dContainer.TweenPosition(new Vector2(-1000f, 0f), 0.6f))
			.Join(_3dContainer.TweenRotationDegrees(50f, 0.6f))
			.AppendCallback(() =>
			{
				_3dContainer.SetVisible(false);
			});
	}

	protected override void AfterAnimateOut()
	{
		base.AfterAnimateOut();

		_3dRoot.SetVisible(true);
	}

	private async GDTaskVoid AnimationSequence()
	{
		CancellationToken cancellationToken = BetweenScenariosController.Instance.DestroyCancellationToken;

		foreach((int key, ScenarioButton scenarioButton) in _scenarioButtons)
		{
			if(scenarioButton.SavedScenarioProgress.Completed)
			{
				foreach(ScenarioFlowchartArrow arrow in scenarioButton.Arrows)
				{
					ScenarioButton destination = arrow.To;
					if(!destination.SavedScenarioProgress.Discovered)
					{
						// Animate in the background outline
						destination.ScenarioButtonOutline.AnimateIn();
						await GDTask.Delay(0.7f, cancellationToken: cancellationToken);

						// Animate in the scenario number
						destination.AnimateIn();
						await GDTask.Delay(1.2f, cancellationToken: cancellationToken);

						// Merge the outline
						for(int i = 0; i < DirectionOffsets.Length; i++)
						{
							Vector2I directionOffset = DirectionOffsets[i];
							Vector2I neighbourCoords = destination.Coords + directionOffset;
							if(_scenarioButtonsByCoords.TryGetValue(neighbourCoords, out ScenarioButton neighbour) &&
							   neighbour.SavedScenarioProgress.Discovered &&
							   destination.Model.ScenarioChain.BaseScenarioChain == neighbour.Model.ScenarioChain.BaseScenarioChain)
							{
								destination.ScenarioButtonOutline.AnimateDirectionalExtension(i);
								neighbour.ScenarioButtonOutline.AnimateDirectionalExtension((i + 2) % 4);
							}
						}

						for(int i = 0; i < DirectionOffsets.Length; i++)
						{
							Vector2I directionOffset = DirectionOffsets[i];
							Vector2I neighbourCoords = destination.Coords + directionOffset;
							Vector2I otherDirectionOffset = DirectionOffsets[(i + 1) % 4];
							Vector2I otherNeighbourCoords = destination.Coords + otherDirectionOffset;
							Vector2I diagonalNeighbourCoords = neighbourCoords + otherDirectionOffset;

							if(_scenarioButtonsByCoords.TryGetValue(neighbourCoords, out ScenarioButton neighbour) &&
							   neighbour.SavedScenarioProgress.Discovered &&
							   destination.Model.ScenarioChain.BaseScenarioChain == neighbour.Model.ScenarioChain.BaseScenarioChain &&
							   _scenarioButtonsByCoords.TryGetValue(otherNeighbourCoords, out ScenarioButton otherNeighbour) &&
							   otherNeighbour.SavedScenarioProgress.Discovered &&
							   destination.Model.ScenarioChain.BaseScenarioChain == otherNeighbour.Model.ScenarioChain.BaseScenarioChain &&
							   _scenarioButtonsByCoords.TryGetValue(diagonalNeighbourCoords, out ScenarioButton diagonalNeighbour) &&
							   diagonalNeighbour.SavedScenarioProgress.Discovered &&
							   destination.Model.ScenarioChain.BaseScenarioChain == diagonalNeighbour.Model.ScenarioChain.BaseScenarioChain)
							{
								destination.ScenarioButtonOutline.AnimateDiagonalExtension(i);
								neighbour.ScenarioButtonOutline.AnimateDiagonalExtension((i + 1) % 4);
								otherNeighbour.ScenarioButtonOutline.AnimateDiagonalExtension((i + 3) % 4);
								diagonalNeighbour.ScenarioButtonOutline.AnimateDiagonalExtension((i + 2) % 4);
							}
						}

						await GDTask.Delay(0.7f, cancellationToken: cancellationToken);

						// Show arrow visual
						arrow.AnimateIn();

						destination.SavedScenarioProgress.Discovered = true;

						await GDTask.Delay(1f, cancellationToken: cancellationToken);
					}
				}
			}
		}

		_animating = false;
	}
}