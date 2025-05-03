using System.Collections.Generic;

public class Chess {
    public Piece[,] board;
    public List<Move> MovesList;
    public PieceColor GameColor;
    public List<(Move, Move)> PastMoves = new List<(Move, Move)>();
    public bool isAi = false;

    public void GetMoves() {
        MovesList = MoveGenerator.GetValidMoves(this, GameColor);
    }

    public void MakeMove(Move move) {
        MoveMaker.CommitMove(this, move);

        if (GameColor == PieceColor.White) {
            PastMoves.Add((move, null));
        } else {
            PastMoves[^1] = (PastMoves[^1].Item1, move);
        }

        GameColor = GameColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    public void UnMakeMove(Move move) {
        GameColor = GameColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        MoveMaker.UnCommitMove(this, move);

        if (PastMoves.Count == 0) return;

        if (GameColor == PieceColor.White) {
            PastMoves.RemoveAt(PastMoves.Count - 1);
        } else {
            PastMoves[^1] = (PastMoves[^1].Item1, null);
        }
    }

    public void ResetGame() {}
}
