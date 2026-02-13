public interface IHasEmpower
{
	int RemainingEmpowerCount { get; set; }
    AMDCardModel CreateEmpower();
}