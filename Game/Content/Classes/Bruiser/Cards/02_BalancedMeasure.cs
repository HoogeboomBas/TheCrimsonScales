using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BalancedMeasure : BruiserCardModel<BalancedMeasure.CardTop, BalancedMeasure.CardBottom>
{
    public override string Name => "Balanced Measure";
    public override int Level => 1;
    public override int Initiative => 20;
    protected override int AtlasIndex => 2;

    private static readonly Dictionary<Figure, int> HexesMoved = new();
    private static readonly Dictionary<Figure, int> DamageDealt = new();

    public class CardTop : BruiserCardSide
    {
        protected override List<AbilityCardAbility> GetAbilities() =>
        [
            new AbilityCardAbility(AttackAbility.Builder()
                .WithDamage(new DynamicInt<AttackAbility.State>(state => TurnStats.GetHexesMoved(state.Performer)))
                .Build())
        ];
    }

    public class CardBottom : BruiserCardSide
    {
        protected override List<AbilityCardAbility> GetAbilities() =>
        [
            new AbilityCardAbility(MoveAbility.Builder()
                .WithDistance(new DynamicInt<MoveAbility.State>(state => TurnStats.GetDamageDealt(state.Performer)))
                .Build())
        ];
    }
}