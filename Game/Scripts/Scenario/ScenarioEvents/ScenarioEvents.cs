using System;
using System.Collections.Generic;
using Godot;

public class ScenarioEvents
{
	private readonly List<EventSubscriberPair> _eventSubscriberPairs = new List<EventSubscriberPair>();
	private static List<EventSubscriberPair> EventSubscriberPairs => GameController.Instance.ScenarioEvents._eventSubscriberPairs;

	public static EventSubscriberPair GetSubscriberPair(object subscriberA, object subscriberB)
	{
		if(subscriberA == null || subscriberB == null)
		{
			throw new Exception("One of the given subscribers is null.");
		}

		foreach(EventSubscriberPair eventSubscriberPair in EventSubscriberPairs)
		{
			if(eventSubscriberPair.SubscriberA == subscriberA && eventSubscriberPair.SubscriberB == subscriberB)
			{
				return eventSubscriberPair;
			}
		}

		EventSubscriberPair newEventSubscriberPair = new EventSubscriberPair(subscriberA, subscriberB);
		EventSubscriberPairs.Add(newEventSubscriberPair);

		return newEventSubscriberPair;
	}

	public class GenericChoice : ScenarioEvent<GenericChoice.Parameters>
	{
		public class Parameters() : ParametersBase
		{
			public bool ChoiceMade { get; private set; }

			public void SetChoiceMade()
			{
				ChoiceMade = true;
			}
		}
	}

	private readonly GenericChoice _genericChoice = new GenericChoice();
	public static GenericChoice GenericChoiceEvent => GameController.Instance.ScenarioEvents._genericChoice;

	// public class AttackAbilityStart : ScenarioEvent<AttackAbilityStart.Parameters>
	// {
	// 	public class Parameters(AttackAbility.State abilityState)
	// 		: ParametersBase<AttackAbility.State>(abilityState)
	// 	{
	// 	}
	// }
	//
	// private readonly AttackAbilityStart _attackAbilityStart = new AttackAbilityStart();
	// public static AttackAbilityStart AttackAbilityStartEvent => GameController.Instance.ScenarioEvents._attackAbilityStart;

	public class DuringAttack : ScenarioEvent<DuringAttack.Parameters>
	{
		public class Parameters(AttackAbility.State abilityState) : ParametersBase<AttackAbility.State>(abilityState)
		{
		}
	}

	private readonly DuringAttack _duringAttack = new DuringAttack();
	public static DuringAttack DuringAttackEvent => GameController.Instance.ScenarioEvents._duringAttack;

	public class AttackAfterTargetConfirmed : ScenarioEvent<AttackAfterTargetConfirmed.Parameters>
	{
		public class Parameters(AttackAbility.State abilityState) : ParametersBase<AttackAbility.State>(abilityState)
		{
		}
	}

	private readonly AttackAfterTargetConfirmed _attackAfterTargetConfirmed = new AttackAfterTargetConfirmed();
	public static AttackAfterTargetConfirmed AttackAfterTargetConfirmedEvent => GameController.Instance.ScenarioEvents._attackAfterTargetConfirmed;

	public class AfterAttackPerformed : ScenarioEvent<AfterAttackPerformed.Parameters>
	{
		public class Parameters(AttackAbility.State abilityState) : ParametersBase<AttackAbility.State>(abilityState)
		{
		}
	}

	private readonly AfterAttackPerformed _afterAttackPerformed = new AfterAttackPerformed();
	public static AfterAttackPerformed AfterAttackPerformedEvent => GameController.Instance.ScenarioEvents._afterAttackPerformed;

	public class AMDCardDrawn : ScenarioEvent<AMDCardDrawn.Parameters>
	{
		public class Parameters(AttackAbility.State abilityState, AMDCard amdCard)
			: ParametersBase<AttackAbility.State>(abilityState)
		{
			public AMDCard AMDCard = amdCard;
			public AMDCardType Type { get; private set; } = amdCard.Type;
			public int? Value { get; private set; } = amdCard.Value;

			public void SetType(AMDCardType type)
			{
				Type = type;
			}

			public void SetValue(int? value)
			{
				Value = value;
			}
		}
	}

