using UnityEngine;

public class PromoterManager : MonoBehaviour {
    public Type pieceType;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        MoveManager.instance.FindAndCommitPromotionMove(pieceType);
    }

    void OnMouseEnter()
    {
        spriteRenderer.color = GameManager.instance.hightlightColor;
    }

    void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
    }
}