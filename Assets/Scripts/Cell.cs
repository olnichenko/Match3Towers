using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CellType
{
	Hole = -1,
	Blank = 0,
	Apple = 1,
	Lemon = 2,
	Bread = 3,
	Broccoli = 4,
	Coconut = 5
}

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	
	[SerializeField] Image _image;
	public RectTransform Rect;
	private CellData _cellData;
	private CellMover _cellMover;
	[SerializeField]private float _moveSpeed = 10f;
	private Vector2 _position;
	private bool _isUpdating;

	public Point Point => _cellData.Point;
	public CellType CellType => _cellData.CellType;

	public void Initialize(CellData cellData, Sprite sprite, CellMover cellMover)
	{
		_cellData = cellData;
		_image.sprite = sprite;
		_cellMover = cellMover;
	}

	public bool UpdateCell()
	{
		if (Vector3.Distance(Rect.anchoredPosition, _position) > 1)
		{
			MoveToPosition(_position);
			_isUpdating = true;
		}
		else
		{
			Rect.anchoredPosition = _position;
			_isUpdating = false;
		}
		return _isUpdating;
	}

	private void UpdateName() => transform.name = $"Cell [{Point.x}, {Point.y}]";
	public void OnPointerDown(PointerEventData eventData)
	{
		_cellMover.MoveCell(this);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_cellMover.DropCell();
	}

	public void MoveToPosition(Vector2 position)
	{
		Rect.anchoredPosition = Vector2.Lerp(Rect.anchoredPosition, position, Time.deltaTime * _moveSpeed);
	}

	internal void ResetPosition()
	{
		_position = BoardService.GetBoardPositionFromPoint(Point);
	}

	public void SetCellPoint(Point point)
	{
		_cellData.Point = point;
		UpdateName();
		ResetPosition();
	}
}