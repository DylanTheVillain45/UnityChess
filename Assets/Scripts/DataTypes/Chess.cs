using System.Collections.Generic;

public class Chess {
    public Piece[,] board;
    public List<Move> MovesList;
    public PieceColor GameColor;
    public List<(Move, Move)> PastMoves = new List<(Move, Move)>();
    public bool isAi = false;
    public bool isLateGame = false;

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

    public Chess DeepCopy() {
        Chess copy = new Chess();

        copy.board = new Piece[8, 8];

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                Piece originalPiece = this.board[y, x];
                if (originalPiece != null) {
                    copy.board[y, x] = new Piece(originalPiece);
                }
            }
        }

        copy.PastMoves = new List<(Move, Move)>(this.PastMoves);

        copy.GameColor = this.GameColor;
        copy.isAi = this.isAi;

        copy.MovesList = null;

        return copy;
    }
}
