public static class Check {
    public static bool IsCheck(Chess chess, PieceColor color) {
        Piece  king = FindKing(chess, color);

        if (king == null) {
            GameManager.instance.RaiseError("KING NOT FOUND");
            return false;
        }

        if (IsNonPawnCheck(chess, king) || IsPawnCheck(chess, king)) {
            return true;
        }
        
        return false;
    }

    static bool IsPawnCheck(Chess chess, Piece king) {
        int direction = king.color == PieceColor.White ? 1 : -1;
        for (int i = -1; i <= 1; i += 2) {
            int newY = king.y + direction;
            int newX = king.x + i;
            if (newX >= 0 && newX < 8 && newY >= 0 && newY < 8) {
                Piece landingSquare = chess.board[newY, newX];
                if (landingSquare != null && landingSquare.type == Type.Pawn && landingSquare.color != king.color) {
                    return true;
                }
            }
        }

        return false;
    }

    static bool IsNonPawnCheck(Chess chess, Piece king) {
        Type[] checkingPieces = {Type.Knight, Type.Bishop, Type.Rook, Type.Queen, Type.King};
        foreach (Type piece in checkingPieces) {
            foreach (var (dx, dy, repeatable) in MoveGenerator.PieceMap[piece]) {
                int newY = king.y + dy;
                int newX = king.x + dx;

                while (newY >= 0 && newY < 8 && newX >= 0 && newX < 8) {
                    Piece landingSquare = chess.board[newY, newX];
                    if (landingSquare != null) {
                        if (landingSquare.type == piece && landingSquare.color != king.color) {
                            return true;
                        }

                        break;
                    }

                    if (repeatable) {
                        newY += dy;
                        newX += dx;
                    } else {
                        break;
                    }
                }
            }
        }
        return false;
    }

    public static Piece FindKing(Chess chess, PieceColor color) {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (chess.board[i, j] != null && chess.board[i, j].type == Type.King && chess.board[i, j].color == color) {
                    return chess.board[i, j];
                }
            }
        }
        GameManager.instance.RaiseError("KING NOT FOUND");
        return null;
    }

}