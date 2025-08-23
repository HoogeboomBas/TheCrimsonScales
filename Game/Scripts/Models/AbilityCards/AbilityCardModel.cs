using System.Collections.Generic;
using Godot;

public abstract class AtlasAbilityCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : AbilityCardSide, new()
	where TBottom : AbilityCardSide, new()
{
	protected abstract string TexturePath { get; }
	protected abstract int AtlasIndex { get; }
	protected abstract int ColumnCount { get; }
	protected abstract int RowCount { get; }

	public override Texture2D GetTexture()
	{
		return AtlasTextureHelper.CreateAtlasTexture(
			AtlasIndex, ColumnCount, RowCount,
			ResourceLoader.Load<Texture2D>(TexturePath));
	}
}

public abstract class AbilityCardModel<TTop, TBottom> : AbilityCardModel
	where TTop : AbilityCardSide, new()
	where TBottom : AbilityCardSide, new()
{
	public override AbilityCardSide CreateTopSide(AbilityCard abilityCard)
	{
		return new TTop
		{
			AbilityCard = abilityCard
		};
	}

	public override AbilityCardSide CreateBottomSide(AbilityCard abilityCard)
	{
		return new TBottom
		{
			AbilityCard = abilityCard
		};
	}
}

public abstract class AbilityCardModel : AbstractModel<AbilityCardModel>
{
	public abstract string Name { get; }
	public abstract int Level { get; }
	public abstract int Initiative { get; }

	public abstract AbilityCardSide CreateTopSide(AbilityCard abilityCard);
	public abstract AbilityCardSide CreateBottomSide(AbilityCard abilityCard);

	public virtual AbilityCardSide CreateBasicTopSide(AbilityCard abilityCard)
	{
		return new BasicTopSide
		{
			AbilityCard = abilityCard
		};
	}

	public virtual AbilityCardSide CreateBasicBottomSide(AbilityCard abilityCard)
	{
		return new BasicBottomSide
		{
			AbilityCard = abilityCard
		};
	}

	public abstract Texture2D GetTexture();

	private class BasicTopSide : AbilityCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(2).Build())
		];
	}

	private class BasicBottomSide : AbilityCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build())
		];
	}
}