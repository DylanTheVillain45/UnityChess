using System.Collections.Generic;

public static class MoveMaker {
    public static void CommitMove(Chess chess, Move move) {
        int startY = move.startY;
        int startX = move.startX;
        int endY = move.endY;
        int endX = move.endX;
        
        Piece piece = move.piece;
        (piece.x, piece.y) = (endX, endY);

        chess.board[startY, startX] = null;
        chess.board[endY, endX] = piece;

        if (move.isCastle) {
            int rookStartX = move.isShortCastle ? 7 : 0;
            int rookEndX = move.isShortCastle ? 5 : 3; 

            Piece rook = chess.board[startY, rookStartX];
            if (rook != null && rook.type == Type.Rook) {
                rook.x = rookEndX;
                chess.board[startY, rookEndX] = rook;
                chess.board[startY, rookStartX] = null;
            }
        }

        if (move.isEnpassant) {
            chess.board[startY, endX] = null;
        }

        if (move.isPromotion) {
            piece.type = move.promotionPiece;
        }
    }

    public static void UnCommitMove(Chess chess, Move move) {
        int startY = move.startY;
        int startX = move.startX;
        int endY = move.endY;
        int endX = move.endX;
        
        Piece piece = move.piece;
        Piece capturedPiece = move.capturedPiece;
        (piece.x, piece.y) = (startX, startY);
        chess.board[startY, startX] = piece;
        
        if (move.isCastle) {
            int rookStartX = move.isShortCastle ? 7 : 0;
            int rookEndX = move.isShortCastle ? 5 : 3; 

            Piece rook = chess.board[startY, rookEndX];
            if (rook != null && rook.type == Type.Rook) {
                rook.x = rookStartX;
                chess.board[startY, rookStartX] = rook;
                chess.board[startY, rookEndX] = null;
            }
        }

        if (move.isCapture) {
            if (move.isEnpassant) {
                chess.board[startY, endX] = capturedPiece;
            } else {
                chess.board[endY, endX] = capturedPiece;
            }
        }      
        else {
            chess.board[move.endY, move.endX] = null;    
        } 

        if (move.isPromotion) {
            piece.type = Type.Pawn;
        }
    }
}