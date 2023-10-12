using Assets.Scripts.StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class CellFactory : MonoBehaviour
{
	private BoardService _boardService;
	private readonly List<KilledCell> _killedCells = new List<KilledCell>();

	[Header("BoardRects")]
	[SerializeField] private RectTransform _boardRect;
	[SerializeField] private RectTransform _killedBoardRect;
	
	[Header("Prefabs")]
	[SerializeField] private KilledCell _killedCellPrefab;
	[SerializeField] private Cell _cellPrefab;

	public Cell InstantiateCell() => Instantiate(_cellPrefab, _boardRect);

	public void InstantiateBoard(BoardService boardService, CellMover cellMover)
	{
		_boardService = boardService;
		for (var y = 0; y < Config.BOARD_HEIGHT; y++)
		{
			for (var x = 0; x < Config.BOARD_WIDTH; x++)
			{
				var point = new Point(x, y);
				var cellData = boardService.GetCellAtPoint(point);
				var cellType = cellData.CellType;
				if (cellType < 0)
				{
					continue;
				}
				var cell = InstantiateCell();
				cell.Rect.anchoredPosition = BoardService.GetBoardPositionFromPoint(point);
				cell.Initialize(
					new CellData(cellType, new Point(x, y)), 
					boardService.CellSprites[(int)cellType - 1], 
					cellMover
					);
				cellData.SetCell(cell);
			}
		}
	}

	public void KillCell(Point point)
	{
		var availableCells = new List<KilledCell>();
		foreach (var killedCell in _killedCells)
		{
			if (!killedCell.IsFalling)
			{
				availableCells.Add(killedCell);
			}	
		}
		KilledCell showedKilledCell;
		if (availableCells.Count > 0)
		{
			showedKilledCell = availableCells[0];
		}
		else
		{
			var killedCell = Instantiate(_killedCellPrefab, _killedBoardRect);
			showedKilledCell = killedCell;
			_killedCells.Add(killedCell);
		}

		var cellTypeIndex = (int)_boardService.GetCellTypeAtPoint(point) - 1;
		if (showedKilledCell != null && cellTypeIndex >= 0 && cellTypeIndex < _boardService.CellSprites.Length)
		{
			showedKilledCell.Initialize(
				_boardService.CellSprites[cellTypeIndex], 
				BoardService.GetBoardPositionFromPoint(point)
				);
		}

	}
}