using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class MovePath : Node2D
{
	[Export]
	private PackedScene _waypointScene;
	[Export]
	private Line2D _line2D;

	private readonly List<Waypoint> _waypoints = new List<Waypoint>();

	private Vector2[] _currentTargetPoints;

	private CancellationTokenSource _updatePointsCancellationToken;

	public override void _Ready()
	{
		base._Ready();

		Hide();

		_currentTargetPoints = [];
		_line2D.Points = _currentTargetPoints;
	}

	public void Open(List<Hex> path, List<Hex> waypointHexes)
	{
		Show();

		_updatePointsCancellationToken?.Cancel();
		_updatePointsCancellationToken = new CancellationTokenSource();
		UpdatePointsTask(path, waypointHexes, _updatePointsCancellationToken.Token).Forget();
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

	private async GDTaskVoid UpdatePointsTask(List<Hex> path, List<Hex> waypointHexes, CancellationToken cancellationToken)
	{
		_line2D.Points = _currentTargetPoints;

		Vector2[] prevTargetPoints = _currentTargetPoints;
		_currentTargetPoints = path.Select(hex => hex.GlobalPosition).ToArray();
		int difference = _currentTargetPoints.Length - prevTargetPoints.Length;

		TryAddWaypoint(waypointHexes[0]);

		for(int i = _waypoints.Count - 1; i >= 0; i--)
		{
			Waypoint waypoint = _waypoints[i];
			if(waypointHexes.Any(waypointHex => waypoint.Hex == waypointHex))
			{
				continue;
			}

			waypoint.Destroy();
			_waypoints.RemoveAt(i);
			await GDTask.Delay(0.1f, cancellationToken: cancellationToken);
		}

		if(difference > 0 && _currentTargetPoints.Length > 1)
		{
			for(int i = 0; i < difference; i++)
			{
				int indexToHandle = prevTargetPoints.Length + i;
				Vector2 startPoint = _currentTargetPoints[indexToHandle - 1];
				Vector2 endPoint = _currentTargetPoints[indexToHandle];
				_line2D.AddPoint(startPoint);

				await CustomGTweenExtensions.Tween(t =>
				{
					Vector2 position = startPoint.Lerp(endPoint, t);
					_line2D.SetPointPosition(indexToHandle, position);
				}, 0.03f).SetEasing(Easing.Linear).PlayAsync(cancellationToken);
			}

			foreach(Hex waypointHex in waypointHexes)
			{
				TryAddWaypoint(waypointHex);
			}
		}
		else
		{
			_line2D.Points = _currentTargetPoints;
		}
	}

	private void TryAddWaypoint(Hex waypointHex)
	{
		if(_waypoints.Any(waypoint => waypoint.Hex == waypointHex))
		{
			return;
		}

		Waypoint newWaypoint = _waypointScene.Instantiate<Waypoint>();
		AddChild(newWaypoint);
		newWaypoint.GlobalPosition = waypointHex.GlobalPosition;
		newWaypoint.Init(waypointHex);
		_waypoints.Add(newWaypoint);
	}
}