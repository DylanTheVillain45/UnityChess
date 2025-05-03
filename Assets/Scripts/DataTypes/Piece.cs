public class Piece {
    public Type type;
    public PieceColor color;
    public int y;
    public int x;

    public Piece(Type type, PieceColor color, int y, int x){
        this.type = type;
        this.color = color;
        this.y = y;
        this.x = x;
    }
}