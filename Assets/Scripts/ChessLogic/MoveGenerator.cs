using System.Collections.Generic;

public static class MoveGenerator {
    static readonly Type[] promotablePieces = {Type.Knight, Type.Bishop, Type.Rook, Type.Queen};
    
    public static List<Move> GetValidMoves(Chess chess, PieceColor color) {
        List<Move> moves = GetAllMoves(chess, color);
        MoveHandler.CheckFilters(chess, moves);
        return moves;
    }

    public static List<Move> GetAllMoves(Chess chess, PieceColor color) {
        List<Move> MoveList = new List<Move>();

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (chess.board[i, j] != null && chess.board[i, j].color == color) {
                    GetMoves(chess, chess.board[i,j], MoveList);
                }
            }
        }

        return MoveList;
    }

    static void GetMoves(Chess chess, Piece piece, List<Move> moveList) {
        if (piece.type == Type.Pawn) GetPawnMoves(chess, piece, moveList);
        else GetNonPawnMoves(chess, piece, moveList);
        if (piece.type == Type.King) GetCastleMoves(chess, piece, moveList);
    }

    static void GetPawnMoves(Chess chess, Piece piece, List<Move> moveList) {
        int direction = piece.color == PieceColor.White ? 1 : -1;
        int startRow = piece.color == PieceColor.White ? 1 : 6;
        int enPassantRow = piece.color == PieceColor.White ? 4 : 3;
        int endRow = piece.color == PieceColor.White ? 7 : 0;

        if (piece.y + direction < 0 || piece.y + direction >= 8) return;

        if (chess.board[piece.y + direction, piece.x] == null) {
            if (piece.y + direction == endRow) {
                for (int i = 0; i < promotablePieces.Length; i++) {
                    Move newMove = new Move(piece, piece.y, piece.x, piece.y + direction, piece.x, false, null, false, false, false, true, promotablePieces[i]);
                    moveList.Add(newMove);
                }
            } else {
                Move newMove = new Move(piece, piece.y, piece.x, piece.y + direction, piece.x);
                moveList.Add(newMove);
            }
            if (piece.y == startRow && chess.board[piece.y + direction * 2, piece.x] == null)
            {
                Move newMove2 = new Move(piece, piece.y, piece.x, piece.y + direction * 2, piece.x);
                moveList.Add(newMove2);
            }
        }

        for (int dx = -1; dx <= 1; dx += 2)
        {
            int newY = piece.y + direction;
            int newX = piece.x + dx;
            if (newX >= 0 && newX < 8)
            {
                Piece capturedPiece = chess.board[newY, newX];
                Piece enPassantCapturePiece = chess.board[piece.y, newX];
                if (capturedPiece != null && capturedPiece.color != piece.color && capturedPiece.type != Type.King)
                {
                    if (newY == endRow) {
                        for (int i = 0; i < promotablePieces.Length; i++) {
                            Move newMove = new Move(piece, piece.y, piece.x, newY, newX, true, capturedPiece, false, false, false, true, promotablePieces[i]);
                            moveList.Add(newMove);
                        }
                    } else {
                        Move newMove = new Move(piece, piece.y, piece.x, newY, newX, true, capturedPiece);
                        moveList.Add(newMove);
                    }
                }
                else if (enPassantCapturePiece != null && enPassantCapturePiece.type == Type.Pawn && enPassantCapturePiece.color != piece.color && piece.y == enPassantRow)
                {
                    if (IsValidEnPassant(chess, enPassantCapturePiece))
                    {
                        Move newMove = new Move(piece, piece.y, piece.x, newY, newX, true, enPassantCapturePiece, false, false, true);
                        moveList.Add(newMove);
                    }
                }
            }
        }
    }

    static bool IsValidEnPassant(Chess chess, Piece capturedPiece) {
        if (chess.PastMoves.Count == 0 || capturedPiece == null) {
            GameManager.instance.RaiseError("piece is null");
            return false;
        }


        var lastTurn = chess.PastMoves[^1];
        Move lastMove = capturedPiece.color == PieceColor.White ? lastTurn.Item1 : lastTurn.Item2;

        if (lastMove == null || lastMove.piece == null) {
            GameManager.instance.RaiseError("last move is null");
            return false;
        }

        int startRow = capturedPiece.color == PieceColor.White ? 1 : 6;

        return lastMove.startY == startRow;
    }

    
    static void GetNonPawnMoves(Chess chess, Piece piece, List<Move> moveList)
    {
        foreach (var (dx, dy, repeatable) in PieceMap[piece.type])
        {
            int newX = piece.x + dx;
            int newY = piece.y + dy;

            while (newX < 8 && newX >= 0 && newY < 8 && newY >= 0)
            {
                Piece landingSquare = chess.board[newY, newX];

                if (landingSquare == null)
                {
                    Move newMove = new Move(piece, piece.y, piece.x, newY, newX);
                    moveList.Add(newMove);
                }

                else if (landingSquare.color != piece.color && landingSquare.type != Type.King)
                {
                    Move newMove = new Move(piece, piece.y, piece.x, newY, newX, true, landingSquare);
                    moveList.Add(newMove);
                    break;
                }

                else break;

                if (!repeatable) break;

                newY += dy;
                newX += dx;
            }
        }
    }

    static void GetCastleMoves(Chess chess, Piece king, List<Move> moveList) {
        if (HasMoved(chess, king)) {
            return;
        };

        int y = king.color == PieceColor.White ? 0 : 7;

        Piece rookShort = chess.board[y, 0];
        if (rookShort != null && rookShort.type == Type.Rook && rookShort.color == king.color && !HasMoved(chess, rookShort)) {
            if (IsPathClear(chess, king, true)) {
                if (IsSafeForKing(chess, king, new int[] {5, 6})) {
                    Move newMove = new Move(king, king.y, king.x, king.y, king.x + 2, false, null, true, true);
                    moveList.Add(newMove);
                }
            }
        }

        Piece rookLong = chess.board[y, 7];
        if (rookLong != null && rookLong.type == Type.Rook && rookLong.color == king.color && !HasMoved(chess, rookShort)) {
            if (IsPathClear(chess, king, false)) {
                if (IsSafeForKing(chess, king, new int[] {3, 2})) {
                    Move newMove = new Move(king, king.y, king.x, king.y, king.x - 2, false, null, true, false);
                    moveList.Add(newMove);
                }
            }
        }
    }
    static bool IsSafeForKing(Chess chess, Piece king, int[] pathX) {
        int startX = king.x;
        foreach (int x in pathX) {
            if (chess.board[king.y, x] != null) return false;
            chess.board[king.y, king.x] = null;
            king.x = x;
            chess.board[king.y, king.x] = king;
            if (Check.IsCheck(chess, king.color)) {
                chess.board[king.y, king.x] = null;
                king.x = startX;
                chess.board[king.y, king.x] = king;
                return false;
            }
        }
        chess.board[king.y, king.x] = null;
        king.x = startX;
        chess.board[king.y, king.x] = king;
        return true;
    }

    static bool IsPathClear(Chess chess, Piece king, bool isShort) {
        if (isShort) {
            for (int i = 1; i <= 2; i++) {
                int x = king.x + i;
                if (x < 0 || x > 7) return false;
                if (chess.board[king.y, king.x + i] != null) return false;
            }

            return true;
        } else {
            for (int i = 1; i <= 3; i++) {
                int x = king.x - i;
                if (x < 0 || x > 7) return false;
                if (chess.board[king.y, king.x - i] != null) return false;
            }

            return true;
        }
    }

    static bool HasMoved(Chess chess, Piece piece) {
        if (piece == null) return true;
        foreach (var move in chess.PastMoves) {
            if (piece.color == PieceColor.White) {
                if (move.Item1 != null && move.Item1.piece == piece) {
                    return true;
                }
            } else {
                if (move.Item2 != null && move.Item2.piece == piece) {
                    return true;
                }
            }
        }

        return false;
    }

    public static readonly Dictionary<Type, List<(int, int, bool)>> PieceMap = new Dictionary<Type, List<(int, int, bool)>> {
        {Type.Knight, new List<(int, int, bool)> {(1, 2, false), (2, 1, false), (-1, 2, false), (-2, 1, false), (1, -2, false), (2, -1, false), (-1, -2, false), (-2, -1, false)}},
        {Type.Bishop, new List<(int, int, bool)> {(1, 1, true), (-1, 1, true), (1, -1, true), (-1, -1, true)}},
        {Type.Rook, new List<(int, int, bool)> {(1, 0, true), (-1, 0, true), (0, 1, true), (0, -1, true)}},
        {Type.Queen, new List<(int, int, bool)> {(1, 1, true), (-1, 1, true), (1, -1, true), (-1, -1, true), (1, 0, true), (-1, 0, true), (0, 1, true), (0, -1, true)}},
        {Type.King, new List<(int, int, bool)> {(1, 1, false), (-1, 1, false), (1, -1, false), (-1, -1, false), (1, 0, false), (-1, 0, false), (0, 1, false), (0, -1, false)}},
    };

}