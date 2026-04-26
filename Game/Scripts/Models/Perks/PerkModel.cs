using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public abstract class PerkModel : AbstractModel, IEventSubscriber
{
	public virtual string ToString(RichTextParameters richTextParameters)
	{
		if(!string.IsNullOrEmpty(Title))
		{
			return GetNonAMDString(richTextParameters);
		}

		string returnValue = string.Empty;

		if(IgnoreScenarioEffects)
		{
			returnValue += "ignore scenario effects";
		}

		if(IgnoreItemMinusOneEffects)
		{
			if(!string.IsNullOrEmpty(returnValue))
			{
				returnValue += " and ";
			}

			returnValue += $"ignore item {Icons.Inline(Icons.MinusOneCard, richTextParameters)} effects";
		}

		if(CardsToRemove.Count > 0)
		{
			if(!string.IsNullOrEmpty(returnValue))
			{
				returnValue += " and ";
			}

			if(CardsToAdd.Count > 0)
			{
				returnValue += "replace ";
			}
			else
			{
				returnValue += "remove ";
			}

			returnValue += GetCardsString(CardsToRemove, richTextParameters);

			if(CardsToAdd.Count > 0)
			{
				returnValue += $" with {GetCardsString(CardsToAdd, richTextParameters)}";
			}
		}
		else if(CardsToAdd.Count > 0)
		{
			if(!string.IsNullOrEmpty(returnValue))
			{
				returnValue += " and ";
			}

			returnValue += $"add {GetCardsString(CardsToAdd, richTextParameters)}";
		}

		if(string.IsNullOrEmpty(returnValue))
		{
			return returnValue;
		}

		returnValue = string.Concat(returnValue[0].ToString().ToUpper(), returnValue.AsSpan(1));
		return returnValue;
	}

	public virtual int PerkBoxCount => 1;

	protected virtual string Title => null;
	public virtual string GetNonAMDDescription(RichTextParameters richTextParameters) => null;

	public virtual List<AMDCardModel> CardsToRemove => [];
	public virtual List<AMDCardModel> CardsToAdd => [];

	public virtual bool IgnoreScenarioEffects => false;
	public virtual bool IgnoreItemMinusOneEffects => false;

	public virtual async GDTask OnScenarioSetupPhaseCompleted(Character character)
	{
		if(IgnoreScenarioEffects)
		{
			ScenarioCheckEvents.ApplyScenarioEffectsCheckEvent.Subscribe(this,
				parameters => parameters.Character == character,
				parameters =>
				{
					parameters.SetIgnoreScenarioEffects();
				});
		}

		await GDTask.CompletedTask;
	}

	public virtual void OnPerkAcquired(SavedCharacter savedCharacter)
	{
	}

	private string GetNonAMDString(RichTextParameters richTextParameters)
	{
		return $"[b]{Title}:[/b] {GetNonAMDDescription(richTextParameters)}";
	}

	private string GetCardsString(List<AMDCardModel> cards, RichTextParameters richTextParameters)
	{
		string returnValue = string.Empty;

		IEnumerable<IGrouping<AMDCardModel, AMDCardModel>> amdCardGroups = cards.GroupBy(perkModel => perkModel);
		foreach(IGrouping<AMDCardModel, AMDCardModel> amdCardGroup in amdCardGroups)
		{
			if(!string.IsNullOrEmpty(returnValue))
			{
				returnValue += " and ";
			}

			int count = amdCardGroup.Count();
			string number;
			switch(count)
			{
				case 1:
					number = "one";
					break;
				case 2:
					number = "two";
					break;
				case 3:
					number = "three";
					break;
				case 4:
					number = "four";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(count), count, null);
			}

			returnValue += $"{number} {amdCardGroup.Key.ToString(richTextParameters)} ";
			returnValue += count > 1 ? "cards" : "card";
		}

		return returnValue;
	}
}