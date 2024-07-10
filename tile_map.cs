using Godot;
using System;
using System.Numerics;

public partial class tile_map : TileMap
{
	public int currentPlayer = 1;
	private int[][] board;
	public int winCondition = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Rect2 usedRect = this.GetUsedRect();

		int width = (int)(usedRect.End.X - usedRect.Position.X);
		int height = (int)(usedRect.End.Y - usedRect.Position.Y);

		board = new int[width][];
		for (int i = 0; i < width; i++)
		{
			board[i] = new int[height];
		}

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				board[x][y] = 0;
				//GD.Print(x, y, board[x][y]);
			}
		}

	}
	public void OnTileClicked(Vector2I clickedTile)
	{
		int column = (int)clickedTile.X;
		int row = GetLowestEmptyRow(column);

		if (row != -1)
		{
			PlaceToken(column, row, currentPlayer);
			switch (CheckForWin(column, row))
			{
				case 1:
					WinCondition();
					break;
				case 0:
					DrawCondition();
					break;
				default:
					break;
			}

			ChangePlayer();
		}
	}
	private int GetLowestEmptyRow(int column)
	{
		int boardHeight = this.GetUsedRect().Size.Y;
		try
		{
			for (int y = boardHeight - 1; y >= 0; y--)
			{
				if (board[column][y] == 0)
				{
					return y;
				}
			}
			return -1;
		}
		catch (IndexOutOfRangeException)
		{
			//GD.Print("IndexOutOfRangeException");
			return -1;
		}
	}

	private void PlaceToken(int column, int row, int player)
	{
		int gameboardStartX = this.GetUsedRect().Position.X;
		int gameboardStartY = this.GetUsedRect().Position.Y;
		//GD.Print(gameboardStartX, gameboardStartY);
		//GD.Print(column, row);

		board[column][row] = player;
		SetCell(1, new Vector2I(gameboardStartX + column, gameboardStartY + row), player, new Vector2I(0, 0));
	}
	private void ChangePlayer()
	{
		currentPlayer = (currentPlayer == 1) ? 2 : 1;
	}

	private void WinCondition()
	{
		winCondition = 1;
		if (currentPlayer == 1)
		{
			GetNode<CanvasItem>("../TileMap/RedWins").Visible = true;
		}
		else
		{
			GetNode<CanvasItem>("../TileMap/YellowWins").Visible = true;
		}
	}

	private void DrawCondition()
	{
		winCondition = 1;
		GetNode<CanvasItem>("../TileMap/Draw").Visible = true;
	}

	private bool HasEmptyCell()
	{
		foreach (int[] row in board)
		{
			foreach (int cell in row)
			{
				if (cell == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void Reset()
	{
		this.ClearLayer(1);
		for (int i = 0; i < board.Length; i++)
		{
			for (int j = 0; j < board[i].Length; j++)
			{
				board[i][j] = 0;
			}
		}
		winCondition = 0;
		GetNode<CanvasItem>("../TileMap/RedWins").Visible = false;
		GetNode<CanvasItem>("../TileMap/YellowWins").Visible = false;
		GetNode<CanvasItem>("../TileMap/Draw").Visible = false;

	}

	private int CheckForWin(int column, int row)
	{
		int player = board[column][row];

		// Check horizontal
		int count = 0;
		for (int i = 0; i < board.Length; i++)
		{
			if (board[i][row] == player)
			{
				count++;
				if (count >= 4)
				{
					return 1;
				}
			}
			else
			{
				count = 0;
			}
		}

		// Check vertical
		count = 0;
		for (int i = 0; i < board[0].Length; i++)
		{
			if (board[column][i] == player)
			{
				count++;
				if (count >= 4)
				{
					return 1;
				}
			}
			else
			{
				count = 0;
			}
		}

		// Check diagonal (top-left to bottom-right)
		count = 0;
		int r = row, c = column;
		while (r >= 0 && c >= 0)
		{
			r--;
			c--;
		}
		r++;
		c++;
		while (r < board[0].Length && c < board.Length)
		{
			if (board[c][r] == player)
			{
				count++;
				if (count >= 4)
				{
					return 1;
				}
			}
			else
			{
				count = 0;
			}
			r++;
			c++;
		}

		// Check diagonal (bottom-left to top-right)
		count = 0;
		r = row;
		c = column;
		while (r >= 0 && c < board.Length)
		{
			r--;
			c++;
		}
		r++;
		c--;
		while (r < board[0].Length && c >= 0)
		{
			if (board[c][r] == player)
			{
				count++;
				if (count >= 4)
				{
					return 1;
				}
			}
			else
			{
				count = 0;
			}
			r++;
			c--;
		}
		if (HasEmptyCell())
		{
			return -1;
		}

		return 0;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Keycode == Key.R && keyEvent.Pressed)
		{
			Reset();
		}

		Rect2 usedRect = this.GetUsedRect();

		int width = (int)(usedRect.End.X - usedRect.Position.X);
		int startX = (int)usedRect.Position.X;
		int startY = (int)usedRect.Position.Y;

		if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false && winCondition == 0)
			{
				Vector2I posOfBoard = this.LocalToMap(eventMouseButton.Position) - this.GetUsedRect().Position;
				//GD.Print("posofboard", posOfBoard);
				OnTileClicked(posOfBoard);
			}
		}
	}
}