	private readonly AMDCardDrawn _amdCardDrawn = new AMDCardDrawn();
	public static AMDCardDrawn AMDCardDrawnEvent => GameController.Instance.ScenarioEvents._amdCardDrawn;

	public class AMDCardValueApplied : ScenarioEvent<AMDCardValueApplied.Parameters>
	{
		public class Parameters(AttackAbility.State abilityState, AMDCardValue amdCardValue)
			: ParametersBase<AttackAbility.State>(abilityState)
		{
			public AMDCardValue AMDCardValue { get; } = amdCardValue;
		}
	}

	private readonly AMDCardValueApplied _amdCardValueApplied = new AMDCardValueApplied();
	public static AMDCardValueApplied AMDCardValueAppliedEvent => GameController.Instance.ScenarioEvents._amdCardValueApplied;
	
	public class DuringHeal : ScenarioEvent<DuringHeal.Parameters>
	{
		public class Parameters(HealAbility.State abilityState) : ParametersBase<HealAbility.State>(abilityState)
		{
		}
	}

	private readonly DuringHeal _duringHeal = new DuringHeal();
	public static DuringHeal DuringHealEvent => GameController.Instance.ScenarioEvents._duringHeal;

	public class HealAfterTargetConfirmed : ScenarioEvent<HealAfterTargetConfirmed.Parameters>
	{
		public class Parameters(HealAbility.State abilityState)
			: ParametersBase<HealAbility.State>(abilityState)
		{
		}
	}

	private readonly HealAfterTargetConfirmed _healAfterTargetConfirmed = new HealAfterTargetConfirmed();
	public static HealAfterTargetConfirmed HealAfterTargetConfirmedEvent => GameController.Instance.ScenarioEvents._healAfterTargetConfirmed;

	public class HealBlockTime : ScenarioEvent<HealBlockTime.Parameters>
	{
		public class Parameters(HealAbility.State abilityState)
			: ParametersBase<HealAbility.State>(abilityState)
		{
			public bool IsBlocked { get; private set; }

			public void SetBlocked(bool blocked)
			{
				IsBlocked = blocked;
			}
		}
	}

	private readonly HealBlockTime _healBlockTime = new HealBlockTime();
	public static HealBlockTime HealBlockTimeEvent => GameController.Instance.ScenarioEvents._healBlockTime;

	public class AfterHealPerformed : ScenarioEvent<AfterHealPerformed.Parameters>
	{
		public class Parameters(HealAbility.State abilityState, bool isBlocked) : ParametersBase<HealAbility.State>(abilityState)
		{
			public bool IsBlocked { get; } = isBlocked;
		}
	}

	private readonly AfterHealPerformed _afterHealPerformed = new AfterHealPerformed();
	public static AfterHealPerformed AfterHealPerformedEvent => GameController.Instance.ScenarioEvents._afterHealPerformed;

	public class ConditionAfterTargetConfirmed : ScenarioEvent<ConditionAfterTargetConfirmed.Parameters>
	{
		public class Parameters(ConditionAbility.State abilityState) : ParametersBase<ConditionAbility.State>(abilityState)
		{
		}
	}

	private readonly ConditionAfterTargetConfirmed _conditionAfterTargetConfirmed = new ConditionAfterTargetConfirmed();
	public static ConditionAfterTargetConfirmed ConditionAfterTargetConfirmedEvent => GameController.Instance.ScenarioEvents._conditionAfterTargetConfirmed;

