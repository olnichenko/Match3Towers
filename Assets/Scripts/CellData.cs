using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CellData
{
	public CellType CellType;
	public Point Point;
	private Cell _cell;
	public CellData(CellType cellType, Point point)
	{
		CellType = cellType;
		Point = point;
	}

	public Cell GetCell()
	{
		return _cell;
	}

	public void SetCell(Cell newCell)
	{
		_cell = newCell;
		if (_cell == null)
		{
			CellType = CellType.Blank;
		}
		else
		{
			CellType = newCell.CellType;
			_cell.SetCellPoint(Point);
		}
	}
}

