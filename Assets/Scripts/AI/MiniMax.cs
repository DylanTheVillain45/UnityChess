using System.Collections.Generic;
using System;

public static class MiniMax {
    public static Move GetBestMove(Chess chess, int maxDepth, PieceColor aiColor) {
        int bestScore = int.MinValue;
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        Move bestMove = null;

        PieceColor maximizingColor = aiColor;
        PieceColor minimizingColor = maximizingColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

        Chess chessCopy = chess.DeepCopy();

        List<Move> posMoves = MoveGenerator.GetValidMoves(chessCopy, maximizingColor);
        MoveListOrdering.OrderForABPrune(posMoves);


        foreach (Move move in posMoves) {
            chessCopy.MakeMove(move);

            int score;
            if (move.isCheckMate) {
                score = 1000;
            } else {
                score = Beta(chessCopy, maximizingColor, minimizingColor, alpha, beta, 1, maxDepth);
            }

            if (score > bestScore) {
                bestScore = score;
                bestMove = move;
            }

            alpha = Math.Max(alpha, bestScore);
            chessCopy.UnMakeMove(move);
        }

        return bestMove;
    } 

    static int Alpha(Chess chess, PieceColor maximizingColor, PieceColor minimizingColor, int alpha, int beta, int depth, int maxDepth) {
        if (depth > maxDepth) return Evalution.EvalBoard(chess, maximizingColor);      

        List<Move> posMoves = MoveGenerator.GetValidMoves(chess, maximizingColor);
        if (posMoves.Count == 0) {
            bool inCheck = Check.IsCheck(chess, maximizingColor);
            if (inCheck) return -1000 + depth * 100;
            return 0;
        }

        MoveListOrdering.OrderForABPrune(posMoves);

        int bestScore = int.MinValue;
        foreach (Move move in posMoves) {
            chess.MakeMove(move);

            int score;
            if (move.isCheckMate) {
                score = 1000 - depth * 100;
            } else {
                score = Beta(chess, maximizingColor, minimizingColor, alpha, beta, depth + 1, maxDepth);
            }

            bestScore = Math.Max(bestScore, score);
            alpha = Math.Max(alpha, bestScore);

            chess.UnMakeMove(move);

            if (beta <= alpha) break;
        }

        return bestScore;
    }

    static int Beta(Chess chess, PieceColor maximizingColor, PieceColor minimizingColor, int alpha, int beta, int depth, int maxDepth) {
        if (depth > maxDepth) return Evalution.EvalBoard(chess, maximizingColor);

        List<Move> posMoves = MoveGenerator.GetValidMoves(chess, minimizingColor);
        if (posMoves.Count == 0) {
            bool inCheck = Check.IsCheck(chess, minimizingColor);
            if (inCheck) return 1000 - depth * 100;
            return 0;
        }

        MoveListOrdering.OrderForABPrune(posMoves);

        int bestScore = int.MaxValue;
        foreach (Move move in posMoves) {
            chess.MakeMove(move);

            int score;
            if (move.isCheckMate) {
                score = -1000 + depth * 100;
            } else {
                score = Alpha(chess, maximizingColor, minimizingColor, alpha, beta, depth + 1, maxDepth);
            }

            bestScore = Math.Min(bestScore, score);
            beta = Math.Min(beta, bestScore);

            chess.UnMakeMove(move);

            if (beta <= alpha) break;
        }

        return bestScore;
    }
}