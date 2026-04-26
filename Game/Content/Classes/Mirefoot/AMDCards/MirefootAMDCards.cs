using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class MirefootAMDCards
{
	public class PlusZero : MirefootAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
	}

	public class PlusOne : MirefootAMDCardModel
	{
		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	}

	public class PlusZeroPlusXWhereXIsTargetPoisonValue : MirefootAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"Add +X{Icons.Inline(Icons.Attack, richTextParameters)}, where X is the {Icons.Inline(Icons.GetCondition(Conditions.Poison1), richTextParameters)} value of the target");

		protected override int AtlasIndex => 3;

		public override int? GetValue(AttackAbility.State attackAbilityState)
		{
			if(attackAbilityState == null)
			{
				return +0;
			}

			if(attackAbilityState.Target.HasCondition(Conditions.Poison4))
			{
				return +4;
			}

			if(attackAbilityState.Target.HasCondition(Conditions.Poison3))
			{
				return +3;
			}

			if(attackAbilityState.Target.HasCondition(Conditions.Poison2))
			{
				return +2;
			}

			if(attackAbilityState.Target.HasCondition(Conditions.Poison1))
			{
				return +1;
			}

			return +0;
		}
	}

	public class PlusZeroCreateDifficultTerrainRolling : MirefootAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Create one 1-hex difficult terrain tile in the featureless hex occupied by the target",
				rolling: true);

		protected override int AtlasIndex => 7;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				Hex hex = await AbilityCmd.SelectHex(state, hexes => hexes.Add(state.Target.Hex),
					hintText: "Place difficult terrain?");
				PackedScene scene = ResourceLoader.Load<PackedScene>(
					GameController.Instance.StateRNG.Randf() > 0.5f
						? "res://Content/Classes/Mirefoot/Bog1H.tscn"
						: "res://Content/Classes/Mirefoot/BrokenLog1H.tscn");
				await AbilityCmd.CreateDifficultTerrain(hex, scene);
			};
	}

	public class PlusTwo : MirefootAMDCardModel
	{
		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
	}

	public class PlusZeroWoundTwo : MirefootAMDCardModel
	{
		protected override int AtlasIndex => 13;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound2];
	}

	public class PlusZeroIfOccupyingDifficultTerrainGainInvisibleRolling : MirefootAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"If you are occupying difficult terrain, gain {Icons.Inline(Icons.GetCondition(Conditions.Invisible), richTextParameters)}",
				rolling: true);

		protected override int AtlasIndex => 15;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				if(state.Performer.Hex.HasHexObjectOfType<DifficultTerrain>())
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Invisible);
				}
			};
	}

	public class PlusZeroIfOccupyingDifficultTerrainPlusOneInsteadRolling : MirefootAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"If you are occupying difficult terrain, {Icons.Inline(Icons.GetAMDValue("+1"))} instead", rolling: true);

		protected override int AtlasIndex => 17;

		public override int? GetValue(AttackAbility.State attackAbilityState) =>
			attackAbilityState?.Performer.Hex.HasHexObjectOfType<DifficultTerrain>() == true ? +1 : +0;
	}
}