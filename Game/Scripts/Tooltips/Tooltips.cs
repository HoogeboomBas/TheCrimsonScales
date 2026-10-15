public static class Tooltips
{
	public static Tooltip XP { get; } = new Tooltip("Experience", Icons.XP, null, textParameters =>
		"Experience measures a character’s growth and defines when they level up. When an action depicts this icon in the lower right corner, if any part of that action is performed, the character gains the indicated amount of experience. Sometimes an ability specifies that experience is only gained under certain conditions, such as consuming an element, meeting a requirement, or advancing a character token between use slots.");

	// public static Tooltip Attack { get; } = new Tooltip("Attack", Icons.Attack, null, textParameters =>
	// 	$"Attack X” is a negative targeted ability that allows a figure to deal a base amount of X damage to an enemy within the specified range. An attack with no specified range value is considered a melee attack with range 1, capable of targeting adjacent enemies only, unless otherwise stated. For example, if an effect provides +Y{Icons.Inline(Icons.Range, textParameters)} to an attack, it would give the attack with no specified range value {Icons.Inline(Icons.Range, textParameters)}1 + Y” and make it a ranged attack.");

	public static Tooltip Attack { get; } = new Tooltip("Attack", Icons.Attack, "Ability", textParameters =>
		$"Attack X is a negative targeted ability that allows a figure to deal a base amount of X damage to an enemy within the specified range. An attack with no specified range value is considered a melee attack with range 1, capable of targeting adjacent enemies only, unless otherwise stated. After a target has been chosen, an attack modifier card is drawn from the attacker’s deck and applied. Once all modifications have been applied, the target suffers the resulting amount of damage.");

	public static Tooltip Move { get; } = new Tooltip("Move", Icons.Move, "Ability", textParameters =>
		"Move X is an ability that gives a figure X movement points. That figure may then spend those movement points to move one hex per movement point. Figures can move through allies but cannot move through enemies, obstacles, objectives, or walls. Traps and other effects of hexes trigger when a figure enters them. A figure must always end their movement in an unoccupied hex. A figure must enter at least one hex to be considered to have performed a move ability.");

	public static Tooltip Jump { get; } = new Tooltip("Jump", Icons.Jump, null, textParameters =>
		"Jump is an added effect of a move ability. Jumping allows a figure to ignore all enemies, obstacles, objectives, traps, and hazardous terrain, except for in the last hex. Difficult terrain is ignored completely when jumping.");

	public static Tooltip Flying { get; } = new Tooltip("Flying", Icons.Flying, null, textParameters =>
		"Flying is an active bonus. Flying allows a figure to completely ignore all enemies, obstacles, objectives, traps, and overlay terrain, including in the last hex. However, a flying figure still cannot end their movement in a hex occupied by another figure (and vice versa). If a flying figure occupies a hex with an immobilize trap or stun trap (see p.29), treat them as an obstacle when determining the movement of other figures. If a figure loses flying, they are treated as if entering their current hex with a normal movement. In such cases, if the figure currently occupies a hex with an obstacle, they instead move into the closest unoccupied hex without an obstacle, triggering any effects of that hex.");

	public static Tooltip Teleport { get; } = new Tooltip("Teleport", Icons.Teleport, "Ability", textParameters =>
		"Teleport X is an ability that transports a figure to any location within X hexes of their current hex, without evaluating their path through all of the hexes between. A teleport ability is not considered a move ability, and teleporting is not considered movement. Teleportation is not affected by anything, even walls, in the hexes between. A figure can only teleport to a valid hex, and any effects of entering that hex still trigger normally, unless otherwise stated. A figure cannot teleport into an unrevealed room, but they can teleport to a hex with a closed door, opening the door as a result.");

	public static Tooltip Heal { get; } = new Tooltip("Heal", Icons.Heal, "Ability", textParameters =>
		"Heal X is a positive targeted ability that allows the target to heal X amount of damage, increasing their current hit point value. A figure’s current hit point value can never exceed their maximum hit point value, though it is permitted to heal a figure that is already at their maximum hit point value.");

	public static Tooltip Shield { get; } = new Tooltip("Shield", Icons.Shield, "Ability", textParameters =>
		"Shield X is a positive ability that gives the acting figure a shield bonus of X. This reduces the attack value of an incoming attack by X. The shield bonus is only applied after all attack bonuses or penalties and modifiers have been applied. Multiple shield bonuses stack with one another as a single reduction in attack value. A shield bonus does not apply to sources of damage that are not attacks.");

	public static Tooltip Retaliate { get; } = new Tooltip("Retaliate", Icons.Retaliate, "Ability", textParameters =>
		"Retaliate X is a positive ability that gives the acting figure a retaliate bonus of X. This deals X damage to any figure who attacks them, if they are within the specified range after all attack effects are resolved (including push or pull). If no range is specified, the bonus is only applied to adjacent attackers. This bonus is triggered by each attack and occurs after the attack is resolved. A retaliate bonus is not an attack or targeted effect, therefore the damage cannot be reduced by a shield bonus. If a retaliating figure is killed or exhausted by an attack, the retaliate bonus does not occur. Multiple retaliate bonuses stack with one another as a single source of damage, but each retaliate bonus is only applied within its specified range.");

	public static Tooltip Summon { get; } = new Tooltip("Summon", null, "Ability", textParameters =>
		"Summon is a positive ability that adds other allied figures to the map. These figures, known as “summons,” must be placed in different empty hexes adjacent to the summoner. If there is no adjacent hex available, the figure is not summoned. A character summon is considered a persistent bonus, and their card is placed in the summoner’s active area. When the summon’s hit point value is reduced to zero, the summon is removed from the map. The card is then placed in the summoner’s discard pile or lost pile, depending on whether the action has a lost icon.");

	public static Tooltip SufferDamage { get; } = new Tooltip("Suffer Damage", null, "Ability", textParameters =>
		"Some abilities cause figures to suffer damage without an attack being performed. This damage is not modified by anything except ward. Suffer damage is not a targeted ability.");

	public static Tooltip Loot { get; } = new Tooltip("Suffer Damage", Icons.Loot, "Ability", textParameters =>
		"Loot X is an ability that allows a figure to loot all loot tokens and treasure tiles within range X, including any in their current hex. This ability is unaffected by the presence of figures or overlay tiles. If there are no loot tokens or treasure tiles within the specified range, the ability cannot be performed.");

	public static Tooltip Grant { get; } = new Tooltip("Grant", null, "Ability", textParameters =>
		"Allows a figure to grant an ability to any figure. The commanded figure then performs that ability. If the commanded figure is a character summon or scenario ally, the commanding figure decides how the ability is performed.");

	public static Tooltip Control { get; } = new Tooltip("Control", null, "Ability", textParameters =>
		"Allows a figure to control an ability of an enemy. The commanded figure then performs that ability, and the commanding figure decides how the ability is performed. During this ability, the commanded figure treats the commanding figure’s allies and enemies as their own allies and enemies. When a move ability is controlled, it is considered forced movement.");

	// public static Tooltip Attack { get; } = new Tooltip("Attack", Icons.Attack, null, textParameters =>
	// 	"");

	// public static Tooltip Attack { get; } = new Tooltip("Attack", Icons.Attack, null, textParameters =>
	// 	"");

	// public static Tooltip Attack { get; } = new Tooltip("Attack", Icons.Attack, null, textParameters =>
	// 	"");

	public static Tooltip Pierce { get; } = new Tooltip("Pierce", Icons.Pierce, null, textParameters =>
		$"Pierce X is an added effect that reduces the target’s shield bonus by X. Multiple {Icons.Inline(Icons.Pierce, textParameters, true)} effects can be combined. For example, if an attack with {Icons.Inline(Icons.Pierce, textParameters, true)}2 gains {Icons.Inline(Icons.Pierce, textParameters, true)}3, the effect would reduce the target’s shield bonus by 5 for that attack.");

	public static Tooltip Push { get; } = new Tooltip("Push", Icons.Push, "Forced Movement", textParameters =>
		$"Push X forces the target to move up to X hexes away from the acting figure. Each individual hex entered must place the target farther by range from the acting figure.");

	public static Tooltip Pull { get; } = new Tooltip("Pull", Icons.Pull, "Forced Movement", textParameters =>
		$"Pull X forces the target to move up to X hexes toward the acting figure. Each individual hex entered must place the target closer by range to the acting figure.");

	public static Tooltip Swing { get; } = new Tooltip("Swing", Icons.Swing, "Forced Movement", textParameters =>
		$"Swing X forces the target to move up to X hexes in one direction (clockwise/counterclockwise) spatially at the same distance from the acting figure.");
}