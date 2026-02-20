using System.Collections.Generic;
using Fractural.Tasks;

public abstract class AbilityCardSideModel<TCharacter> : AbilityCardSideModel
	where TCharacter : Character
{
	protected TCharacter GetOriginalOwner(AbilityState abilityState)
	{
		return (TCharacter)GetAbilityCardSide(abilityState).AbilityCard.OriginalOwner;
	}
}

public abstract class AbilityCardSideModel : AbstractModel
{
	private bool _gotAbilities;
	private IEnumerable<AbilityCardAbility> _abilities;
	private readonly List<EnhancementMark> _enhancements = new List<EnhancementMark>();

	public AbilityCardModel AbilityCardModel { get; private set; }
	public virtual AbilityCardSideType AbilityCardSideType { get; private set; }

	public virtual IEnumerable<CardElementInfusion> Elements { get; } = [];
	public virtual int XP => 0;

	public virtual bool Round => false;
	public virtual bool Persistent => false;
	public virtual bool Loss => false;
	public virtual bool Unrecoverable => false;
	public virtual bool CanDeactivate => true;

	public IEnumerable<AbilityCardAbility> Abilities
	{
		get
		{
			TryGetAbilitiesAndEnhancements();

			return _abilities;
		}
	}

	public List<EnhancementMark> Enhancements
	{
		get
		{
			TryGetAbilitiesAndEnhancements();

			return _enhancements;
		}
	}

	public void Init(AbilityCardModel abilityCardModel, AbilityCardSideType abilityCardSideType)
	{
		AbilityCardModel = abilityCardModel;
		AbilityCardSideType = abilityCardSideType;

		TryGetAbilitiesAndEnhancements();
	}

	public void RegisterEnhancementMark(EnhancementMark enhancementMark)
	{
		_enhancements.AddIfNew(enhancementMark);
	}

	public virtual async GDTask OnActionPerformed(Figure figure)
	{
		await GDTask.CompletedTask;
	}

	protected virtual void InitExtraEnhancements()
	{
	}

	protected abstract List<AbilityCardAbility> GetAbilities();

	protected AbilityCardSide GetAbilityCardSide(AbilityState abilityState)
	{
		return (AbilityCardSide)abilityState.ActionState.ActionSource;
	}

	protected AbilityCard GetAbilityCard(AbilityState abilityState)
	{
		return GetAbilityCardSide(abilityState).AbilityCard;
	}

	protected async GDTask GainXP(AbilityState abilityState)
	{
		await AbilityCmd.GainXP(abilityState.Performer, 1);
	}

	protected async GDTask Gain2XP(AbilityState abilityState)
	{
		await AbilityCmd.GainXP(abilityState.Performer, 2);
	}

	private void TryGetAbilitiesAndEnhancements()
	{
		if(_gotAbilities)
		{
			return;
		}

		_gotAbilities = true;

		InitExtraEnhancements();
		_abilities = GetAbilities();
	}
}