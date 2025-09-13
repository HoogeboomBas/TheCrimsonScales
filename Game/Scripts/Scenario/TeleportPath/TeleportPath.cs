using System.Collections.Generic;
using System.Threading;
using Fractural.Tasks;
using Godot;

public partial class TeleportPath : Node2D
{
	[Export]
	private PackedScene _waypointScene;
	[Export]
	private Line2D _line2D;

	private readonly List<Waypoint> _waypoints = new List<Waypoint>();

	private CancellationTokenSource _updatePointsCancellationToken;

	public override void _Ready()
	{
		base._Ready();

		Hide();

		_line2D.Points = [];
	}

	public void Open(Hex origin, Hex target)
	{
		Show();

		foreach(Waypoint waypoint in _waypoints)
		{
			waypoint.Destroy();
		}

		_waypoints.Clear();

		_updatePointsCancellationToken?.Cancel();
		_updatePointsCancellationToken = new CancellationTokenSource();
		UpdatePointsTask(origin, target, _updatePointsCancellationToken.Token).Forget();
	}

	public void Close()
	{
		Hide();
		_line2D.Points = [];

		foreach(Waypoint waypoint in _waypoints)
		{
			waypoint.Destroy();
		}

		_waypoints.Clear();
	}

	private async GDTaskVoid UpdatePointsTask(Hex origin, Hex target, CancellationToken cancellationToken)
	{
		List<Vector2> linePoints = [origin.GlobalPosition];

		if(target != null && origin != target)
		{
			linePoints.Add(target.GlobalPosition);
		}

		_line2D.Points = linePoints.ToArray();

		AddWaypoint(origin);

		if(target != null)
		{
			AddWaypoint(target);
		}

		await GDTask.CompletedTask;

		//_line2D.Points

		// if(difference > 0 && _currentTargetPoints.Length > 1)
		// {
		// 	for(int i = 0; i < difference; i++)
		// 	{
		// 		int indexToHandle = prevTargetPoints.Length + i;
		// 		Vector2 startPoint = _currentTargetPoints[indexToHandle - 1];
		// 		Vector2 endPoint = _currentTargetPoints[indexToHandle];
		// 		_line2D.AddPoint(startPoint);
		//
		// 		await CustomGTweenExtensions.Tween(t =>
		// 		{
		// 			Vector2 position = startPoint.Lerp(endPoint, t);
		// 			_line2D.SetPointPosition(indexToHandle, position);
		// 		}, 0.03f).SetEasing(Easing.Linear).PlayAsync(cancellationToken);
		// 	}
		//
		// 	foreach(Hex waypointHex in waypointHexes)
		// 	{
		// 		AddWaypoint(waypointHex);
		// 	}
		// }
		// else
		// {
		// 	_line2D.Points = _currentTargetPoints;
		// }
	}

	private void AddWaypoint(Hex waypointHex)
	{
		Waypoint newWaypoint = _waypointScene.Instantiate<Waypoint>();
		AddChild(newWaypoint);
		newWaypoint.GlobalPosition = waypointHex.GlobalPosition;
		newWaypoint.Init(waypointHex);
		_waypoints.Add(newWaypoint);
	}
}