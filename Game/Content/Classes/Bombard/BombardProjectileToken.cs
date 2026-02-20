using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweens.Enums;
using GTweensGodot.Extensions;

public partial class BombardProjectileToken : HexObject
{
	public AbilityCardSide AbilityCardSide { get; private set; }

	public void SetCardSide(AbilityCardSide abilityCardSide)
	{
		AbilityCardSide = abilityCardSide;
	}

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		Scale = Vector2.Zero;
		this.TweenRotationDegrees(360f, 0.3f, RotationMode.TotalDistance).PlayFastForwardable();
		this.TweenScale(1f, 0.5f).SetEasing(Easing.OutBack).PlayFastForwardable();

		AppController.Instance.AudioController.PlayFastForwardable("res://Content/Classes/Bombard/ProjectileAim.wav", minPitch: 1f, maxPitch: 1f,
			delay: 0.0f);
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new BombardProjectileInfoItem.Parameters(this));
	}
}