using UnityEngine;
using System.Collections.Generic;

public class MoveManager : MonoBehaviour {
    public static MoveManager instance;
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public int startY;
    public int startX;
    public int endY;
    public int endX;
    public bool isStart = true;
    private List<Move> moveList = new List<Move>();
    private List<GameObject> higlightedTiles = new List<GameObject>();
    public GameObject PromotingSelector;
    private GameObject currentPromoter;

    public void HandleClick(GameObject tileObj)
    {
        if (currentPromoter != null) { 
            UnHighlightTiles();
            Destroy(currentPromoter);
        }
        if (tileObj == null || GameManager.instance.isMoving) return;

        TileManager tile = tileObj.GetComponent<TileManager>();
        PieceManager pieceManager = tile.transform.childCount > 0 ? tile.transform.GetChild(0).GetComponent<PieceManager>() : null;
        Piece piece = null;
        if (pieceManager != null) piece = pieceManager.pieceData;

        int y = tile.y;
        int x = tile.x;

        if (piece != null && piece.color == GameManager.instance.chess.GameColor)
        {
            UnHighlightTiles();

            moveList = MoveFinder.FindMovesWithStart(GameManager.instance.chess, piece);

            if (moveList.Count > 0) {
                SetStart(y, x);

                for (int i = 0; i < moveList.Count; i++)
                {
                    Move move = moveList[i];
                    GameObject landingTileObj = GameManager.instance.tileBoard[move.endY, move.endX];
                    SpriteRenderer landingTileRender = landingTileObj.GetComponent<SpriteRenderer>();
                    higlightedTiles.Add(landingTileObj);
                    landingTileRender.color = GameManager.instance.hightlightColor;
                }
            }
        } else if (isStart == false && (piece == null || (piece.color != GameManager.instance.chess.GameColor) && piece.type != Type.King)) {
            Move move = MoveFinder.FindMoveWithEndAndList(moveList, y, x);
            (endY, endX) = (y, x);
            if (move == null) return;

            if (move.isPromotion) {
                FindPromotingPiece(move.piece.color);
            } else {
                CommitMove(move);
            }
        }
    }

    private void FindPromotingPiece(PieceColor color) {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);



        GameObject piecePromoter = Instantiate(PromotingSelector, this.transform);
        currentPromoter = piecePromoter;
        piecePromoter.transform.position = new Vector3(mouseWorldPos.x + (piecePromoter.transform.localScale.x / 2), mouseWorldPos.y + 3 * (color == PieceColor.White ? (-piecePromoter.transform.localScale.y / 2) : (piecePromoter.transform.localScale.y / 2)), -5);
    }

    public void FindAndCommitPromotionMove(Type type) {
        Destroy(currentPromoter);
        Move move = MoveFinder.FindMoveWithEndAndListPromotion(moveList, endY, endX, type);
        CommitMove(move);
    }

    public void CommitMove(Move move) {
        SetReset();
        GameManager.instance.MakeMove(move);
    }

    private void SetStart(int y, int x)
    {
        this.startY = y;
        this.startX = x;
        this.isStart = false;
    }

    private void SetReset()
    {
        UnHighlightTiles();
        this.startY = 0;
        this.startX = 0;
        this.endY = 0;
        this.endX = 0;
        this.isStart = true;
        higlightedTiles = new List<GameObject>();
    }

    private void UnHighlightTiles()
    {
        foreach (GameObject tileObj in higlightedTiles)
        {
            SpriteRenderer landingTileRender = tileObj.GetComponent<SpriteRenderer>();
            TileManager tile = tileObj.GetComponent<TileManager>();
            landingTileRender.color = (tile.y + tile.x) % 2 == 0 ? GameManager.instance.blackTile : GameManager.instance.whiteTIle;
        }
    }
}