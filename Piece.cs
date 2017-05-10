using UnityEngine;

public class Piece : MonoBehaviour{

    public enum Shape { Empty, X, O }

    Sprite sprite;
    SpriteRenderer sr;
    BoxCollider2D col;
    public Shape shape;
    public int x, y;

    [HideInInspector]
    public bool visited;

    public void Instantiate(int x, int y, Shape shape, Transform parent)
    {
        this.x = x; this.y = y;
        this.shape = shape;
        tag = "Piece";
        visited = false;

        //looks for a sprite with the same name as the enum.
        transform.parent = parent;
        name = x + " " + y + " " + shape.ToString();

        //adds components if it's not empty.
        if (shape != Shape.Empty)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Sprites/" + shape.ToString());
            col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }

    public void DrawAtPosAndScale(float unit, Vector3 offset)
    {
        transform.position = Camera.main.ScreenToWorldPoint(Screen.height * Vector3.up);
        transform.position += offset;
        transform.position += unit * ((3 * x) + 1) * Vector3.right + unit * ((3 * y) + 1) * Vector3.down;
        transform.position -= transform.position.z * Vector3.forward; //NO Z
        transform.localScale *= 2 * unit;
    }

    public void Move(int x, int y)
    {
        this.x = x; this.y = y;
    }
}