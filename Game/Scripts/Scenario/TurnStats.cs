using System.Collections.Generic;
using Fractural.Tasks;

public static class TurnStats
{
    private static readonly Dictionary<Figure, int> HexesMoved = new();
    private static readonly Dictionary<Figure, int> DamageDealt = new();

    public static int GetHexesMoved(Figure figure) => HexesMoved.GetValueOrDefault(figure);
    public static int GetDamageDealt(Figure figure) => DamageDealt.GetValueOrDefault(figure);

    public static void Track(Figure figure)
    {
        HexesMoved[figure] = 0;
        DamageDealt[figure] = 0;

        ScenarioEvents.FigureTurnStartedEvent.Subscribe(figure, figure,
            parameters => parameters.Figure == figure,
            async parameters =>
            {
                HexesMoved[figure] = 0;
                DamageDealt[figure] = 0;
                await GDTask.CompletedTask;
            });

        ScenarioEvents.FigureEnteredHexEvent.Subscribe(figure, figure,
            parameters => parameters.Figure == figure && !parameters.ForcedMovement,
            async parameters =>
            {
                HexesMoved[figure] = GetHexesMoved(figure) + 1;
                await GDTask.CompletedTask;
            });

        ScenarioEvents.AfterSufferDamageEvent.Subscribe(figure, figure,
            parameters => parameters.PotentialAbilityState?.Performer == figure,
            async parameters =>
            {
                DamageDealt[figure] = GetDamageDealt(figure) + parameters.DamageDealt;
                await GDTask.CompletedTask;
            });
    }
}