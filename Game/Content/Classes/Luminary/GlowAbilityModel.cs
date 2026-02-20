using System;
using System.Collections.Generic;

public class GlowAbilityModel(List<Element> elements, Func<List<Element>, Ability> ability, string hintText, string hintIcon)
{
	public List<Element> Elements { get; } = elements;
	public Func<List<Element>, Ability> Ability { get; } = ability;
	public string HintText { get; } = hintText;
	public string HintIcon { get; } = hintIcon;
}