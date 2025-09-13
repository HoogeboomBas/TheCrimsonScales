using System.Collections.Generic;

public class ForcedMovementNode
{
	public Hex Hex { get; }
	public int MoveSpent { get; }
	public int MoveLeft { get; }
	public List<ForcedMovementNode> Parents { get; } = new List<ForcedMovementNode>();

	public ForcedMovementNode(Hex hex, int moveSpent, int moveLeft)
	{
		Hex = hex;
		MoveSpent = moveSpent;
		MoveLeft = moveLeft;
	}

	public CompareResult CompareTo(ForcedMovementNode other)
	{
		if(other.MoveSpent > MoveSpent)
		{
			return CompareResult.Better;
		}

		if(MoveSpent > other.MoveSpent)
		{
			return CompareResult.Worse;
		}

		return CompareResult.Equal;
	}
}