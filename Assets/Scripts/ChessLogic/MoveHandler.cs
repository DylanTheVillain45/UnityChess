using System.Collections.Generic;

public static class MoveHandler {
    public static void CheckFilters(Chess chess, List<Move> moves) {
        List<Move> ValidMoves = new List<Move>();

        PieceColor myColor = moves[0].piece.color;
        PieceColor theirColor = myColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

        foreach (Move move in moves) {
            MoveMaker.CommitMove(chess, move);

            bool checkFilter = Check.IsCheck(chess, myColor);

            if (checkFilter) {
                MoveMaker.UnCommitMove(chess, move);
                continue;
            }

            ValidMoves.Add(move);

            bool moveCheck = Check.IsCheck(chess, theirColor);

            if (moveCheck) {
                move.isCheck = true;

                bool isCheckMate = true;

                List<Move> escapeMoves = MoveGenerator.GetAllMoves(chess, theirColor);


                foreach (Move escapeMove in escapeMoves) {

                    MoveMaker.CommitMove(chess, escapeMove);

                    // maybe sort list to put king moves first?

                    bool isStillCheck = Check.IsCheck(chess, theirColor);

                    if (isStillCheck == false) {
                        isCheckMate = false;
                        MoveMaker.UnCommitMove(chess, escapeMove);
                        break;
                    }
                    MoveMaker.UnCommitMove(chess, escapeMove);
                }

                move.isCheckMate = isCheckMate;
            }

            MoveMaker.UnCommitMove(chess, move);
        }

        moves.Clear();
        moves.AddRange(ValidMoves);
    }
}