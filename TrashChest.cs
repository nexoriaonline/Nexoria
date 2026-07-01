using Godot;
using System;

public partial class TrashChest : AnimatedSprite2D
{
	public override void _Ready()
	{
		Stop();
		SetOpen(false);
	}

	public void SetOpen(bool isOpen)
	{
		Stop();
		Frame = isOpen ? 1 : 0;
	}

	public bool ContainsGlobalPoint(Vector2 globalPosition)
	{
		var area = GetNodeOrNull<Area2D>("Area2D");
		var collisionShape = area?.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collisionShape?.Shape is not RectangleShape2D rectangleShape) return false;

		var localPosition = collisionShape.ToLocal(globalPosition);
		var rect = new Rect2(-rectangleShape.Size / 2.0f, rectangleShape.Size);
		return rect.HasPoint(localPosition);
	}
}
