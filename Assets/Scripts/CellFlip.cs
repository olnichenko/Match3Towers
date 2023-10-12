using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CellFlip
{
	private readonly Cell _firstCell;
	private readonly Cell _secondCell;

	public CellFlip (Cell firstCell, Cell secondCell)
	{
		_firstCell = firstCell;
		_secondCell = secondCell;
	}

	public Cell GetOtherCell(Cell cell)
	{
		if (cell == _firstCell)
		{
			return _secondCell;
		}
		if (cell == _secondCell)
		{
			return _firstCell;
		}
		return null;
	}
}

