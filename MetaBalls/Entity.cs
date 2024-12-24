using System.Numerics;

namespace MetaBalls;

public class Circle
{
	public float PositionX { get; set; }
	public float PositionY { get; set; }
	public float Radius { get; set; }
	public float DirectionX { get; set; }
	public float DirectionY { get; set; }

	public Circle(Vector2 pos, float radius)
	{
		PositionX = pos.X;
		PositionY = pos.Y;
		Radius = radius;
	}
}