	public class InflictConditions : ScenarioEvent<InflictConditions.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure target, List<ConditionModel> conditionModels) : ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Target { get; } = target;
			public List<ConditionModel> ConditionModels { get; } = conditionModels;

			public void PreventCondition(ConditionModel conditionModel)
			{
				ConditionModels.Remove(conditionModel);
			}
		}
	}

	private readonly InflictConditions _inflictConditions = new InflictConditions();
	public static InflictConditions InflictConditionsEvent => GameController.Instance.ScenarioEvents._inflictConditions;

	public class InflictCondition : ScenarioEvent<InflictCondition.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure target, ConditionModel condition) : ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Target { get; } = target;
			public ConditionModel Condition { get; } = condition;

			public bool Prevented { get; private set; }

			public void SetPrevented(bool prevented)
			{
				Prevented = prevented;
			}
		}
	}

	private readonly InflictCondition _inflictCondition = new InflictCondition();
	public static InflictCondition InflictConditionEvent => GameController.Instance.ScenarioEvents._inflictCondition;

	public class InflictConditionDuplicatesCheck : ScenarioEvent<InflictConditionDuplicatesCheck.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure target, ConditionModel condition)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Target { get; } = target;
			public ConditionModel Condition { get; } = condition;

			public bool Prevented { get; private set; }

			public void SetPrevented(bool prevented)
			{
				Prevented = prevented;
			}
		}
	}

	private readonly InflictConditionDuplicatesCheck _inflictConditionDuplicatesCheck = new InflictConditionDuplicatesCheck();
	public static InflictConditionDuplicatesCheck InflictConditionDuplicatesCheckEvent => GameController.Instance.ScenarioEvents._inflictConditionDuplicatesCheck;

	public class DuringGrant : ScenarioEvent<DuringGrant.Parameters>
	{
		public class Parameters(GrantAbility.State abilityState) : ParametersBase<GrantAbility.State>(abilityState)
		{
		}
	}

	private readonly DuringGrant _duringGrant = new DuringGrant();
	public static DuringGrant DuringGrantEvent => GameController.Instance.ScenarioEvents._duringGrant;

	public class SufferDamage : ScenarioEvent<SufferDamage.Parameters>
	{
		public class Parameters : ParametersBase
		{
			public AttackAbility.State PotentialAttackAbilityState { get; }
			public Figure Figure { get; }
			public int InitialDamage { get; }
			public int CalculatedCurrentDamage { get; private set; }

			public int Shield { get; private set; } = 0;
			public int UnpierceableShield { get; private set; } = 0;

			public bool DamagePrevented { get; private set; }

			public bool HasWard { get; private set; }
			public bool HasBrittle { get; private set; }

			public bool FromAttack => PotentialAttackAbilityState != null;

			public bool WouldSufferDamage => CalculatedCurrentDamage > 0 && !DamagePrevented;

			public Parameters(AttackAbility.State potentialAttackAbilityState, Figure figure, int initialDamage)
			{
				PotentialAttackAbilityState = potentialAttackAbilityState;
				Figure = figure;
				InitialDamage = initialDamage;

				CalculateCurrentDamage();
			}

			public void AdjustShield(int amount)
			{
				Shield += amount;

				CalculateCurrentDamage();
			}

			public void AdjustUnpierceableShield(int amount)
			{
				UnpierceableShield += amount;

				CalculateCurrentDamage();
			}

			public void SetDamagePrevented()
			{
				DamagePrevented = true;

				CalculateCurrentDamage();
			}

			public void SetWard(bool ward)
			{
				HasWard = ward;

				CalculateCurrentDamage();
			}

			public void SetBrittle(bool brittle)
			{
				HasBrittle = brittle;

				CalculateCurrentDamage();
			}

			private void CalculateCurrentDamage()
			{
				if(DamagePrevented)
				{
					CalculatedCurrentDamage = 0;
					return;
				}

				bool ignoresShield = PotentialAttackAbilityState?.SingleTargetIgnoresAllShields ?? false;

				int finalPierce = Mathf.Max(PotentialAttackAbilityState?.SingleTargetPierce ?? 0, 0);
				int finalShieldValue = ignoresShield ? 0 : Mathf.Max(Shield - finalPierce, 0) + UnpierceableShield;
				int finalDamage = Mathf.Max(InitialDamage - finalShieldValue, 0);

				if(HasBrittle)
				{
					finalDamage *= 2;
				}

				if(HasWard)
				{
					finalDamage /= 2;
				}

				CalculatedCurrentDamage = finalDamage;
			}
		}
	}

	private readonly SufferDamage _sufferDamage = new SufferDamage();
	public static SufferDamage SufferDamageEvent => GameController.Instance.ScenarioEvents._sufferDamage;

	public class JustBeforeSufferDamage : ScenarioEvent<JustBeforeSufferDamage.Parameters>
	{
		public class Parameters(Figure figure, int damage, AttackAbility.State potentialAttackAbilityState, SufferDamage.Parameters sufferDamageParameters) : ParametersBase
		{
			public Figure Figure { get; } = figure;
			public int Damage { get; } = damage;
			public AttackAbility.State PotentialAttackAbilityState { get; } = potentialAttackAbilityState;
			public SufferDamage.Parameters SufferDamageParameters { get; } = sufferDamageParameters;

			public bool Prevented { get; private set; }

			public void SetPrevented()
			{
				Prevented = true;
			}
		}
	}

	private readonly JustBeforeSufferDamage _justBeforeSufferDamage = new JustBeforeSufferDamage();
	public static JustBeforeSufferDamage JustBeforeSufferDamageEvent => GameController.Instance.ScenarioEvents._justBeforeSufferDamage;

	public class AfterSufferDamage : ScenarioEvent<AfterSufferDamage.Parameters>
	{
		public class Parameters(Figure figure, int damage, AttackAbility.State potentialAttackAbilityState, SufferDamage.Parameters sufferDamageParameters) : ParametersBase
		{
			public Figure Figure { get; } = figure;
			public int Damage { get; } = damage;
			public AttackAbility.State PotentialAttackAbilityState { get; } = potentialAttackAbilityState;
			public SufferDamage.Parameters SufferDamageParameters { get; } = sufferDamageParameters;
		}
	}

	private readonly AfterSufferDamage _afterSufferDamage = new AfterSufferDamage();
	public static AfterSufferDamage AfterSufferDamageEvent => GameController.Instance.ScenarioEvents._afterSufferDamage;

	public class FigureKilled : ScenarioEvent<FigureKilled.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure figure) : ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Figure { get; } = figure;
		}
	}

	private readonly FigureKilled _figureKilled = new FigureKilled();
	public static FigureKilled FigureKilledEvent => GameController.Instance.ScenarioEvents._figureKilled;

	public class Retaliate : ScenarioEvent<Retaliate.Parameters>
	{
		public class Parameters(AttackAbility.State abilityState, Figure retaliatingFigure) : ParametersBase<AttackAbility.State>(abilityState)
		{
			public Figure RetaliatingFigure { get; } = retaliatingFigure;

			public int Retaliate { get; private set; }
			public bool RetaliateBlocked { get; private set; }

			public void AdjustRetaliate(int amount)
			{
				Retaliate += amount;
			}

			public void SetRetaliateBlocked()
			{
				RetaliateBlocked = true;
			}
		}
	}

	private readonly Retaliate _retaliate = new Retaliate();
	public static Retaliate RetaliateEvent => GameController.Instance.ScenarioEvents._retaliate;

	public class DuringMovement : ScenarioEvent<DuringMovement.Parameters>
	{
		public class Parameters(MoveAbility.State abilityState) : ParametersBase<MoveAbility.State>(abilityState)
		{
		}
	}

	private readonly DuringMovement _duringMovement = new DuringMovement();
	public static DuringMovement DuringMovementEvent => GameController.Instance.ScenarioEvents._duringMovement;

	public class CanMoveFurtherCheck : ScenarioEvent<CanMoveFurtherCheck.Parameters>
	{
		public class Parameters(Figure performer)
			: ParametersBase
		{
			public Figure Performer { get; } = performer;

			public bool CanMoveFurther { get; private set; } = true;

			public void SetCannotMoveFurther()
			{
				CanMoveFurther = false;
			}
		}
	}

	private readonly CanMoveFurtherCheck _canMoveFurtherCheck = new CanMoveFurtherCheck();
	public static CanMoveFurtherCheck CanMoveFurtherCheckEvent => GameController.Instance.ScenarioEvents._canMoveFurtherCheck;

	public class FigureEnteredHex : ScenarioEvent<FigureEnteredHex.Parameters>
	{
		public class Parameters(AbilityState abilityState, Figure figure)
			: ParametersBase<AbilityState>(abilityState)
		{
			public Figure Figure { get; } = figure;

			public Hex Hex => Figure.Hex;
		}
	}

	private readonly FigureEnteredHex _figureEnteredHex = new FigureEnteredHex();
	public static FigureEnteredHex FigureEnteredHexEvent => GameController.Instance.ScenarioEvents._figureEnteredHex;

	public class HazardousTerrainTriggered : ScenarioEvent<HazardousTerrainTriggered.Parameters>
	{
		public class Parameters(AbilityState abilityState, Hex hex, HazardousTerrain hazardousTerrain, bool affectedByHazardousTerrain)
			: ParametersBase<AbilityState>(abilityState)
		{
			public Hex Hex { get; } = hex;
			public HazardousTerrain HazardousTerrain { get; } = hazardousTerrain;
			public bool AffectedByHazardousTerrain { get; private set; } = affectedByHazardousTerrain;

			public void SetAffectedByHazardousTerrain(bool affectedByHazardousTerrain)
			{
				AffectedByHazardousTerrain = affectedByHazardousTerrain;
			}
		}
	}

	private readonly HazardousTerrainTriggered _hazardousTerrainTriggered = new HazardousTerrainTriggered();
	public static HazardousTerrainTriggered HazardousTerrainTriggeredEvent => GameController.Instance.ScenarioEvents._hazardousTerrainTriggered;

	public class TrapTriggered : ScenarioEvent<TrapTriggered.Parameters>
	{
		public class Parameters(AbilityState abilityState, Hex hex, Trap trap, bool triggersTrap)
			: ParametersBase<AbilityState>(abilityState)
		{
			public Hex Hex { get; } = hex;
			public Trap Trap { get; } = trap;
			public bool TriggersTrap { get; private set; } = triggersTrap;

			public void SetTriggersTrap(bool triggersTrap)
			{
				TriggersTrap = triggersTrap;
			}
		}
	}

	private readonly TrapTriggered _trapTriggered = new TrapTriggered();
	public static TrapTriggered TrapTriggeredEvent => GameController.Instance.ScenarioEvents._trapTriggered;

	public class ConsumeElement : ScenarioEvent<ConsumeElement.Parameters>
	{
		public class Parameters(IEnumerable<Element> elements)
			: ParametersBase
		{
			public IEnumerable<Element> Elements { get; } = elements;

			public bool Consumed { get; private set; }
			public Element ConsumedElement { get; private set; }

			public void SetConsumed(Element element)
			{
				Consumed = true;
				ConsumedElement = element;
			}
		}
	}

	private readonly ConsumeElement _consumeElement = new ConsumeElement();
	public static ConsumeElement ConsumeElementElement => GameController.Instance.ScenarioEvents._consumeElement;

	public class AbilityStarted : ScenarioEvent<AbilityStarted.Parameters>
	{
		public class Parameters(AbilityState abilityState)
			: ParametersBase<AbilityState>(abilityState)
		{
			public bool IsBlocked { get; private set; }

			public void SetIsBlocked(bool isBlocked)
			{
				IsBlocked = isBlocked;
			}
		}
	}

	private readonly AbilityStarted _abilityStarted = new AbilityStarted();
	public static AbilityStarted AbilityStartedEvent => GameController.Instance.ScenarioEvents._abilityStarted;

	public class AbilityEnded : ScenarioEvent<AbilityEnded.Parameters>
	{
		public class Parameters(AbilityState abilityState)
			: ParametersBase<AbilityState>(abilityState)
		{
		}
	}

	private readonly AbilityEnded _abilityEnded = new AbilityEnded();
	public static AbilityEnded AbilityEndedEvent => GameController.Instance.ScenarioEvents._abilityEnded;

	public class AbilityPerformed : ScenarioEvent<AbilityPerformed.Parameters>
	{
		public class Parameters(AbilityState abilityState)
			: ParametersBase<AbilityState>(abilityState)
		{
		}
	}

	private readonly AbilityPerformed _abilityPerformed = new AbilityPerformed();
	public static AbilityPerformed AbilityPerformedEvent => GameController.Instance.ScenarioEvents._abilityPerformed;

	public class AbilityCardStateChanged : ScenarioEvent<AbilityCardStateChanged.Parameters>
	{
		public class Parameters(AbilityCard abilityCard)
			: ParametersBase
		{
			public AbilityCard AbilityCard { get; } = abilityCard;
		}
	}

	private readonly AbilityCardStateChanged _abilityCardStateChanged = new AbilityCardStateChanged();
	public static AbilityCardStateChanged AbilityCardStateChangedEvent => GameController.Instance.ScenarioEvents._abilityCardStateChanged;

	public class ActionStarted : ScenarioEvent<ActionStarted.Parameters>
	{
		public class Parameters(ActionState actionState)
			: ParametersBase
		{
			public ActionState ActionState { get; } = actionState;
		}
	}

	private readonly ActionStarted _actionStarted = new ActionStarted();
	public static ActionStarted ActionStartedEvent => GameController.Instance.ScenarioEvents._actionStarted;

	public class ActionEnded : ScenarioEvent<ActionEnded.Parameters>
	{
		public class Parameters(ActionState actionState)
			: ParametersBase
		{
			public ActionState ActionState { get; } = actionState;
		}
	}

	private readonly ActionEnded _actionEnded = new ActionEnded();
	public static ActionEnded ActionEndedEvent => GameController.Instance.ScenarioEvents._actionEnded;

	public class ItemStateChanged : ScenarioEvent<ItemStateChanged.Parameters>
	{
		public class Parameters(ItemModel item)
			: ParametersBase
		{
			public ItemModel Item { get; } = item;
		}
	}

	private readonly ItemStateChanged _itemStateChanged = new ItemStateChanged();
	public static ItemStateChanged ItemStateChangedEvent => GameController.Instance.ScenarioEvents._itemStateChanged;

	public class ShortRestStarted : ScenarioEvent<ShortRestStarted.Parameters>
	{
		public class Parameters(Character character)
			: ParametersBase
		{
			public Character Character { get; } = character;

			public bool CanSelectCardToLose { get; private set; } = false;

			public void SetCanSelectCardToUse()
			{
				CanSelectCardToLose = true;
			}
		}
	}

	private readonly ShortRestStarted _shortRestStarted = new ShortRestStarted();
	public static ShortRestStarted ShortRestStartedEvent => GameController.Instance.ScenarioEvents._shortRestStarted;

	public class LongRestCardSelection : ScenarioEvent<LongRestCardSelection.Parameters>
	{
		public class Parameters(Character character) : ParametersBase
		{
			public Character Character { get; } = character;
		}
	}

	private readonly LongRestCardSelection _longRestCardSelection = new LongRestCardSelection();
	public static LongRestCardSelection LongRestCardSelectionEvent => GameController.Instance.ScenarioEvents._longRestCardSelection;

	public class FigureTurnStarted : ScenarioEvent<FigureTurnStarted.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;
		}
	}

	private readonly FigureTurnStarted _figureTurnStarted = new FigureTurnStarted();
	public static FigureTurnStarted FigureTurnStartedEvent => GameController.Instance.ScenarioEvents._figureTurnStarted;

	public class AbilityCardSideStarted : ScenarioEvent<AbilityCardSideStarted.Parameters>
	{
		public class Parameters(AbilityCardSide abilityCardSide, Figure performer) : ParametersBase
		{
			public AbilityCardSide AbilityCardSide { get; } = abilityCardSide;
			public Figure Performer { get; } = performer;

			public bool ForgoneAction { get; private set; }

			public void ForgoAction()
			{
				ForgoneAction = true;
			}
		}
	}

	private readonly AbilityCardSideStarted _abilityCardSideStarted = new AbilityCardSideStarted();
	public static AbilityCardSideStarted AbilityCardSideStartedEvent => GameController.Instance.ScenarioEvents._abilityCardSideStarted;

	public class AbilityCardSideEnded : ScenarioEvent<AbilityCardSideEnded.Parameters>
	{
		public class Parameters(AbilityCardSide abilityCardSide, Figure performer) : ParametersBase
		{
			public AbilityCardSide AbilityCardSide { get; } = abilityCardSide;
			public Figure Performer { get; } = performer;
		}
	}

	private readonly AbilityCardSideEnded _abilityCardSideEnded = new AbilityCardSideEnded();
	public static AbilityCardSideEnded AbilityCardSideEndedEvent => GameController.Instance.ScenarioEvents._abilityCardSideEnded;

	public class CardSideSelection : ScenarioEvent<CardSideSelection.Parameters>
	{
		public class Parameters(Character character)
			: ParametersBase
		{
			public Character Character { get; } = character;

			// public bool ForgoneTopAction { get; private set; }
			//
			// public void SetForgoneTopAction()
			// {
			// 	ForgoneTopAction = true;
			// }
		}
	}

	private readonly CardSideSelection _cardSideSelectionStarted = new CardSideSelection();
	public static CardSideSelection CardSideSelectionEvent => GameController.Instance.ScenarioEvents._cardSideSelectionStarted;

	// public class BeforeCardSidePerform : ScenarioEvent<BeforeCardSidePerform.Parameters>
	// {
	// 	public class Parameters(Character character)
	// 		: ParametersBase
	// 	{
	// 		public Character Character { get; } = character;
	//
	// 		public AbilityCardSide AbilityCardSide { get; private set; }
	//
	// 		public bool ForgoneAction { get; private set; }
	//
	// 		public void ForgoAction()
	// 		{
	// 			ForgoneAction = true;
	// 		}
	// 	}
	// }
	//
	// private readonly BeforeCardSidePerform _beforeCardSidePerform = new BeforeCardSidePerform();
	// public static BeforeCardSidePerform BeforeCardSidePerformEvent => GameController.Instance.ScenarioEvents._beforeCardSidePerform;

	public class AfterCardsPlayed : ScenarioEvent<AfterCardsPlayed.Parameters>
	{
		public class Parameters(Character character)
			: ParametersBase
		{
			public Character Character { get; } = character;
		}
	}

	private readonly AfterCardsPlayed _afterCardsPlayedEvent = new AfterCardsPlayed();
	public static AfterCardsPlayed AfterCardsPlayedEvent => GameController.Instance.ScenarioEvents._afterCardsPlayedEvent;

	public class FigureTurnEnding : ScenarioEvent<FigureTurnEnding.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;
		}
	}

	private readonly FigureTurnEnding _figureTurnEnding = new FigureTurnEnding();
	public static FigureTurnEnding FigureTurnEndingEvent => GameController.Instance.ScenarioEvents._figureTurnEnding;

	public class FigureTurnEndedConditionsFallOff : ScenarioEvent<FigureTurnEndedConditionsFallOff.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;
		}
	}

	private readonly FigureTurnEndedConditionsFallOff _figureTurnEndedConditionsFallOff = new FigureTurnEndedConditionsFallOff();
	public static FigureTurnEndedConditionsFallOff FigureTurnEndedConditionsFallOffEvent => GameController.Instance.ScenarioEvents._figureTurnEndedConditionsFallOff;

	public class FigureTurnEnded : ScenarioEvent<FigureTurnEnded.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;
		}
	}

	private readonly FigureTurnEnded _figureTurnEnded = new FigureTurnEnded();
	public static FigureTurnEnded FigureTurnEndedEvent => GameController.Instance.ScenarioEvents._figureTurnEnded;

	public class RoundStartBeforeCardSelection : ScenarioEvent<RoundStartBeforeCardSelection.Parameters>
	{
		public class Parameters()
			: ParametersBase
		{
		}
	}

	private readonly RoundStartBeforeCardSelection _roundStartBeforeCardSelection = new RoundStartBeforeCardSelection();
	public static RoundStartBeforeCardSelection RoundStartBeforeCardSelectionEvent => GameController.Instance.ScenarioEvents._roundStartBeforeCardSelection;

	public class RoundStartedBeforeInitiativesSorted : ScenarioEvent<RoundStartedBeforeInitiativesSorted.Parameters>
	{
		public class Parameters(int roundIndex)
			: ParametersBase
		{
			public int RoundIndex { get; } = roundIndex;

			public int RoundNumber => RoundIndex + 1;
		}
	}

	private readonly RoundStartedBeforeInitiativesSorted _roundStartedBeforeInitiativesSorted = new RoundStartedBeforeInitiativesSorted();
	public static RoundStartedBeforeInitiativesSorted RoundStartedBeforeInitiativesSortedEvent => GameController.Instance.ScenarioEvents._roundStartedBeforeInitiativesSorted;

	public class InitiativesSorted : ScenarioEvent<InitiativesSorted.Parameters>
	{
		public class Parameters(int roundIndex)
			: ParametersBase
		{
			public int RoundIndex { get; } = roundIndex;

			public int RoundNumber => RoundIndex + 1;
		}
	}

	private readonly InitiativesSorted _initiativesSorted = new InitiativesSorted();
	public static InitiativesSorted InitiativesSortedEvent => GameController.Instance.ScenarioEvents._initiativesSorted;

	public class RoundEnded : ScenarioEvent<RoundEnded.Parameters>
	{
		public class Parameters(int roundIndex)
			: ParametersBase
		{
			public int RoundIndex { get; } = roundIndex;

			public int RoundNumber => RoundIndex + 1;
		}
	}

	private readonly RoundEnded _roundEnded = new RoundEnded();
	public static RoundEnded RoundEndedEvent => GameController.Instance.ScenarioEvents._roundEnded;

	public class RoomRevealed : ScenarioEvent<RoomRevealed.Parameters>
	{
		public class Parameters(Room room, Door openedDoor)
			: ParametersBase
		{
			public Room Room { get; } = room;
			public Door OpenedDoor { get; } = openedDoor;
		}
	}

	private readonly RoomRevealed _roomRevealed = new RoomRevealed();
	public static RoomRevealed RoomRevealedEvent => GameController.Instance.ScenarioEvents._roomRevealed;

	public class ItemUseStarted : ScenarioEvent<ItemUseStarted.Parameters>
	{
		public class Parameters(ItemModel item, Figure performer) : ParametersBase
		{
			public ItemModel Item { get; } = item;
			public Figure Performer { get; } = performer;
		}
	}

	private readonly ItemUseStarted _itemUseStarted = new ItemUseStarted();
	public static ItemUseStarted ItemUseStartedEvent => GameController.Instance.ScenarioEvents._itemUseStarted;

	public class ItemUseEnded : ScenarioEvent<ItemUseEnded.Parameters>
	{
		public class Parameters(ItemModel item, Figure performer) : ParametersBase
		{
			public ItemModel Item { get; } = item;
			public Figure Performer { get; } = performer;
		}
	}

	private readonly ItemUseEnded _itemUseEnded = new ItemUseEnded();
	public static ItemUseEnded ItemUseEndedEvent => GameController.Instance.ScenarioEvents._itemUseEnded;
}