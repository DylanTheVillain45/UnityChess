using System.Collections.Generic;

public static class MoveFinder {
    public static List<Move> FindMovesWithStart(Chess chess, Piece piece) {
        List<Move> moves = new List<Move>();

        for (int i = 0; i < chess.MovesList.Count; i++)
        {
            if (chess.MovesList[i].piece == piece)
            {
                moves.Add(chess.MovesList[i]);
            }
        }

        return moves;
    }

    public static Move FindMoveWithEndAndList(List<Move> moves, int endY, int endX) {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].endY == endY && moves[i].endX == endX)
            {
                return moves[i];
            }
        }
        return null;
    }

    public static Move FindMoveWithEndAndListPromotion(List<Move> moves, int endY, int endX, Type type) {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].endY == endY && moves[i].endX == endX && moves[i].isPromotion && moves[i].promotionPiece == type)
            {
                return moves[i];
            }
        }
        return null;
    }

}