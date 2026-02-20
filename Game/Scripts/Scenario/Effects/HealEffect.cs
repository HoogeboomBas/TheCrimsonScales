using Godot;

public partial class HealEffect : Node2D
{
	[Export]
	private CpuParticles2D _particles;

	public void Init()
	{
		_particles.SetEmitting(true);

		this.DelayedCall(QueueFree, 3f);
	}
}