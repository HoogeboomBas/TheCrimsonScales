public interface IHasEnfeeble
{
	int RemainingEnfeebleCount { get; set; }
    AMDCardModel CreateEnfeeble();
}