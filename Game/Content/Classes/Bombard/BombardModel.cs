using System.Collections.Generic;
using Godot;

public class BombardModel : ClassModel
{
	public override string Name => "Bombard";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.High;
	public override int HandSize => 9;
	public override Ancestry Ancestry => Ancestry.Quatryl;

	public override string AssetPath => "res://Content/Classes/Bombard";
	public override Color PrimaryColor => Color.FromHtml("8c683b");
	public override Color SecondaryColor => Color.FromHtml("948572");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Bombard.tscn");

	public override List<AbilityCardModel> AbilityCards { get; } =
	[
		ModelDB.AbilityCard<ConsistentFiring>(),
		ModelDB.AbilityCard<DoubleCannons>(),
		ModelDB.AbilityCard<ExplodingCannonball>(),
		ModelDB.AbilityCard<ForcefulBolt>(),
		ModelDB.AbilityCard<IgnitedLaunch>(),
		ModelDB.AbilityCard<GrapplingHook>(),
		ModelDB.AbilityCard<RollingIntoPosition>(),
		ModelDB.AbilityCard<BarbedArmor>(),
		ModelDB.AbilityCard<UnexpectedBombshell>(),

		ModelDB.AbilityCard<ChainGrapnel>(),
		ModelDB.AbilityCard<ManTheCannon>(),
		ModelDB.AbilityCard<PillarsOfSmoke>(),

		ModelDB.AbilityCard<DistantRetribution>(),
		ModelDB.AbilityCard<RapidFire>(),
		ModelDB.AbilityCard<StationaryEnhancements>(),
		ModelDB.AbilityCard<TwinBlast>(),
		ModelDB.AbilityCard<HurriedRepairs>(),
		ModelDB.AbilityCard<PowerfulBuckshot>(),
	];

	public override List<PerkModel> Perks { get; } =
	[
		ModelDB.Perk<BombardPerks.RemoveTwoMinusOnes>(),

		ModelDB.Perk<BombardPerks.ReplaceOneMinusOneWithOneShieldOneRolling>(),
		ModelDB.Perk<BombardPerks.ReplaceOneMinusOneWithOneShieldOneRolling>(),

		ModelDB.Perk<BombardPerks.ReplaceOnePlusZeroWithOnePlusZeroPlusThreeIfProjectile>(),
		ModelDB.Perk<BombardPerks.ReplaceOnePlusZeroWithOnePlusZeroPlusThreeIfProjectile>(),

		ModelDB.Perk<BombardPerks.ReplaceTwoPlusZeroWithTwoPierceThreeRolling>(),

		ModelDB.Perk<BombardPerks.ReplacePlusZeroWithOnePlusOneWound>(),

		ModelDB.Perk<BombardPerks.ReplacePlusZeroWithOnePlusZeroStun>(),

		ModelDB.Perk<BombardPerks.ReplaceOnePlusOneWithTwoPlusOneRetaliateOne>(),

		ModelDB.Perk<BombardPerks.ReplaceOnePlusOneWithOnePlusZeroStrengthenSelf>(),

		ModelDB.Perk<BombardPerks.AddOnePlusTwoImmobilize>(),
		ModelDB.Perk<BombardPerks.AddOnePlusTwoImmobilize>(),

		ModelDB.Perk<BombardPerks.AddTwoPlusZeroHealOneSelfRolling>(),

		ModelDB.Perk<BombardPerks.IgnoreNegativeScenarioEffectsAddPlusOnePullSelf>(),

		ModelDB.Perk<BombardPerks.IgnoreNegativeItemEffectsAddPlusOnePullSelf>(),

		ModelDB.Perk<BombardPerks.EmergencyEmplacement>(),
	];
}