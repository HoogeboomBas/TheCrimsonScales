using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class AMDCardModel : AbstractModel
{
	protected abstract string GetTexturePath(AMDCardOwner owner);
	protected abstract int AtlasIndex { get; }
	protected abstract int ColumnCount { get; }
	protected abstract int RowCount { get; }

	public virtual bool Reshuffles => false;
	public virtual bool RemoveAfterDraw => false;

	public virtual AMDCardType Type => AMDCardType.Value;

	public virtual bool GetRolling(AttackAbility.State attackAbilityState) => false;

	public virtual int? GetValue(AttackAbility.State attackAbilityState) => null;

	public virtual int? Pierce => null;
	public virtual int? Push => null;
	public virtual int? Pull => null;
	public virtual int? Swing => null;
	public virtual int? AddedTargets => null;

	public virtual List<CardElementInfusion> ElementInfusions => [];
	public virtual List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [];
	public virtual List<Ability> GetAbilities(AttackAbility.State attackAbilityState) => [];

	public virtual Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) => null;

	public Texture2D GetTexture(AMDCardOwner owner)
	{
		return AtlasTextureHelper.CreateAtlasTexture(
			AtlasIndex, ColumnCount, RowCount,
			ResourceLoader.Load<Texture2D>(GetTexturePath(owner)));
	}
}