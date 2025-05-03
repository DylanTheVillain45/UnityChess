using System.Collections.Generic;

public static class MoveListOrdering {
    public static void OrderForABPrune(List<Move> moveList) {
        moveList.Sort((a, b) => ScoreMove(b).CompareTo(ScoreMove(a)));  
    }

    private static int ScoreMove(Move move) {
        int score = 0;

        if (move.isCheckMate) score += int.MaxValue;
        else if (move.isCheck) score += 5000;

        if (move.isPromotion) score += 2000 + Evalution.PieceValueMap[move.promotionPiece] * 200;

        if (move.capturedPiece != null) {
            score += Evalution.PieceValueMap[move.capturedPiece.type] * 100;
        }

        if (move.piece.type == Type.Queen) score += 50;

        return score;
    }
}