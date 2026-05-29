using System;
using System.Collections.Generic;
using Godot;

public partial class ScenarioEvents
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
		public class Parameters(object source) : ParametersBase
		{
			public object Source { get; } = source;

			public bool ChoiceMade { get; private set; }

			public void SetChoiceMade()
			{
				ChoiceMade = true;
			}
		}
	}

	private readonly GenericChoice _genericChoice = new GenericChoice();
	public static GenericChoice GenericChoiceEvent => GameController.Instance.ScenarioEvents._genericChoice;

	public class HexObjectDestroyed : ScenarioEvent<HexObjectDestroyed.Parameters>
	{
		public class Parameters(HexObject hexObject, bool immediately, bool forceDestroy) : ParametersBase
		{
			public HexObject HexObject { get; } = hexObject;
			public bool Immediately { get; } = immediately;
			public bool ForceDestroy { get; } = forceDestroy;
		}
	}

	private readonly HexObjectDestroyed _hexObjectDestroyed = new HexObjectDestroyed();
	public static HexObjectDestroyed HexObjectDestroyedEvent => GameController.Instance.ScenarioEvents._hexObjectDestroyed;

	public class DuringPush : ScenarioEvent<DuringPush.Parameters>
	{
		public class Parameters(PushAbility.State abilityState) : ParametersBase<PushAbility.State>(abilityState)
		{
		}
	}

	private readonly DuringPush _duringPush = new DuringPush();
	public static DuringPush DuringPushEvent => GameController.Instance.ScenarioEvents._duringPush;

	public class DuringPull : ScenarioEvent<DuringPull.Parameters>
	{
		public class Parameters(PullAbility.State abilityState) : ParametersBase<PullAbility.State>(abilityState)
		{
		}
	}

	private readonly DuringPull _duringPull = new DuringPull();
	public static DuringPull DuringPullEvent => GameController.Instance.ScenarioEvents._duringPull;

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
			public bool CannotGainDisadvantage { get; private set; } = false;

			public void SetCannotGainDisadvantage()
			{
				CannotGainDisadvantage = true;
			}
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
			public AMDCardType Type { get; private set; } = amdCard.Model.Type;
			public int? Value { get; private set; } = amdCard.Model.GetValue(abilityState);

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

	public class AMDCardPeeked : ScenarioEvent<AMDCardPeeked.Parameters>
	{
		public class Parameters(DivinationAbility.State abilityState, AMDCard amdCard)
			: ParametersBase<DivinationAbility.State>(abilityState)
		{
			public AMDCard AMDCard = amdCard;
			public bool PlaceAtDeckTop { get; private set; } = false;
			public bool PlaceAtDeckBottom { get; private set; } = false;

			public void SetPlaceAtDeckTop()
			{
				PlaceAtDeckTop = true;
				PlaceAtDeckBottom = false;
			}

			public void SetPlaceAtDeckBottom()
			{
				PlaceAtDeckTop = false;
				PlaceAtDeckBottom = true;
			}
		}
	}

	private readonly AMDCardPeeked _amdCardPeeked = new AMDCardPeeked();
	public static AMDCardPeeked AMDCardPeekedEvent => GameController.Instance.ScenarioEvents._amdCardPeeked;

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

	public class EmpowerAdded : ScenarioEvent<EmpowerAdded.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure EmpoweredFigure { get; } = figure;

			public bool ShuffleDrawPile { get; private set; } = true;

			public void SetShuffleDrawPile(bool shuffleDrawPile)
			{
				ShuffleDrawPile = shuffleDrawPile;
			}
		}
	}

	private readonly EmpowerAdded _empowerAdded = new EmpowerAdded();
	public static EmpowerAdded EmpowerAddedEvent => GameController.Instance.ScenarioEvents._empowerAdded;

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

	public static ConditionAfterTargetConfirmed ConditionAfterTargetConfirmedEvent =>
		GameController.Instance.ScenarioEvents._conditionAfterTargetConfirmed;

	public class InflictConditions : ScenarioEvent<InflictConditions.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure target, List<ConditionModel> conditionModels) : ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Target { get; } = target;
			public List<ConditionModel> ConditionModels { get; } = conditionModels;

			public void PreventCondition(ConditionModel conditionModel)
			{
				for(int i = ConditionModels.Count - 1; i >= 0; i--)
				{
					ConditionModel otherModel = ConditionModels[i];
					if(otherModel == conditionModel)
					{
						ConditionModels.RemoveAt(i);
						break;
					}
				}
			}
		}
	}

	private readonly InflictConditions _inflictConditions = new InflictConditions();
	public static InflictConditions InflictConditionsEvent => GameController.Instance.ScenarioEvents._inflictConditions;

	public class InflictCondition : ScenarioEvent<InflictCondition.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure target, Figure potentialConditionGiver, ConditionModel conditionModel)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Target { get; } = target;
			public Figure PotentialConditionGiver { get; } = potentialConditionGiver;
			public ConditionModel ConditionModel { get; } = conditionModel;

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
		public class Parameters(AbilityState potentialAbilityState, Figure target, ConditionModel conditionModel)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Target { get; } = target;
			public ConditionModel ConditionModel { get; } = conditionModel;

			public bool Prevented { get; private set; }
			public bool AddStack { get; private set; }

			public void SetPrevented(bool prevented)
			{
				Prevented = prevented;
			}

			public void SetAddStack()
			{
				AddStack = true;
			}
		}
	}

	private readonly InflictConditionDuplicatesCheck _inflictConditionDuplicatesCheck = new InflictConditionDuplicatesCheck();

	public static InflictConditionDuplicatesCheck InflictConditionDuplicatesCheckEvent =>
		GameController.Instance.ScenarioEvents._inflictConditionDuplicatesCheck;

	public class ConditionAdded : ScenarioEvent<ConditionAdded.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure target, Figure potentialConditionGiver, ConditionModel conditionModel)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Target { get; } = target;
			public Figure PotentialConditionGiver { get; } = potentialConditionGiver;
			public ConditionModel ConditionModel { get; } = conditionModel;
		}
	}

	private readonly ConditionAdded _conditionAdded = new ConditionAdded();
	public static ConditionAdded ConditionAddedEvent => GameController.Instance.ScenarioEvents._conditionAdded;

	public class RemoveCondition : ScenarioEvent<RemoveCondition.Parameters>
	{
		public class Parameters(Condition condition, AbilityState potentialAbilityState) : ParametersBase
		{
			public Condition Condition { get; } = condition;

			public Figure Figure { get; } = condition.Owner;
			public ConditionModel ConditionModel { get; } = condition.ConditionModel;
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;

			public bool Prevented { get; private set; } = false;

			public void SetPrevented()
			{
				Prevented = true;
			}
		}
	}

	private readonly RemoveCondition _removeCondition = new RemoveCondition();
	public static RemoveCondition RemoveConditionEvent => GameController.Instance.ScenarioEvents._removeCondition;

	public class AfterRemoveCondition : ScenarioEvent<AfterRemoveCondition.Parameters>
	{
		public class Parameters(Figure figure, ConditionModel condition, AbilityState potentialAbilityState) : ParametersBase
		{
			public Figure Figure { get; } = figure;
			public ConditionModel Condition { get; } = condition;
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
		}
	}

	private readonly AfterRemoveCondition _afterRemoveCondition = new AfterRemoveCondition();
	public static AfterRemoveCondition AfterRemoveConditionEvent => GameController.Instance.ScenarioEvents._afterRemoveCondition;

	public class DuringGrant : ScenarioEvent<DuringGrant.Parameters>
	{
		public class Parameters(GrantAbility.State abilityState) : ParametersBase<GrantAbility.State>(abilityState)
		{
		}
	}

	private readonly DuringGrant _duringGrant = new DuringGrant();
	public static DuringGrant DuringGrantEvent => GameController.Instance.ScenarioEvents._duringGrant;

	public class DuringControl : ScenarioEvent<DuringControl.Parameters>
	{
		public class Parameters(ControlAbility.State abilityState) : ParametersBase<ControlAbility.State>(abilityState)
		{
		}
	}

	private readonly DuringControl _duringControl = new DuringControl();
	public static DuringControl DuringControlEvent => GameController.Instance.ScenarioEvents._duringControl;

	public class SufferDamage : ScenarioEvent<SufferDamage.Parameters>
	{
		public class Parameters : ParametersBase
		{
			public AbilityState PotentialAbilityState { get; }
			public Figure Figure { get; }
			public Figure PotentialDamageDealer { get; }
			public int InitialDamage { get; }

			public int CalculatedCurrentDamage { get; private set; }

			public int Shield { get; private set; } = 0;
			public int UnpierceableShield { get; private set; } = 0;

			public bool DamagePrevented { get; private set; }

			public bool HasWard { get; private set; }
			public bool HasBrittle { get; private set; }

			public bool FromAttack { get; }

			public bool WouldSufferDamage => CalculatedCurrentDamage > 0 && !DamagePrevented;
			public int TotalShield => Shield + UnpierceableShield;

			public Parameters(AbilityState abilityState, Figure figure, Figure potentialDamageDealer, int initialDamage, bool fromAttack)
			{
				PotentialAbilityState = abilityState;
				Figure = figure;
				PotentialDamageDealer = potentialDamageDealer;
				InitialDamage = initialDamage;
				FromAttack = fromAttack;

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

			public void AdjustPierce(int amount)
			{
				((AttackAbility.State)PotentialAbilityState).SingleTargetAdjustPierce(amount);

				CalculateCurrentDamage();
			}

			private void CalculateCurrentDamage()
			{
				if(DamagePrevented)
				{
					CalculatedCurrentDamage = 0;
					return;
				}

				int finalShieldValue = 0;
				if(FromAttack)
				{
					bool ignoresShield = ((AttackAbility.State)PotentialAbilityState).SingleTargetIgnoresAllShields;

					int finalPierce = Mathf.Max(((AttackAbility.State)PotentialAbilityState).SingleTargetPierce, 0);
					finalShieldValue = ignoresShield ? 0 : Mathf.Max(Shield - finalPierce, 0) + UnpierceableShield;
				}

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
		public class Parameters(Figure figure, int damage, AbilityState abilityState, SufferDamage.Parameters sufferDamageParameters) : ParametersBase
		{
			public Figure Figure { get; } = figure;
			public int Damage { get; } = damage;
			public AbilityState PotentialAbilityState { get; } = abilityState;
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
		public class Parameters(Figure figure, int damageDealt, int damageSuffered, AbilityState abilityState, SufferDamage.Parameters sufferDamageParameters) : ParametersBase
		{
			public Figure Figure { get; } = figure;
			public int DamageDealt { get; } = damageDealt;
			public int DamageSuffered { get; } = damageSuffered;
			public AbilityState PotentialAbilityState { get; } = abilityState;
			public SufferDamage.Parameters SufferDamageParameters { get; } = sufferDamageParameters;
		}
	}

	private readonly AfterSufferDamage _afterSufferDamage = new AfterSufferDamage();
	public static AfterSufferDamage AfterSufferDamageEvent => GameController.Instance.ScenarioEvents._afterSufferDamage;

	public class LosingCardToNegateDamage : ScenarioEvent<LosingCardToNegateDamage.Parameters>
	{
		public class Parameters(Character character, AbilityCard abilityCard, SufferDamage.Parameters sufferDamageParameters) : ParametersBase
		{
			public Character Character { get; } = character;
			public AbilityCard AbilityCard { get; } = abilityCard;
			public SufferDamage.Parameters SufferDamageParameters { get; } = sufferDamageParameters;

			public bool Prevented { get; private set; }

			public void SetPrevented()
			{
				Prevented = true;
			}
		}
	}

	private readonly LosingCardToNegateDamage _losingCardToNegateDamage = new LosingCardToNegateDamage();
	public static LosingCardToNegateDamage LosingCardToNegateDamageEvent => GameController.Instance.ScenarioEvents._losingCardToNegateDamage;

	public class BeforeFigureKilled : ScenarioEvent<BeforeFigureKilled.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure figure) : ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Figure { get; } = figure;
			public bool Prevented { get; private set; } = false;

			public void SetPrevented()
			{
				Prevented = true;
			}
		}
	}

	private readonly BeforeFigureKilled _beforeFigureKilled = new BeforeFigureKilled();
	public static BeforeFigureKilled BeforeFigureKilledEvent => GameController.Instance.ScenarioEvents._beforeFigureKilled;

	public class FigureKilled : ScenarioEvent<FigureKilled.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure figure, Figure potentialKiller) : ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Figure { get; } = figure;
			public Figure PotentialKiller { get; } = potentialKiller;
		}
	}

	private readonly FigureKilled _figureKilled = new FigureKilled();
	public static FigureKilled FigureKilledEvent => GameController.Instance.ScenarioEvents._figureKilled;

	// public class FigureInitialized : ScenarioEvent<FigureInitialized.Parameters>
	// {
	// 	public class Parameters(Figure figure) : ParametersBase
	// 	{
	// 		public Figure Figure { get; } = figure;
	// 	}
	// }
	//
	// private readonly FigureInitialized _figureInitialized = new FigureInitialized();
	// public static FigureInitialized FigureInitializedEvent => GameController.Instance.ScenarioEvents._figureInitialized;

	public class FigureRegistered : ScenarioEvent<FigureRegistered.Parameters>
	{
		public class Parameters(Figure figure) : ParametersBase
		{
			public Figure Figure { get; } = figure;
		}
	}

	private readonly FigureRegistered _figureRegistered = new FigureRegistered();
	public static FigureRegistered FigureRegisteredEvent => GameController.Instance.ScenarioEvents._figureRegistered;

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

			public void SetCannotMoveFurther(bool cannotMoveFurther)
			{
				CanMoveFurther = !cannotMoveFurther;
			}
		}
	}

	private readonly CanMoveFurtherCheck _canMoveFurtherCheck = new CanMoveFurtherCheck();
	public static CanMoveFurtherCheck CanMoveFurtherCheckEvent => GameController.Instance.ScenarioEvents._canMoveFurtherCheck;

	public class FigureExitingHex : ScenarioEvent<FigureExitingHex.Parameters>
	{
		public class Parameters(AbilityState abilityState, Figure figure)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = abilityState;
			public Figure Figure { get; } = figure;

			public Hex Hex => Figure.Hex;
		}
	}

	private readonly FigureExitingHex _figureExitingHex = new FigureExitingHex();
	public static FigureExitingHex FigureExitingHexEvent => GameController.Instance.ScenarioEvents._figureExitingHex;

	public class FigureEnteredHex : ScenarioEvent<FigureEnteredHex.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure figure) : ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Figure { get; } = figure;

			public Hex Hex => Figure.Hex;
		}
	}

	private readonly FigureEnteredHex _figureEnteredHex = new FigureEnteredHex();
	public static FigureEnteredHex FigureEnteredHexEvent => GameController.Instance.ScenarioEvents._figureEnteredHex;

	public class MoveTogether : ScenarioEvent<MoveTogether.Parameters>
	{
		public class Parameters(AbilityState abilityState, Figure performer, Hex destinationHex)
			: ParametersBase
		{
			public AbilityState AbilityState { get; } = abilityState;
			public Figure Performer { get; } = performer;
			public Hex DestinationHex { get; } = destinationHex;

			public List<Figure> OtherFigures { get; } = new List<Figure>();

			public bool TriggerHexEffects { get; private set; } = true;

			public void AddOtherFigure(Figure otherFigure)
			{
				OtherFigures.Add(otherFigure);
			}

			public void SetTriggerHexEffects(bool triggerHexEffects)
			{
				TriggerHexEffects = triggerHexEffects;
			}
		}
	}

	private readonly MoveTogether _moveTogether = new MoveTogether();
	public static MoveTogether MoveTogetherEvent => GameController.Instance.ScenarioEvents._moveTogether;

	public class HazardousTerrainTriggered : ScenarioEvent<HazardousTerrainTriggered.Parameters>
	{
		public class Parameters(
			AbilityState potentialAbilityState, Hex hex, Figure figure, HazardousTerrain hazardousTerrain, bool affectedByHazardousTerrain)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Hex Hex { get; } = hex;
			public Figure Figure { get; } = figure;
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

	public class DifficultTerrainTriggered : ScenarioEvent<DifficultTerrainTriggered.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Figure figure, Hex hex, DifficultTerrain difficultTerrain)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Figure Figure { get; } = figure;
			public Hex Hex { get; } = hex;
			public DifficultTerrain DifficultTerrain { get; } = difficultTerrain;
			public bool AffectedByDifficultTerrain { get; private set; } = true;

			public void SetAffectedByDifficultTerrain(bool affectedByDifficultTerrain)
			{
				AffectedByDifficultTerrain = affectedByDifficultTerrain;
			}
		}
	}

	private readonly DifficultTerrainTriggered _difficultTerrainTriggered = new DifficultTerrainTriggered();
	public static DifficultTerrainTriggered DifficultTerrainTriggeredEvent => GameController.Instance.ScenarioEvents._difficultTerrainTriggered;

	public class TrapTriggered : ScenarioEvent<TrapTriggered.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Hex hex, Trap trap, Figure figure, bool triggersTrap)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Hex Hex { get; } = hex;
			public Trap Trap { get; } = trap;
			public Figure Figure { get; } = figure;
			public bool TriggersTrap { get; private set; } = triggersTrap;

			public void SetTriggersTrap(bool triggersTrap)
			{
				TriggersTrap = triggersTrap;
			}

			public void AdjustTrapDamage(int damage)
			{
				Trap.SetTrapDamage(Trap.Damage + damage);
			}
		}
	}

	private readonly TrapTriggered _trapTriggered = new TrapTriggered();
	public static TrapTriggered TrapTriggeredEvent => GameController.Instance.ScenarioEvents._trapTriggered;

	public class TrapDisarmed : ScenarioEvent<TrapDisarmed.Parameters>
	{
		public class Parameters(Trap trap, Figure potentialDisarmer)
			: ParametersBase
		{
			public Trap Trap { get; } = trap;
			public Figure PotentialDisarmer { get; } = potentialDisarmer;
		}
	}

	private readonly TrapDisarmed _trapDisarmed = new TrapDisarmed();
	public static TrapDisarmed TrapDisarmedEvent => GameController.Instance.ScenarioEvents._trapDisarmed;

	public class ElementInfused : ScenarioEvent<ElementInfused.Parameters>
	{
		public class Parameters(AbilityState potentialAbilityState, Element element, Figure potentialInfuser)
			: ParametersBase
		{
			public AbilityState PotentialAbilityState { get; } = potentialAbilityState;
			public Element Element { get; } = element;
			public Figure PotentialInfuser { get; } = potentialInfuser;
		}
	}

	private readonly ElementInfused _elementInfused = new ElementInfused();
	public static ElementInfused ElementInfusedEvent => GameController.Instance.ScenarioEvents._elementInfused;

	public class FinishElementInfused : ScenarioEvent<FinishElementInfused.Parameters>
	{
		public class Parameters : ParametersBase
		{
		}
	}

	private readonly FinishElementInfused _finishElementInfused = new FinishElementInfused();
	public static FinishElementInfused FinishElementInfusedEvent => GameController.Instance.ScenarioEvents._finishElementInfused;

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
	public static ConsumeElement ConsumeElementEvent => GameController.Instance.ScenarioEvents._consumeElement;

	public class InfuseElement : ScenarioEvent<InfuseElement.Parameters>
	{
		public class Parameters(Element element, AbilityState state, Figure authority)
			: ParametersBase
		{
			public Figure Authority { get; private set; } = authority;
			public Element Element { get; } = element;
			public AbilityState AbilityState { get; } = state;
			public bool CanInfuse { get; private set; } = true;

			public void SetCanInfuse(bool canInfuse)
			{
				CanInfuse = canInfuse;
			}
		}
	}

	private readonly InfuseElement _infuseElement = new InfuseElement();
	public static InfuseElement InfuseElementEvent => GameController.Instance.ScenarioEvents._infuseElement;

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

	public class OverlayTileCreated : ScenarioEvent<OverlayTileCreated.Parameters>
	{
		public class Parameters(OverlayTile overlayTile) : ParametersBase
		{
			public OverlayTile OverlayTile = overlayTile;
		}
	}

	private readonly OverlayTileCreated _overlayTileCreated = new OverlayTileCreated();
	public static OverlayTileCreated OverlayTileCreatedEvent => GameController.Instance.ScenarioEvents._overlayTileCreated;

	public class OverlayTileMoved : ScenarioEvent<OverlayTileMoved.Parameters>
	{
		public class Parameters(OverlayTile overlayTile) : ParametersBase
		{
			public OverlayTile OverlayTile = overlayTile;
		}
	}

	private readonly OverlayTileMoved _overlayTileMoved = new OverlayTileMoved();
	public static OverlayTileMoved OverlayTileMovedEvent => GameController.Instance.ScenarioEvents._overlayTileMoved;

	public class ShortRestStarted : ScenarioEvent<ShortRestStarted.Parameters>
	{
		public class Parameters(Character character)
			: ParametersBase
		{
			public Character Character { get; } = character;

			public bool CanSelectCardToLose { get; private set; } = false;
			public bool LoseCard { get; private set; } = true;

			public void SetCanSelectCardToUse()
			{
				CanSelectCardToLose = true;
			}

			public void SetLoseCard(bool loseCard)
			{
				LoseCard = loseCard;
			}
		}
	}

	private readonly ShortRestStarted _shortRestStarted = new ShortRestStarted();
	public static ShortRestStarted ShortRestStartedEvent => GameController.Instance.ScenarioEvents._shortRestStarted;

	public class LongRestStarted : ScenarioEvent<LongRestStarted.Parameters>
	{
		public class Parameters(Character character) : ParametersBase
		{
			public Character Character { get; } = character;
			public bool LoseCard { get; private set; } = true;

			public void SetLoseCard(bool loseCard)
			{
				LoseCard = loseCard;
			}
		}
	}

	private readonly LongRestStarted _longRestStarted = new LongRestStarted();
	public static LongRestStarted LongRestStartedEvent => GameController.Instance.ScenarioEvents._longRestStarted;

	public class LongRestCardSelection : ScenarioEvent<LongRestCardSelection.Parameters>
	{
		public class Parameters(Character character) : ParametersBase
		{
			public Character Character { get; } = character;
		}
	}

	private readonly LongRestCardSelection _longRestCardSelection = new LongRestCardSelection();
	public static LongRestCardSelection LongRestCardSelectionEvent => GameController.Instance.ScenarioEvents._longRestCardSelection;

	public class LongRestEnded : ScenarioEvent<LongRestEnded.Parameters>
	{
		public class Parameters(Character character) : ParametersBase
		{
			public Character Character { get; } = character;
		}
	}

	private readonly LongRestEnded _longRestEnded = new LongRestEnded();
	public static LongRestEnded LongRestEndedEvent => GameController.Instance.ScenarioEvents._longRestEnded;

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
		public class Parameters(AbilityCardSide abilityCardSide, Figure performer, CardState resultingState) : ParametersBase
		{
			public AbilityCardSide AbilityCardSide { get; } = abilityCardSide;
			public Figure Performer { get; } = performer;
			public CardState ResultingState { get; } = resultingState;
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
		}
	}

	private readonly CardSideSelection _cardSideSelectionStarted = new CardSideSelection();
	public static CardSideSelection CardSideSelectionEvent => GameController.Instance.ScenarioEvents._cardSideSelectionStarted;

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

	public static FigureTurnEndedConditionsFallOff FigureTurnEndedConditionsFallOffEvent =>
		GameController.Instance.ScenarioEvents._figureTurnEndedConditionsFallOff;

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

	public static RoundStartBeforeCardSelection RoundStartBeforeCardSelectionEvent =>
		GameController.Instance.ScenarioEvents._roundStartBeforeCardSelection;

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

	public static RoundStartedBeforeInitiativesSorted RoundStartedBeforeInitiativesSortedEvent =>
		GameController.Instance.ScenarioEvents._roundStartedBeforeInitiativesSorted;

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
		public class Parameters(Room room, Door openedDoor, Figure potentialOpener)
			: ParametersBase
		{
			public Room Room { get; } = room;
			public Door OpenedDoor { get; } = openedDoor;
			public Figure PotentialOpener { get; } = potentialOpener;
		}
	}

	private readonly RoomRevealed _roomRevealed = new RoomRevealed();
	public static RoomRevealed RoomRevealedEvent => GameController.Instance.ScenarioEvents._roomRevealed;

	public class DoorOpened : ScenarioEvent<DoorOpened.Parameters>
	{
		public class Parameters(Door openedDoor, Figure potentialOpener)
			: ParametersBase
		{
			public Door OpenedDoor { get; } = openedDoor;
			public Figure PotentialOpener { get; } = potentialOpener;
		}
	}

	private readonly DoorOpened _doorOpened = new DoorOpened();
	public static DoorOpened DoorOpenedEvent => GameController.Instance.ScenarioEvents._doorOpened;

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

	public class SwingDirectionCheck : ScenarioEvent<SwingDirectionCheck.Parameters>
	{
		public class Parameters(AbilityState abilityState) : ParametersBase<AbilityState>(abilityState)
		{
			public SwingDirectionType? RequiredDirection { get; private set; } = null;

			public void SetRequiredSwingDirection(SwingDirectionType requiredDirection)
			{
				RequiredDirection = requiredDirection;
			}
		}
	}

	private readonly SwingDirectionCheck _swingDirectionCheck = new SwingDirectionCheck();
	public static SwingDirectionCheck SwingDirectionCheckEvent => GameController.Instance.ScenarioEvents._swingDirectionCheck;

	public class FigureFoundFocus : ScenarioEvent<FigureFoundFocus.Parameters>
	{
		public class Parameters(AbilityState abilityState, Figure focus)
			: ParametersBase<AbilityState>(abilityState)
		{
			public Figure Focus { get; private set; } = focus;
			public Hex FocusHex { get; private set; } = focus?.Hex;

			public void SetNewFocus(Figure newFocus)
			{
				Focus = newFocus;
				FocusHex = newFocus?.Hex;
			}

			public void SetFocusHex(Hex hex)
			{
				FocusHex = hex;
			}
		}
	}

	private readonly FigureFoundFocus _figureFoundFocus = new FigureFoundFocus();
	public static FigureFoundFocus FigureFoundFocusEvent => GameController.Instance.ScenarioEvents._figureFoundFocus;

	public class NextActiveFigure : ScenarioEvent<NextActiveFigure.Parameters>
	{
		public class Parameters(Figure previousActiveFigure, Figure nextActiveFigure)
			: ParametersBase
		{
			public Figure PreviousActiveFigure { get; private set; } = previousActiveFigure;
			public Figure NextActiveFigure { get; private set; } = nextActiveFigure;

			public bool SortingRequired { get; private set; } = false;

			public void SetSortingRequired()
			{
				SortingRequired = true;
			}
		}
	}

	private readonly NextActiveFigure _nextActiveFigure = new NextActiveFigure();
	public static NextActiveFigure NextActiveFigureEvent => GameController.Instance.ScenarioEvents._nextActiveFigure;

	public class ScenarioEnded : ScenarioEvent<ScenarioEnded.Parameters>
	{
		public class Parameters(bool win)
			: ParametersBase
		{
			public bool Win { get; } = win;
		}
	}

	private readonly ScenarioEnded _scenarioEnded = new ScenarioEnded();
	public static ScenarioEnded ScenarioEndedEvent => GameController.Instance.ScenarioEvents._scenarioEnded;

	public class CoinSpawned : ScenarioEvent<CoinSpawned.Parameters>
	{
		public class Parameters(Figure potentialDropper, Coin coin)
			: ParametersBase
		{
			public Figure PotentialDropper { get; } = potentialDropper;
			public Coin Coin { get; } = coin;
		}
	}

	private readonly CoinSpawned _coinSpawned = new CoinSpawned();
	public static CoinSpawned CoinSpawnedEvent => GameController.Instance.ScenarioEvents._coinSpawned;

	public class CoinLooted : ScenarioEvent<CoinLooted.Parameters>
	{
		public class Parameters(Figure lootObtainer, Coin coin)
			: ParametersBase
		{
			public Figure LootObtainer { get; } = lootObtainer;
			public Coin Coin { get; } = coin;
		}
	}

	private readonly CoinLooted _coinLooted = new CoinLooted();
	public static CoinLooted CoinLootedEvent => GameController.Instance.ScenarioEvents._coinLooted;

	public class LootableObjectLooted : ScenarioEvent<LootableObjectLooted.Parameters>
	{
		public class Parameters(Figure lootObtainer, LootableObject lootableObject)
			: ParametersBase
		{
			public Figure LootObtainer { get; } = lootObtainer;
			public LootableObject LootableObject { get; } = lootableObject;
		}
	}

	private readonly LootableObjectLooted _lootableObjectLooted = new LootableObjectLooted();
	public static LootableObjectLooted LootableObjectLootedEvent => GameController.Instance.ScenarioEvents._lootableObjectLooted;

	public class GainedExperience : ScenarioEvent<GainedExperience.Parameters>
	{
		public class Parameters(Figure experienceReceiver, int experienceAmount)
			: ParametersBase
		{
			public Figure ExperienceReceiver { get; } = experienceReceiver;
			public int ExperienceAmount { get; } = experienceAmount;
		}
	}

	private readonly GainedExperience _gainedExperience = new GainedExperience();
	public static GainedExperience GainedExperienceEvent => GameController.Instance.ScenarioEvents._gainedExperience;

	public class InflictConditionEventReward : ScenarioEvent<InflictConditionEventReward.Parameters>
	{
		public class Parameters(Character character, ConditionModel conditionModel)
			: ParametersBase
		{
			public Character Character { get; } = character;
			public ConditionModel ConditionModel { get; } = conditionModel;

			public bool Prevented { get; private set; }

			public void SetPrevented(bool prevented)
			{
				Prevented = prevented;
			}
		}
	}

	private readonly InflictConditionEventReward _inflictConditionEventReward = new InflictConditionEventReward();
	public static InflictConditionEventReward InflictConditionEventRewardEvent => GameController.Instance.ScenarioEvents._inflictConditionEventReward;

	public class SufferDamageEventReward : ScenarioEvent<SufferDamageEventReward.Parameters>
	{
		public class Parameters(Character character)
			: ParametersBase
		{
			public Character Character { get; } = character;

			public bool Prevented { get; private set; }

			public void SetPrevented(bool prevented)
			{
				Prevented = prevented;
			}
		}
	}

	private readonly SufferDamageEventReward _sufferDamageEventReward = new SufferDamageEventReward();
	public static SufferDamageEventReward SufferDamageEventRewardEvent => GameController.Instance.ScenarioEvents._sufferDamageEventReward;

	public class AddMinusOnesEventReward : ScenarioEvent<AddMinusOnesEventReward.Parameters>
	{
		public class Parameters(Character character)
			: ParametersBase
		{
			public Character Character { get; } = character;

			public bool Prevented { get; private set; }

			public void SetPrevented(bool prevented)
			{
				Prevented = prevented;
			}
		}
	}

	private readonly AddMinusOnesEventReward _addMinusOnesEventReward = new AddMinusOnesEventReward();
	public static AddMinusOnesEventReward AddMinusOnesEventRewardEvent => GameController.Instance.ScenarioEvents._addMinusOnesEventReward;
}