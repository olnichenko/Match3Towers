using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Point
{
	public int x;
	public int y;
	public Point(int newX, int newY)
	{
		x = newX;
		y = newY;
	}
	public void Multiply(int value)
	{
		x *= value;
		y *= value;
	}
	public void Add(Point point)
	{
		x += point.x;
		y += point.y;
	}
	public bool Equals(Point point)
	{
		return x == point.x && y == point.y;
	}
	public Vector2 ToVector()
	{
		return new Vector2(x, y);
	}
	public static Point FromVector(Vector2 vector)
	{
		return new Point((int)vector.x, (int)vector.y);
	}
	public static Point FromVector(Vector3 vector)
	{
		return new Point((int)vector.x, (int)vector.y);
	}
	public static Point Multiply(Point point, int value)
	{
		return new Point(point.x * value, point.y * value);

	}
	public static Point Add(Point point1, Point point2)
	{
		return new Point(point1.x + point2.x, point1.y + point2.y);
	}
	public static Point Clone(Point point)
	{
		return new Point(point.x, point.y);
	}
	public static Point Zero => new(0, 0);
	public static Point Up => new(0, 1);
	public static Point Down => new(0, -1);
	public static Point Left => new(-1, 0);
	public static Point Right => new(1, 0);
}

