﻿using System.Collections.Generic;
using Godot;

public partial class FigureInfoConditionsEffect : Control
{
	[Export]
	private PackedScene _iconScene;
	[Export]
	private Control _iconParent;

	private readonly List<FigureInfoIcon> _icons = new List<FigureInfoIcon>();

	public void SetConditions(List<ConditionModel> conditionModels)
	{
		SetVisible(conditionModels.Count > 0);

		this.DelayedCall(() =>
		{
			foreach(FigureInfoIcon icon in _icons)
			{
				icon.QueueFree();
			}

			_icons.Clear();

			float fullWidth = _iconParent.Size.X;
			const int iconWidth = 40;
			const float preferredSpacing = 5f;
			float workableWidth = fullWidth - iconWidth;
			float preferredWidth = iconWidth * conditionModels.Count + preferredSpacing * (conditionModels.Count - 1);

			for(int i = 0; i < conditionModels.Count; i++)
			{
				ConditionModel conditionModel = conditionModels[i];
				FigureInfoIcon figureInfoIcon = _iconScene.Instantiate<FigureInfoIcon>();
				_iconParent.AddChild(figureInfoIcon);
				figureInfoIcon.Init(conditionModel);
				_icons.Add(figureInfoIcon);

				if(preferredWidth > fullWidth)
				{
					float progress = (float)i / (conditionModels.Count - 1);
					float position = progress * workableWidth;
					figureInfoIcon.SetPosition(new Vector2(position, 0f));
				}
				else
				{
					// Align to the side
					//float position = workableWidth - (i * iconWidth);
					float offset = (fullWidth - preferredWidth) / 2;
					float position = i * (iconWidth + preferredSpacing) + offset;
					figureInfoIcon.SetPosition(new Vector2(position, 0f));
				}
			}
		});
	}
}