using Godot;
using System;

public partial class suggestiondropper : TileMap
{
	private TileMap parentTileMap;
	private Rect2I gameboardRect;
	private int currentPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parentTileMap = GetParent<TileMap>();
		gameboardRect = parentTileMap.GetUsedRect();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			Vector2I mousePos = this.LocalToMap(eventMouseMotion.Position);

			if (mousePos.X >= gameboardRect.Position.X
			&& mousePos.X <= gameboardRect.End.X - 1
			&& GetParent<tile_map>().winCondition == 0)
			{
				currentPlayer = GetNode<tile_map>("..").currentPlayer;
				this.Clear();
				this.SetCell(0, new Vector2I(mousePos.X, gameboardRect.Position.Y - 1), currentPlayer - 1, new Vector2I(0, 0));
			}
		}
	}
}
