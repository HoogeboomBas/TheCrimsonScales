using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PortraitView : Control
{
	[Export]
	private PackedScene _characterPortraitScene;
	[Export]
	private PackedScene _monsterGroupPortraitScene;
	[Export]
	private PackedScene _npcPortraitScene;
	[Export]
	private Control _portraitParent;

	public List<PortraitViewPortrait> Portraits { get; } = new List<PortraitViewPortrait>();
	public List<PortraitViewCharacterPortrait> CharacterPortraits { get; } = new List<PortraitViewCharacterPortrait>();
	public List<PortraitViewMonsterGroupPortrait> MonsterGroupPortraits { get; } = new List<PortraitViewMonsterGroupPortrait>();
	public List<PortraitViewNPCPortrait> NPCPortraits { get; } = new List<PortraitViewNPCPortrait>();

	public void Open()
	{
		GameController.Instance.Map.FigureAddedEvent += OnFigureAdded;
		GameController.Instance.Map.FigureRemovedEvent += OnFigureRemoved;

		foreach(Figure figure in GameController.Instance.Map.Figures)
		{
			OnFigureAdded(figure);
		}
	}

	public void Close()
	{
		foreach(PortraitViewPortrait portrait in Portraits)
		{
			portrait.Destroy();
		}

		Portraits.Clear();

		GameController.Instance.Map.FigureAddedEvent -= OnFigureAdded;
		GameController.Instance.Map.FigureRemovedEvent -= OnFigureRemoved;
	}

	public void Reorder()
	{
		Portraits.Sort((portraitA, portraitB) =>
		{
			return portraitA.Initiative.SortingInitiative.CompareTo(portraitB.Initiative.SortingInitiative);
		});

		const float separation = 20f;

		for(int i = 0; i < Portraits.Count; i++)
		{
			PortraitViewPortrait portrait = Portraits[i];

			float portraitWidth = portrait.Size.X;
			float parentWidth = _portraitParent.Size.X;

			float totalWidth = (portraitWidth + separation) * Portraits.Count - separation;
			float totalLeftAnchor = (parentWidth - totalWidth) / 2f;

			float pos = totalLeftAnchor + (portraitWidth + separation) * i;
			portrait.Move(new Vector2(pos, portrait.Position.Y));
		}
	}

	public PortraitViewPortrait CreatePortrait(Figure figure)
	{
		if(figure is Monster monster)
		{
			PortraitViewMonsterGroupPortrait portrait = _monsterGroupPortraitScene.Instantiate<PortraitViewMonsterGroupPortrait>();
			portrait.Init(monster.MonsterGroup);

			return portrait;
		}
		else if(figure is Character character)
		{
			PortraitViewCharacterPortrait portrait = _characterPortraitScene.Instantiate<PortraitViewCharacterPortrait>();
			portrait.Init(character);

			return portrait;
		}
		else if(figure is NPC npc)
		{
			PortraitViewNPCPortrait portrait = _npcPortraitScene.Instantiate<PortraitViewNPCPortrait>();
			portrait.Init(npc);

			return portrait;
		}

		return null;
	}

	private void OnFigureAdded(Figure figure)
	{
		if(figure is Monster monster)
		{
			PortraitViewMonsterGroupPortrait portrait =
				MonsterGroupPortraits.FirstOrDefault(portrait => portrait.MonsterGroup == monster.MonsterGroup);
			if(portrait == null)
			{
				portrait = (PortraitViewMonsterGroupPortrait)CreatePortrait(figure);
				_portraitParent.AddChild(portrait);
				Portraits.Add(portrait);
				MonsterGroupPortraits.Add(portrait);

				Reorder();
			}
		}
		else if(figure is Character character)
		{
			PortraitViewCharacterPortrait portrait = (PortraitViewCharacterPortrait)CreatePortrait(figure);
			_portraitParent.AddChild(portrait);
			Portraits.Add(portrait);
			CharacterPortraits.Add(portrait);

			Reorder();
		}
		else if(figure is NPC npc)
		{
			PortraitViewNPCPortrait portrait = (PortraitViewNPCPortrait)CreatePortrait(figure);
			_portraitParent.AddChild(portrait);
			Portraits.Add(portrait);
			NPCPortraits.Add(portrait);

			Reorder();
		}
	}

	private void OnFigureRemoved(Figure figure)
	{
		if(figure is Monster monster)
		{
			if(monster.MonsterGroup.Monsters.Count == 0)
			{
				PortraitViewMonsterGroupPortrait portrait =
					MonsterGroupPortraits.FirstOrDefault(portrait => portrait.MonsterGroup == monster.MonsterGroup);
				if(portrait != null)
				{
					Portraits.Remove(portrait);
					MonsterGroupPortraits.Remove(portrait);
					portrait.Destroy();

					Reorder();
				}
			}
		}
		else if(figure is Character character)
		{
			PortraitViewCharacterPortrait portrait = CharacterPortraits.FirstOrDefault(portrait => portrait.Character == character);

			if(portrait != null)
			{
				Portraits.Remove(portrait);
				CharacterPortraits.Remove(portrait);
				portrait.Destroy();

				Reorder();
			}
		}
		else if(figure is NPC npc)
		{
			PortraitViewNPCPortrait portrait = NPCPortraits.FirstOrDefault(portrait => portrait.NPC == npc);

			if(portrait != null)
			{
				Portraits.Remove(portrait);
				NPCPortraits.Remove(portrait);
				portrait.Destroy();

				Reorder();
			}
		}
	}
}