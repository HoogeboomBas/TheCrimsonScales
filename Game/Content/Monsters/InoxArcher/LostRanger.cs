using System.Collections.Generic;

public class LostRanger : InoxArcher
{
	private readonly MonsterStats _stats =
		new MonsterStats()
		{
			Health = 10,
			Attack = 3,
			Range = 5
		};

	public override MonsterStats[] NamedLevelStats => [_stats, _stats, _stats, _stats, _stats, _stats, _stats, _stats];

	public override string Name => "Lost Ranger";
	public override MonsterModel ParentMonsterModel => ModelDB.Monster<InoxArcher>();

	//TODO: Look at card back
	public override IEnumerable<MonsterAbilityCardModel> Deck => [ModelDB.MonsterAbilityCard<LostRangerAbilityCard>()];

	private class LostRangerAbilityCard : MonsterAbilityCardModel
	{
		public override int Initiative => 30;
		public override int CardIndex => 0;
		public override string CardsAtlasPath => "res://Content/Monsters/InoxArcher/LostRangerCard.jpg";
		public override int ColumnCount => 1;
		public override int RowCount => 1;

		public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
		[
			new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
		];
	}
}