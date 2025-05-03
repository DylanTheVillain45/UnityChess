using UnityEngine;

public class PieceManager : MonoBehaviour {
    public Piece pieceData;
    public SpriteRenderer spriteRenderer;

    public void SetPieceManager(Type type, PieceColor color, int y, int x, Sprite sprite) {
        this.pieceData = new Piece(type, color, y, x);
        this.spriteRenderer.sprite = sprite;
    }
}