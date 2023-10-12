using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;


public class MatchMachine
{
	private readonly BoardService _boardService;

	private readonly Point[] _directions =
	{
		Point.Up, Point.Right, Point.Down, Point.Left
	};

	public MatchMachine(BoardService boardService)
	{
		_boardService = boardService;
	}

	public List<Point> GetMatchedPoints(Point point, bool main)
	{
		//Debug.Log($"Start - GetMatchedPoints p main {main}");
		var connectedPoints = new List<Point>();
		var cellTypeAtPoint = _boardService.GetCellTypeAtPoint(point);

		CheckForDirectionMatch(ref connectedPoints, point, cellTypeAtPoint);

		CheckForMiddleOfMatch(ref connectedPoints, point, cellTypeAtPoint);

		CheckForSquareMatch(ref connectedPoints, point, cellTypeAtPoint);

		if (main)
		{
			for (var i = 0; i < connectedPoints.Count; i++)
			{
				AddPoints(ref connectedPoints, GetMatchedPoints(connectedPoints[i], false));
			}
		}

		//Debug.Log("End - GetMatchedPoints");
		return connectedPoints;
	}

	private void CheckForSquareMatch(ref List<Point> connectedPoints, Point point, CellType cellTypeAtPoint)
	{
		for (var i = 0; i < 4; i++)
		{
			var square = new List<Point>();
			var nextCellIndex = i + 1;
			nextCellIndex = nextCellIndex > 3 ? 0 : nextCellIndex;

			Point[] checkPoints =
			{
				Point.Add(point, _directions[i]),
				Point.Add(point, _directions[nextCellIndex]),
				Point.Add(point, Point.Add(_directions[i], _directions[nextCellIndex])),
			};
			foreach (var checkPoint in checkPoints)
			{
				if (_boardService.GetCellTypeAtPoint(checkPoint) == cellTypeAtPoint)
				{
					square.Add(checkPoint);
				}
			}

			if (square.Count > 2)
			{
				AddPoints(ref connectedPoints, square);
			}
		}
	}

	private void CheckForMiddleOfMatch(ref List<Point> connectedPoints, Point point, CellType cellTypeAtPoint)
	{
		//Debug.Log("Start method CheckForMiddleOfMatch");
		for (var i = 0; i < 2; i++)
		{
			var line = new List<Point>();

			Point[] checkPoints =
			{
				Point.Add(point, _directions[i]),
				Point.Add(point, _directions[i + 2])
			};

			foreach (var checkPoint in checkPoints)
			{
				if (_boardService.GetCellTypeAtPoint(checkPoint) == cellTypeAtPoint)
				{
					line.Add(checkPoint);
				}
			}

			if (line.Count > 1)
			{
				AddPoints(ref connectedPoints, line);
			}
		}
		//Debug.Log("End method CheckForMiddleOfMatch");
	}

	private void CheckForDirectionMatch(ref List<Point> connectedPoints, Point point, CellType cellTypeAtPoint)
	{
		//Debug.Log("Start method CheckForDirectionMatch");
		foreach (var direction in _directions)
		{
			var line = new List<Point>();

			for (var i = 1; i < 3; i++)
			{
				var checkPoint = Point.Add(point, Point.Multiply(direction, i));
				if (_boardService.GetCellTypeAtPoint(checkPoint) == cellTypeAtPoint)
				{
					line.Add(checkPoint);
				}
			}

			if (line.Count > 1)
			{
				AddPoints(ref connectedPoints, line);
			}
		}
		//Debug.Log("Start method CheckForDirectionMatch");
	}

	public static void AddPoints(ref List<Point> points, List<Point> addPoints)
	{
		foreach (var addPoint in addPoints)
		{
			var doAdd = true;
			foreach (var point in points)
			{
				if (point.Equals(addPoint))
				{
					doAdd = false;
					break;
				}
			}

			if (doAdd)
			{
				points.Add(addPoint);
			}
		}
	}
}

