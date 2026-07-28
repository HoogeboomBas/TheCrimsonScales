using System.Collections.Generic;
using System.Linq;

public class HordeOfBones : SpiritCallerCardModel<HordeOfBones.CardTop, HordeOfBones.CardBottom>
{
	public override string Name => "Horde of Bones";
	public override int Level => 1;
	public override int Initiative => 79;
	protected override int AtlasIndex => 28 - 4;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SpawnAbility.Builder()
				.WithName("Skeletal Archer")
				.WithTexturePath("res://Content/Classes/SpiritCaller/Summons/skeletal_archer.png")
				.WithHealth(2)
				.WithMove(2)
				.WithAttack(1)
				.WithRange(2)
				.WithTraits(new TargetsTrait(2), new PierceTrait(1))
				.Build()
			)
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex swapped = await AbilityCmd.SelectHex(state, list =>
					{
						list.AddRange(GameController.Instance.Map.Hexes
							.Select(hexPair => hexPair.Value)
							.Where(hex => Map.SimpleDistance(state.Performer.Hex.Coords, hex.Coords) <= 3 &&
							              hex.HasHexObjectOfType<Coin>() &&
							              AbilityCmd.CanSwap(state, state.Performer, hex.GetHexObjectOfType<Coin>())));
					}, mandatory: false, "Select a coin token to swap hexes with.");

					if(swapped == null)
					{
						return;
					}

					if(await AbilityCmd.TrySwap(state, state.Performer, swapped.GetHexObjectOfType<Coin>()))
					{
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark))
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(LootAbility.Builder()
					.WithRange(1)
					.WithCustomGetLootObtainer(state => state.ActionState.ParentActionState.Performer)
					.Build())
				.WithCustomGetTargets((state, list) =>
				{
					list.Add(state.Performer);
					list.AddRange(Spirit.GetAllSpirits());
				})
				.WithTarget(Target.Any)
				.WithCanTargetNonFigures()
				.Build())
		];
	}
}