using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Type {Pawn, Knight, Bishop, Rook, Queen, King}
public enum PieceColor { White, Black }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
 
    public Color hightlightColor;
    public Color blackTile;
    public Color whiteTIle;

    public GameObject[,] tileBoard;

    public GameObject TilePref;
    public GameObject PiecePref;
    public Transform boardParent;

    public Sprite[] pieceSprites = new Sprite[12];
    public Dictionary<(Type, PieceColor), int> pieceMap = new Dictionary<(Type, PieceColor), int>()
    {
        { (Type.Pawn, PieceColor.White), 0 },
        { (Type.Knight, PieceColor.White), 1 },
        { (Type.Bishop, PieceColor.White), 2 },
        { (Type.Rook, PieceColor.White), 3 },
        { (Type.Queen, PieceColor.White), 4 },
        { (Type.King, PieceColor.White), 5 },

        { (Type.Pawn, PieceColor.Black), 6 },
        { (Type.Knight, PieceColor.Black), 7 },
        { (Type.Bishop, PieceColor.Black), 8 },
        { (Type.Rook, PieceColor.Black), 9 },
        { (Type.Queen, PieceColor.Black), 10 },
        { (Type.King, PieceColor.Black), 11 }
    };

    public bool isMoving = false;
    public Chess chess;
    public int depth = 3;

    public void Start() {
        chess = new Chess();
        SetUpBoard();
        GetMoves();
    }

    private void GetMoves() {
        chess.GetMoves();
    }

    public void MakeMove(Move move) {
        GameObject tileStart = tileBoard[move.startY, move.startX];
        GameObject tileEnd = tileBoard[move.endY, move.endX];


        foreach (Transform c in tileEnd.transform) {
            Destroy(c.gameObject);
        }

        if (move.isEnpassant) {
            GameObject epTile = tileBoard[move.startY, move.endX];
            foreach (Transform c in epTile.transform) {
                Destroy(c.gameObject);
            }
        }

        if (move.isCastle) {
            int rookStartX = move.isShortCastle ? 7 : 0;
            int rookEndX = move.isShortCastle ? 5 : 3; 

            GameObject rookPiece = tileBoard[move.startY, rookStartX].transform.GetChild(0).gameObject;
            GameObject rookSpot = tileBoard[move.startY, rookEndX];

            rookPiece.transform.SetParent(rookSpot.transform);
            rookPiece.transform.position = new Vector3(rookSpot.transform.position.x, rookSpot.transform.position.y, -1);
        }

        GameObject mover  = tileStart.transform.GetChild(0).gameObject;
        mover.transform.SetParent(tileEnd.transform);
        mover.transform.position = new Vector3(tileEnd.transform.position.x, tileEnd.transform.position.y, -1);

        chess.MakeMove(move);

        if (move.isPromotion) {
            mover.GetComponent<SpriteRenderer>().sprite = pieceSprites[pieceMap[(move.promotionPiece, move.piece.color)]];
        }

        chess.isAi = !chess.isAi;
        ShowBoard();

        if (chess.isAi || false) {
            MakeAiMove();
        } else {
            GetMoves();
        }

        // GetMoves();
    }

    private void MakeAiMove() {
        Move bestMove = MiniMax.GetBestMove(chess, depth, chess.GameColor);
        MakeMove(bestMove);
        // UpdateBoard();
    }

    public void ShowBoard() {
        char[] chessSquares = { '□', '■' };
        Dictionary<(Type, PieceColor), char> CharacterMap = new Dictionary<(Type, PieceColor), char>
        {
            { (Type.Pawn, PieceColor.White), '♟' }, { (Type.Knight, PieceColor.White), '♞' },
            { (Type.Bishop, PieceColor.White), '♝' }, { (Type.Rook, PieceColor.White), '♜' },
            { (Type.Queen, PieceColor.White), '♛' }, { (Type.King, PieceColor.White), '♚' },

            { (Type.Pawn, PieceColor.Black), '♙' }, { (Type.Knight, PieceColor.Black), '♘' },
            { (Type.Bishop, PieceColor.Black), '♗' }, { (Type.Rook, PieceColor.Black), '♖' },
            { (Type.Queen, PieceColor.Black), '♕' }, { (Type.King, PieceColor.Black), '♔' },
        };
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine("----------------------------");

        for (int i = 0; i < 8; i++) {
            sb.Append($"{i + 1} ");
            for (int j = 0; j < 8; j++) {
                Piece piece = chess.board[7 - i, j];
                if (piece != null) {
                    char unicodeChar = CharacterMap[(piece.type, piece.color)];
                    sb.Append($" {unicodeChar} ");
                } else {
                    sb.Append($" {chessSquares[1 - ((7 - i) + j) % 2]} ");
                }
            }
            sb.Append(" |\n");
        }

        sb.AppendLine("---a--b--c--d--e--f--g--h---");
        Debug.Log(sb.ToString());
    }


    private void SetUpBoard() {
        chess.board = new Piece[8,8];
        tileBoard = new GameObject[8, 8];
        chess.GameColor = PieceColor.White;

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                GameObject tileObj = Instantiate(TilePref, boardParent);

                tileObj.transform.position = new Vector2((j - 3.5f) * tileObj.transform.localScale.x + 2, (i - 3.5f) * tileObj.transform.localScale.y);

                SpriteRenderer tileRenderer = tileObj.GetComponent<SpriteRenderer>();

                if ((i + j) % 2 == 0) {
                    tileRenderer.color = blackTile;
                } else {
                    tileRenderer.color = whiteTIle;
                }

                tileObj.name = $"Tile-{i}{j}";
                TileManager tile = tileObj.GetComponent<TileManager>();
                tile.x = j; tile.y = i;
                tileBoard[i, j] = tileObj;

                if (i == 0 || i == 1 || i == 6 || i == 7) {
                    GameObject pieceObj = Instantiate(PiecePref, tileObj.transform);
                    PieceManager pieceManager = pieceObj.GetComponent<PieceManager>();

                    PieceColor pieceColor;
                    Type type = Type.Pawn;

                    if (i == 1 || i == 6) {
                        pieceColor = i == 1 ? PieceColor.White : PieceColor.Black;
                    } else {
                        pieceColor = i == 0 ? PieceColor.White : PieceColor.Black;
                        if (j == 0 || j == 7) type = Type.Rook;
                        if (j == 1 || j == 6) type = Type.Knight;
                        if (j == 2 || j == 5) type = Type.Bishop;
                        if (j == 3) type = Type.Queen;
                        if (j == 4) type = Type.King;

                    }
                    pieceManager.SetPieceManager(type, pieceColor, i, j, pieceSprites[pieceMap[(type, pieceColor)]]);
                    chess.board[i, j] = pieceManager.pieceData;
                }
            }
        }
    }

    public void RaiseError(string message) {
        Debug.LogError(message);
    }
}
