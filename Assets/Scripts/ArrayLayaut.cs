using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ArrayLayout
{
	[Serializable]
	public struct RowData
	{
		public bool[] Row;
	}
	public RowData[] Rows = new RowData[14];

	public ArrayLayout()
	{
		for (var i=0; i < Rows.Length;i++)
		{
			Rows[i].Row = new bool[9];
		}
		Rows[0].Row[2] = true;
		Rows[0].Row[3] = true;
		Rows[0].Row[4] = true;
		Rows[1].Row[2]= true;
		Rows[1].Row[3]= true;
	}
}

