using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StaticData;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(CellFactory))]
public class BoardService : MonoBehaviour
{
	public ArrayLayout BoardLayout;

	[SerializeField] private Sprite[] _cellSprites;
	[SerializeField] private ParticleSystem _matchFxPrefab;
	[SerializeField] private ScoreService _scoreService;

	private CellData[,] _board;
	private CellFactory _cellFactory;
	private MatchMachine _matchMachine;
	private CellMover _cellMover;

	private readonly int[] _flippingCellsCountByColumn = new int[Config.BOARD_WIDTH ];
	private readonly List<Cell> _updatingCells = new List<Cell>();
	private readonly List<Cell> _deadCells = new List<Cell>();
	private readonly List<CellFlip> _flippedCells = new List<CellFlip>();
	private readonly List<ParticleSystem> _matchFxs = new List<ParticleSystem>();

	public Sprite[] CellSprites => _cellSprites;

	private void Start()
	{
		//Debug.Log("Start - MatchMachine.Start");
		// TODO: Вместо BoolArrayLayout
		BoardLayout = new ArrayLayout();

		InitializeBoard();
		VerifyBoardOnMatches();
		_cellFactory.InstantiateBoard(this, _cellMover);
		//Debug.Log("End - MatchMachine.Start");
	}

	private void Update()
	{
		_cellMover.Update();

		var finishUpdating = new List<Cell>();
		foreach (var cell in _updatingCells)
		{
			if (!cell.UpdateCell())
			{
				finishUpdating.Add(cell);
			}
			
		}

		foreach (var cell in finishUpdating)
		{
			var x = cell.Point.x;

			_flippingCellsCountByColumn[x] = Mathf.Clamp(_flippingCellsCountByColumn[x] - 1, 0, Config.BOARD_WIDTH);

			var flip = GetFlip(cell);
			var connectedPoints = _matchMachine.GetMatchedPoints(cell.Point, true);

			Cell flippedCell = null;
			if (flip != null)
			{
				flippedCell = flip.GetOtherCell(cell);
				MatchMachine.AddPoints(
					ref connectedPoints,
					_matchMachine.GetMatchedPoints(flippedCell.Point, true)
				);
			}

			if (connectedPoints.Count == 0)
			{
				if (flippedCell != null)
				{
					FlipCells(cell.Point, flippedCell.Point, false);
				}
			}
			else
			{
				ParticleSystem matchFx;
				if (_matchFxs.Count > 0 && _matchFxs[0].isStopped)
				{
					matchFx = _matchFxs[0];
					_matchFxs.RemoveAt(0);
				}
				else
				{
					matchFx = Instantiate(_matchFxPrefab, transform);
				}

				_matchFxs.Add(matchFx);
				matchFx.Play();
				matchFx.transform.position = cell.transform.position;

				foreach (var connectedPoint in connectedPoints)
				{
					_cellFactory.KillCell(connectedPoint);
					var cellAtPoint = GetCellAtPoint(connectedPoint);
					var connectedCell = cellAtPoint.GetCell();
					if (connectedCell != null)
					{
						connectedCell.gameObject.SetActive(false);
						_deadCells.Add(connectedCell);
					}
					cellAtPoint.SetCell(null);
				}

				_scoreService.AddScore(connectedPoints.Count);

				ApplyGravityToBoard();
			}

			_flippedCells.Remove(flip);
			_updatingCells.Remove(cell);
		}
	}

	private void ApplyGravityToBoard()
	{
		for (var x = 0; x < Config.BOARD_WIDTH; x++)
		{
			for (var y = Config.BOARD_HEIGHT - 1; y >= 0; y--)
			{
				var point = new Point(x, y);
				var cellData = GetCellAtPoint(point);
				var cellTypeAtPoint = GetCellTypeAtPoint(point);
				if (cellTypeAtPoint != 0)
				{
					continue;
				}

				for (var newY = y - 1; newY >= -1 ; newY--)
				{
					var nextPoint = new Point(x, newY);
					var nextCellType = GetCellTypeAtPoint(nextPoint);
					if (nextCellType == 0)
					{
						continue;
					}

					if (nextCellType != CellType.Hole)
					{
						var cellAtPoint = GetCellAtPoint(nextPoint);
						var cell = cellAtPoint.GetCell();
						cellData.SetCell(cell);
						_updatingCells.Add(cell);
						cellAtPoint.SetCell(null);
					}
					else
					{
						var cellType = GetRandomCellType();
						var fallPoint = new Point(x, -1 - _flippingCellsCountByColumn[x]);

						Cell cell;
						if (_deadCells.Count > 0)
						{
							var revivedCell = _deadCells[0];
							revivedCell.gameObject.SetActive(true);
							cell = revivedCell;
							_deadCells.RemoveAt(0);
						}
						else
						{
							cell = _cellFactory.InstantiateCell();
						}

						cell.Initialize(new CellData(cellType, point), _cellSprites[(int)(cellType - 1)], _cellMover);
						cell.Rect.anchoredPosition = GetBoardPositionFromPoint(fallPoint);

						var holeCell = GetCellAtPoint(point);
						holeCell.SetCell(cell);
						ResetCell(cell);
						_flippingCellsCountByColumn[x]++;
					}

					break;
				}
			}
		}
	}

