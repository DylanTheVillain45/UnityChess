using System.Collections.Generic;

public static class MoveHandler {
    public static void CheckFilters(Chess chess, List<Move> moves) {
        List<Move> ValidMoves = new List<Move>();

        PieceColor myColor = moves[0].piece.color;
        PieceColor theirColor = myColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

        foreach (Move move in moves) {
            if (move.capturedPiece != null && move.capturedPiece.type == Type.King) continue;

            chess.MakeMove(move);

            bool checkFilter = Check.IsCheck(chess, myColor);

            if (checkFilter) {
                chess.UnMakeMove(move);
                continue;
            }

            ValidMoves.Add(move);

            bool moveCheck = Check.IsCheck(chess, theirColor);

            if (moveCheck) {
                move.isCheck = true;

                bool isCheckMate = true;

                List<Move> escapeMoves = MoveGenerator.GetAllMoves(chess, theirColor);


                foreach (Move escapeMove in escapeMoves) {

                    chess.MakeMove(escapeMove);

                    // maybe sort list to put king moves first?

                    bool isStillCheck = Check.IsCheck(chess, theirColor);

                    if (isStillCheck == false) {
                        isCheckMate = false;
                        chess.UnMakeMove(escapeMove);
                        break;
                    }
                    chess.UnMakeMove(escapeMove);
                }

                move.isCheckMate = isCheckMate;
            }

            chess.UnMakeMove(move);
        }

        moves.Clear();
        moves.AddRange(ValidMoves);
    }
}