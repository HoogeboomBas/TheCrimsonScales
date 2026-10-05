using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public abstract class AMDCardModel : AbstractModel
{
	public virtual string ToString(RichTextParameters richTextParameters) =>
		GetBasicString(richTextParameters, Type, GetValue(null), conditionModels: GetConditionModels(null), rolling: GetRolling(null));

	public virtual string GetSimpleString(RichTextParameters richTextParameters) => GetBasicString(richTextParameters, Type, GetValue(null),
		conditionModels: GetConditionModels(null), rolling: GetRolling(null), isSimple: true);

	protected abstract string GetTexturePath(AMDCardOwner owner);
	protected abstract int AtlasIndex { get; }
	protected abstract int ColumnCount { get; }
	protected abstract int RowCount { get; }

	public virtual bool Reshuffles => false;
	public virtual bool RemoveAfterDraw => false;
	public virtual bool StaysActive => false;

	public virtual AMDCardType Type => AMDCardType.Value;

	public virtual bool GetRolling(AttackAbility.State attackAbilityState) => false;

	public virtual int? GetValue(AttackAbility.State attackAbilityState) => null;

	public virtual int? Pierce => null;
	public virtual int? Push => null;
	public virtual int? Pull => null;
	public virtual int? Swing => null;
	public virtual int? AddedTargets => null;

	public virtual List<CardElementInfusion> ElementInfusions => [];
	public virtual List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [];
	public virtual List<Ability> GetAbilities(AttackAbility.State attackAbilityState) => [];

	public virtual Func<AttackAbility.State, Character, GDTask> GetExtraEffects() => null;

	public Texture2D GetTexture(AMDCardOwner owner)
	{
		return AtlasTextureHelper.CreateAtlasTexture(
			AtlasIndex, ColumnCount, RowCount,
			ResourceLoader.Load<Texture2D>(GetTexturePath(owner)));
	}

	protected string GetBasicString(RichTextParameters richTextParameters, int value,
		List<ConditionModel> conditionModels = null, string extraText = null, bool rolling = false, string petals = null)
	{
		return GetBasicString(richTextParameters, AMDCardType.Value, value, conditionModels, extraText, rolling, petals);
	}

	protected string GetBasicString(RichTextParameters richTextParameters, AMDCardType cardType,
		List<ConditionModel> conditionModels = null, string extraText = null, bool rolling = false, string petals = null)
	{
		return GetBasicString(richTextParameters, cardType, null, conditionModels, extraText, rolling, petals);
	}

	protected string GetBasicString(RichTextParameters richTextParameters, AMDCardType cardType, int? value,
		List<ConditionModel> conditionModels = null, string extraText = null, bool rolling = false, string petals = null, bool isSimple = false)
	{
		string returnValue = string.Empty;
		if(petals != null)
		{
			returnValue += Icons.Inline($"res://Art/Icons/AMDs/Petals/{petals}LeftPetal.png", richTextParameters, true);
		}

		string valueIcon;

		switch(cardType)
		{
			case AMDCardType.Crit:
				valueIcon = "2x";
				break;
			case AMDCardType.Null:
				valueIcon = "null";
				break;
			case AMDCardType.Value:
				valueIcon = $"{(value >= 0 ? "+" : string.Empty)}{value}";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null);
		}

		returnValue += Icons.Inline(Icons.GetAMDValue(valueIcon), richTextParameters, true);

		if(conditionModels != null)
		{
			for(int i = 0; i < conditionModels.Count; i++)
			{
				ConditionModel conditionModel = conditionModels[i];
				// if(i > 0)
				// {
				// 	returnValue += ", ";
				// }

				if(!isSimple)
				{
					returnValue += " ";
				}

				returnValue += $"{Icons.Inline(Icons.GetCondition(conditionModel), richTextParameters, true)}";
			}
		}

		if(Pierce.HasValue)
		{
			if(!isSimple)
			{
				returnValue += " ";
			}

			returnValue += $"{Icons.Inline(Icons.Pierce, richTextParameters, true)}{Pierce}";
		}

		if(Push.HasValue)
		{
			if(!isSimple)
			{
				returnValue += " ";
			}

			returnValue += $"{Icons.Inline(Icons.Push, richTextParameters, true)}{Push}";
		}

		if(Pull.HasValue)
		{
			if(!isSimple)
			{
				returnValue += " ";
			}

			returnValue += $"{Icons.Inline(Icons.Push, richTextParameters, true)}{Pull}";
		}

		if(Swing.HasValue)
		{
			if(!isSimple)
			{
				returnValue += " ";
			}

			returnValue += $"{Icons.Inline(Icons.Swing, richTextParameters, true)}{Swing}";
		}

		if(extraText != null)
		{
			if(!isSimple)
			{
				returnValue += " ";
			}

			returnValue += $"“{extraText}”";
		}

		foreach(CardElementInfusion cardElementInfusion in ElementInfusions)
		{
			if(cardElementInfusion.ConsumableElements?.Any() == true)
			{
				continue;
			}

			if(!isSimple)
			{
				returnValue += " ";
			}

			if(cardElementInfusion.PossibleInfusedElements.Count == 1)
			{
				returnValue += Icons.Inline(Icons.GetElement(cardElementInfusion.PossibleInfusedElements[0]), richTextParameters, true);
			}
			else if(cardElementInfusion.PossibleInfusedElements.Count == 6)
			{
				returnValue += Icons.Inline(Icons.WildElement, richTextParameters, true);
			}
		}

		if(rolling)
		{
			if(!isSimple)
			{
				returnValue += " ";
			}

			returnValue += $"{Icons.Inline(Icons.Rolling, richTextParameters, true)}";
		}

		if(petals != null)
		{
			returnValue += Icons.Inline($"res://Art/Icons/AMDs/Petals/{petals}RightPetal.png", richTextParameters, true);
		}

		return returnValue;
	}

	protected string GetSimpleString(RichTextParameters richTextParameters, int value, string extraText = null, string petals = null)
	{
		return GetSimpleString(richTextParameters, AMDCardType.Value, value, extraText, petals);
	}

	protected string GetSimpleString(RichTextParameters richTextParameters, AMDCardType cardType, string extraText = null, string petals = null)
	{
		return GetSimpleString(richTextParameters, cardType, null, extraText, petals);
	}

	protected string GetSimpleString(RichTextParameters richTextParameters, AMDCardType cardType, int? value, string extraText = null,
		string petals = null)
	{
		string returnValue = string.Empty;
		if(petals != null)
		{
			returnValue += Icons.Inline($"res://Art/Icons/AMDs/Petals/{petals}LeftPetal.png", richTextParameters, true);
		}

		string valueIcon;
		switch(cardType)
		{
			case AMDCardType.Crit:
				valueIcon = "2x";
				break;
			case AMDCardType.Null:
				valueIcon = "null";
				break;
			case AMDCardType.Value:
				valueIcon = $"{(value >= 0 ? "+" : string.Empty)}{value}";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null);
		}

		returnValue += Icons.Inline(Icons.GetAMDValue(valueIcon), richTextParameters, true) + extraText;

		if(petals != null)
		{
			returnValue += Icons.Inline($"res://Art/Icons/AMDs/Petals/{petals}RightPetal.png", richTextParameters, true);
		}

		return returnValue;
	}

	protected Character GetCharacter(AttackAbility.State state)
	{
		return state?.Performer switch
		{
			Character performer => performer,
			Summon summon => summon.CharacterOwner,
			_ => null
		};
	}

	public virtual async GDTask OnBecomeActive(AMDCard card, Character character)
	{
		await GDTask.CompletedTask;
	}
}