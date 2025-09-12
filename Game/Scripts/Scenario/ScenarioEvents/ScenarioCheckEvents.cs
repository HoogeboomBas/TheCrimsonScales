using System.Collections.Generic;

public class ScenarioCheckEvents
{
	public class AIMoveParametersCheck : ScenarioCheckEvent<AIMoveParametersCheck.Parameters>
	{
		public class Parameters(Figure performer, AIMoveParameters aiMoveParameters)
			: ParametersBase
		{
			public Figure Performer { get; } = performer;
			public AIMoveParameters AIMoveParameters { get; } = aiMoveParameters;

			public void AdjustTargets(int amount)
			{
				AIMoveParameters.Targets += amount;
			}

			public void AdjustRange(int amount)
			{
				AIMoveParameters.Range += amount;
			}
		}
	}

	private readonly AIMoveParametersCheck _aiMoveParametersCheck = new AIMoveParametersCheck();
	public static AIMoveParametersCheck AIMoveParametersCheckEvent => GameController.Instance.ScenarioCheckEvents._aiMoveParametersCheck;

	public class MoveCheck : ScenarioCheckEvent<MoveCheck.Parameters>
	{
		public class Parameters(AbilityState abilityState, Figure performer, Hex hex, Hex fromHex, int moveCost, bool affectedByNegativeHex)
			: ParametersBase
		{
			public AbilityState AbilityState { get; } = abilityState;

			public Figure Performer { get; } = performer;
			public Hex Hex { get; } = hex;
			public Hex FromHex { get; } = fromHex;

			public int MoveCost { get; private set; } = moveCost;
			public bool AffectedByNegativeHex { get; private set; } = affectedByNegativeHex;

			public void SetMoveCost(int moveCost)
			{
				MoveCost = moveCost;
			}

			public void SetAffectedByNegativeHex(bool affectedByNegativeHex)
			{
				AffectedByNegativeHex = affectedByNegativeHex;
			}
		}
	}

	private readonly MoveCheck _moveCheck = new MoveCheck();
	public static MoveCheck MoveCheckEvent => GameController.Instance.ScenarioCheckEvents._moveCheck;

	public class CanEnterObstacleCheck : ScenarioCheckEvent<CanEnterObstacleCheck.Parameters>
	{
		public class Parameters(Figure figure, Hex hex, Obstacle obstacle, bool tryingToStopAt)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;
			public Hex Hex { get; } = hex;
			public Obstacle Obstacle { get; } = obstacle;
			public bool TryingToStopAt { get; } = tryingToStopAt;

			public bool CanEnter { get; private set; } = false;

