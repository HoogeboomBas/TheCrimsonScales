public static class Tooltips
{
	public static Tooltip XP { get; } = new Tooltip("Experience", Icons.XP, null, textParameters =>
		"Experience measures a character’s growth and defines when they level up. When an action depicts this icon in the lower right corner, if any part of that action is performed, the character gains the indicated amount of experience. Sometimes an ability specifies that experience is only gained under certain conditions, such as consuming an element, meeting a requirement, or advancing a character token between use slots.");

	public static Tooltip Pierce { get; } = new Tooltip("Pierce", Icons.Pierce, null, textParameters =>
		$"Pierce is an added effect that reduces the target’s shield bonus by X. Multiple {Icons.Inline(Icons.Pierce, textParameters, true)} effects can be combined. For example, if an attack with {Icons.Inline(Icons.Pierce, textParameters, true)}2 gains {Icons.Inline(Icons.Pierce, textParameters, true)}3, the effect would reduce the target’s shield bonus by 5 for that attack.");
}