using System.Collections.Generic;

public static class Evalution {
    public static readonly Dictionary<Type, int> PieceValueMap = new Dictionary<Type, int> {        
        {Type.Pawn, 100}, {Type.Knight, 320}, {Type.Bishop, 330}, {Type.Rook, 500}, {Type.Queen, 900}, {Type.King, 0}
    };

    public static int EvalBoard(Chess chess, PieceColor maximizingColor) {
        int eval = 0;

        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                Piece piece = chess.board[y, x];
                if (piece != null) {
                    int material = PieceValueMap[piece.type];
                    int position = GetPieceSquareValue(piece, x, y);
                    int total = material + position;

                    eval += (piece.color == PieceColor.White) ? total : -total;
                }
            }
        }

        eval *= maximizingColor == PieceColor.White ? 1 : -1;

        return eval;
    }

    public static readonly Dictionary<Type, int[,]> TableMap = new Dictionary<Type, int[,]> {
        {Type.Pawn, PawnTable}, {Type.Knight, KnightTable}, {Type.Bishop, BishopTable}, {Type.Rook, RookTable}, {Type.Queen, QueenTable}
    };
    private static int GetPieceSquareValue(Piece piece, int x, int y) {
        int[,] table = piece.type switch {
            Type.Pawn => PawnTable,
            Type.Knight => KnightTable,
            Type.Bishop => BishopTable,
            Type.Rook => RookTable,
            Type.Queen => QueenTable,
            _ => EmptyTable
        };

        int row = piece.color == PieceColor.White ? y : 7 - y;

        return table[row, x];
    }
    
    private static readonly int[,] EmptyTable = new int[8, 8];
    private static readonly int[,] PawnTable = {
        { 0,  0,  0,  0,  0,  0,  0,  0 },
        { 5, 10, 10,-20,-20, 10, 10,  5 },
        { 5, -5,-10,  0,  0,-10, -5,  5 },
        { 0,  0,  0, 20, 20,  0,  0,  0 },
        { 5,  5, 10, 25, 25, 10,  5,  5 },
        {10, 10, 20, 30, 30, 20, 10, 10 },
        {50, 50, 50, 50, 50, 50, 50, 50 },
        { 0,  0,  0,  0,  0,  0,  0,  0 }
    };

    private static readonly int[,] KnightTable = {
        {-50,-40,-30,-30,-30,-30,-40,-50},
        {-40,-20,  0,  0,  0,  0,-20,-40},
        {-30,  0, 10, 15, 15, 10,  0,-30},
        {-30,  5, 15, 20, 20, 15,  5,-30},
        {-30,  0, 15, 20, 20, 15,  0,-30},
        {-30,  5, 10, 15, 15, 10,  5,-30},
        {-40,-20,  0,  5,  5,  0,-20,-40},
        {-50,-40,-30,-30,-30,-30,-40,-50}
    };

    private static readonly int[,] BishopTable = {
        {-20,-10,-10,-10,-10,-10,-10,-20},
        {-10,  5,  0,  0,  0,  0,  5,-10},
        {-10, 10, 10, 10, 10, 10, 10,-10},
        {-10,  0, 10, 10, 10, 10,  0,-10},
        {-10,  5,  5, 10, 10,  5,  5,-10},
        {-10,  0,  5, 10, 10,  5,  0,-10},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-20,-10,-10,-10,-10,-10,-10,-20}
    };

    private static readonly int[,] RookTable = {
        { 0,  0,  0,  5,  5,  0,  0,  0 },
        {-5,  0,  0,  0,  0,  0,  0, -5 },
        {-5,  0,  0,  0,  0,  0,  0, -5 },
        {-5,  0,  0,  0,  0,  0,  0, -5 },
        {-5,  0,  0,  0,  0,  0,  0, -5 },
        {-5,  0,  0,  0,  0,  0,  0, -5 },
        { 5, 10, 10, 10, 10, 10, 10,  5 },
        { 0,  0,  0,  0,  0,  0,  0,  0 }
    };

    private static readonly int[,] QueenTable = {
        {-20,-10,-10, -5, -5,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5,  5,  5,  5,  0,-10},
        { -5,  0,  5,  5,  5,  5,  0, -5},
        {  0,  0,  5,  5,  5,  5,  0, -5},
        {-10,  5,  5,  5,  5,  5,  0,-10},
        {-10,  0,  5,  0,  0,  0,  0,-10},
        {-20,-10,-10, -5, -5,-10,-10,-20}
    };
}