			public void SetCanEnter()
			{
				CanEnter = true;
			}
		}
	}

	private readonly CanEnterObstacleCheck _canEnterObstacleCheck = new CanEnterObstacleCheck();
	public static CanEnterObstacleCheck CanEnterObstacleCheckEvent => GameController.Instance.ScenarioCheckEvents._canEnterObstacleCheck;

	public class CanPassEnemyCheck : ScenarioCheckEvent<CanPassEnemyCheck.Parameters>
	{
		public class Parameters(AbilityState abilityState, Figure figure, Figure enemyFigure)
			: ParametersBase
		{
			public AbilityState AbilityState { get; } = abilityState;
			public Figure Figure { get; } = figure;
			public Figure EnemyFigure { get; } = enemyFigure;

			public bool CanPass { get; private set; } = false;

			public void SetCanPass()
			{
				CanPass = true;
			}
		}
	}

	private readonly CanPassEnemyCheck _canPassEnemyCheck = new CanPassEnemyCheck();
	public static CanPassEnemyCheck CanPassEnemyCheckEvent => GameController.Instance.ScenarioCheckEvents._canPassEnemyCheck;

	public class MoveCanStopAtCheck : ScenarioCheckEvent<MoveCanStopAtCheck.Parameters>
	{
		public class Parameters(MoveAbility.State abilityState, Hex hex)
			: ParametersBase
		{
			public MoveAbility.State AbilityState { get; } = abilityState;

			public Figure Performer { get; } = abilityState.Performer;
			public Hex Hex { get; } = hex;

			public bool CanStopAt { get; private set; } = true;

			public void SetCannotStopAt()
			{
				CanStopAt = false;
			}
		}
	}

	private readonly MoveCanStopAtCheck _moveCanStopAtCheck = new MoveCanStopAtCheck();
	public static MoveCanStopAtCheck MoveCanStopAtCheckEvent => GameController.Instance.ScenarioCheckEvents._moveCanStopAtCheck;

	public class ShieldCheck : ScenarioCheckEvent<ShieldCheck.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public int Shield { get; private set; } = 0;
			public bool ExtraValue { get; private set; } = false;

			public void AdjustShield(int amount)
			{
				Shield += amount;
			}

			public void SetExtraValue()
			{
				ExtraValue = true;
			}
		}
	}

	private readonly ShieldCheck _shieldCheck = new ShieldCheck();
	public static ShieldCheck ShieldCheckEvent => GameController.Instance.ScenarioCheckEvents._shieldCheck;

	public class RetaliateCheck : ScenarioCheckEvent<RetaliateCheck.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public List<(int, int)> RetaliateValues { get; } = new List<(int, int)>();

			public void AddRetaliate(int amount, int range)
			{
				RetaliateValues.Add((amount, range));
			}
		}
	}

	private readonly RetaliateCheck _retaliateCheck = new RetaliateCheck();
	public static RetaliateCheck RetaliateCheckEvent => GameController.Instance.ScenarioCheckEvents._retaliateCheck;

	public class FlyingCheck : ScenarioCheckEvent<FlyingCheck.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public bool HasFlying { get; private set; }

			public void SetFlying()
			{
				HasFlying = true;
			}
		}
	}

	private readonly FlyingCheck _flyingCheck = new FlyingCheck();
	public static FlyingCheck FlyingCheckEvent => GameController.Instance.ScenarioCheckEvents._flyingCheck;

	public class PierceCheck : ScenarioCheckEvent<PierceCheck.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public int Pierce { get; private set; } = 0;

			public void AdjustPierce(int amount)
			{
				Pierce += amount;
			}
		}
	}

	private readonly PierceCheck _pierceCheck = new PierceCheck();
	public static PierceCheck PierceCheckEvent => GameController.Instance.ScenarioCheckEvents._pierceCheck;

	public class TargetsCheck : ScenarioCheckEvent<TargetsCheck.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public int Targets { get; private set; } = 1;

			public void AdjustTargets(int amount)
			{
				Targets += amount;
			}
		}
	}

	private readonly TargetsCheck _targetsCheck = new TargetsCheck();
	public static TargetsCheck TargetsCheckEvent => GameController.Instance.ScenarioCheckEvents._targetsCheck;

	public class ImmunitiesVisualCheck : ScenarioCheckEvent<ImmunitiesVisualCheck.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public List<ConditionModel> Immunities { get; } = new List<ConditionModel>();

			public void AddImmunity(ConditionModel conditionModel)
			{
				Immunities.AddIfNew(conditionModel);
			}
		}
	}

	private readonly ImmunitiesVisualCheck _immunitiesVisualCheck = new ImmunitiesVisualCheck();
	public static ImmunitiesVisualCheck ImmunitiesVisualCheckEvent => GameController.Instance.ScenarioCheckEvents._immunitiesVisualCheck;

	public class AppliesVisualCheck : ScenarioCheckEvent<AppliesVisualCheck.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public List<ConditionModel> ConditionModels { get; } = new List<ConditionModel>();

			public void AddConditionModel(ConditionModel conditionModel)
			{
				ConditionModels.AddIfNew(conditionModel);
			}
		}
	}

	private readonly AppliesVisualCheck _appliesVisualCheck = new AppliesVisualCheck();
	public static AppliesVisualCheck AppliesVisualCheckEvent => GameController.Instance.ScenarioCheckEvents._appliesVisualCheck;

	public class FigureInfoItemExtraEffectsCheck : ScenarioCheckEvent<FigureInfoItemExtraEffectsCheck.Parameters>
	{
		public class Parameters(Figure figure)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public List<FigureInfoExtraEffectParameters> FigureInfoExtraEffectsParameters { get; } = new List<FigureInfoExtraEffectParameters>();

			public void Add(FigureInfoExtraEffectParameters figureInfoExtraEffectParameters)
			{
				FigureInfoExtraEffectsParameters.Add(figureInfoExtraEffectParameters);
			}
		}
	}

	private readonly FigureInfoItemExtraEffectsCheck _figureInfoItemExtraEffectsCheck = new FigureInfoItemExtraEffectsCheck();
	public static FigureInfoItemExtraEffectsCheck FigureInfoItemExtraEffectsCheckEvent => GameController.Instance.ScenarioCheckEvents._figureInfoItemExtraEffectsCheck;

	public class CanBeFocusedCheck : ScenarioCheckEvent<CanBeFocusedCheck.Parameters>
	{
		public class Parameters(Figure performer, Figure potentialTarget)
			: ParametersBase
		{
			public Figure Performer { get; } = performer;
			public Figure PotentialTarget { get; } = potentialTarget;

			public bool CanBeFocused { get; private set; } = true;

			public void SetCannotBeFocused()
			{
				CanBeFocused = false;
			}
		}
	}

	private readonly CanBeFocusedCheck _canBeFocusedCheck = new CanBeFocusedCheck();
	public static CanBeFocusedCheck CanBeFocusedCheckEvent => GameController.Instance.ScenarioCheckEvents._canBeFocusedCheck;

	public class CanBeTargetedCheck : ScenarioCheckEvent<CanBeTargetedCheck.Parameters>
	{
		public class Parameters(Figure performer, Figure potentialTarget)
			: ParametersBase
		{
			public Figure Performer { get; } = performer;
			public Figure PotentialTarget { get; } = potentialTarget;

			public bool CanBeTargeted { get; private set; } = true;

			public void SetCannotBeTargeted()
			{
				CanBeTargeted = false;
			}
		}
	}

	private readonly CanBeTargetedCheck _canBeTargetedCheck = new CanBeTargetedCheck();
	public static CanBeTargetedCheck CanBeTargetedCheckEvent => GameController.Instance.ScenarioCheckEvents._canBeTargetedCheck;

	public class DisadvantageCheck : ScenarioCheckEvent<DisadvantageCheck.Parameters>
	{
		public class Parameters(Figure target, Figure attacker, Hex attackerHex, bool hasDisadvantage)
			: ParametersBase
		{
			public Figure Target { get; } = target;
			public Figure Attacker { get; } = attacker;

			public Hex AttackerHex { get; } = attackerHex;

			public bool HasDisadvantage { get; private set; } = hasDisadvantage;

			public void SetDisadvantage()
			{
				HasDisadvantage = true;
			}
		}
	}

	private readonly DisadvantageCheck _disadvantageCheck = new DisadvantageCheck();
	public static DisadvantageCheck DisadvantageCheckEvent => GameController.Instance.ScenarioCheckEvents._disadvantageCheck;

	public class InitiativeCheck : ScenarioCheckEvent<InitiativeCheck.Parameters>
	{
		public class Parameters(Figure figure, Initiative initiative)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public Initiative Initiative { get; private set; } = initiative;

			public void AdjustInitiative(int amount)
			{
				Initiative = new Initiative
				{
					MainInitiative = Initiative.MainInitiative + amount,
					SortingInitiative = Initiative.SortingInitiative + amount * 10000000
				};
			}
		}
	}

	private readonly InitiativeCheck _initiativeCheck = new InitiativeCheck();
	public static InitiativeCheck InitiativeCheckEvent => GameController.Instance.ScenarioCheckEvents._initiativeCheck;
}