	private CellFlip GetFlip(Cell cell)
	{
		foreach (var flip in _flippedCells)
		{
			if (flip.GetOtherCell(cell) != null)
			{
				return flip;
			}
		}
		return null;
	}

	public void FlipCells(Point firstPoint, Point secondPoint, bool main)
	{
		if (GetCellTypeAtPoint(firstPoint) < 0)
		{
			return;
		}

		var firstCellData = GetCellAtPoint(firstPoint);
		var firstCell = firstCellData.GetCell();
		if (GetCellTypeAtPoint(secondPoint) > 0)
		{
			var secondCellData = GetCellAtPoint(secondPoint);
			var secondCell = secondCellData.GetCell();
			firstCellData.SetCell(secondCell);
			secondCellData.SetCell(firstCell);

			if (main)
			{
				_flippedCells.Add(new CellFlip(firstCell, secondCell));
			}
			_updatingCells.Add(firstCell);
			_updatingCells.Add(secondCell);
		}
		else
		{
			ResetCell(firstCell);
		}
	}

	public void ResetCell(Cell cell)
	{
		cell.ResetPosition();
		_updatingCells.Add(cell);
	}

	private void Awake()
	{
		_cellFactory = GetComponent<CellFactory>();
		_matchMachine = new MatchMachine(this);
		_cellMover = new CellMover(this);
	}

	private void VerifyBoardOnMatches()
	{
		//Debug.Log("Start - VerifyBoardOnMatches");
		for (var y = 0; y < Config.BOARD_HEIGHT; y++)
		{
			for (var x = 0; x < Config.BOARD_WIDTH; x++)
			{
				var point = new Point(x, y);
				var cellTypeAtPoint = GetCellTypeAtPoint(point);
				if (cellTypeAtPoint <= 0)
				{
					continue;
				}

				var removeCellTypes = new List<CellType>();

				while (_matchMachine.GetMatchedPoints(point, true).Count > 0)
				{
					if (removeCellTypes.Contains(cellTypeAtPoint) == false)
					{
						removeCellTypes.Add(cellTypeAtPoint);
					}
					SetSellTypeAtPoint(point, GetNewCellType(ref removeCellTypes));
				}
			}
		}
		//Debug.Log("End - VerifyBoardOnMatches");
	}

	private void SetSellTypeAtPoint(Point point, CellType newCellType)
		=> _board[point.x, point.y].CellType = newCellType;

	private CellType GetNewCellType(ref List<CellType> removeCellTypes)
	{
		var availableCellTypes = new List<CellType>();
		for (var i = 0; i < CellSprites.Length; i++)
		{
			availableCellTypes.Add((CellType)i + 1);
		}

		foreach (var removeCellType in removeCellTypes)
		{
			availableCellTypes.Remove(removeCellType);
		}

		return availableCellTypes.Count <= 0
			? CellType.Blank
			: availableCellTypes[Random.Range(0, availableCellTypes.Count)];
	}

	public CellType GetCellTypeAtPoint(Point point)
	{
		if (point.x < 0 || point.x >= Config.BOARD_WIDTH || point.y < 0 || point.y >= Config.BOARD_HEIGHT)
		{
			return CellType.Hole;
		}
		return _board[point.x, point.y].CellType;
	}

	private void InitializeBoard()
	{
		_board = new CellData[Config.BOARD_WIDTH, Config.BOARD_HEIGHT];
		for (var y = 0; y < Config.BOARD_HEIGHT; y++)
		{
			for (var x = 0; x < Config.BOARD_WIDTH; x++)
			{
				_board[x, y] = new CellData(
					BoardLayout.Rows[y].Row[x] ? CellType.Hole : GetRandomCellType(),
					new Point(x, y)
				);
			}
		}
	}

	private CellType GetRandomCellType() => (CellType)(Random.Range(0, _cellSprites.Length) + 1);


	public CellData GetCellAtPoint(Point point) => _board[point.x, point.y];

	public static Vector2 GetBoardPositionFromPoint(Point point) =>
		new(
			Config.PIECE_SIZE / 2 + Config.PIECE_SIZE * point.x,
			-Config.PIECE_SIZE / 2 - Config.PIECE_SIZE * point.y
		);
}
