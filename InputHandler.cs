using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    BoxCollider2D touchCol;
    Rigidbody2D rb;
    List<GameObject> path;

	void Start () {
        touchCol = gameObject.AddComponent<BoxCollider2D>();
        touchCol.size = .01f * (Vector2.one);
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        path = new List<GameObject>();
	}
	
	void Update () {
        touchCol.enabled = Input.GetMouseButton(0);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        switch (Puzzle.boardState)
        {
            case Puzzle.BoardState.Ready:
                if (Input.GetMouseButtonUp(0))
                {
                    FindObjectOfType<Puzzle>().ReceivePath(path);
                    path.Clear();
                }
                break;
            default:break;
        }
        
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        //check if the board will even accept input
        if (Puzzle.boardState == Puzzle.BoardState.Ready)
        {
            if (col.gameObject.tag == "Piece" && CheckIfValidPiece(col.gameObject))
            {
                Debug.Log("adding " + col.gameObject.name);
                path.Add(col.gameObject);
                GetComponent<EffectsHandler>().AddSparks(col.gameObject.transform.position, col.gameObject.transform.localScale.x);
            }
        }
    }

    bool CheckIfValidPiece(GameObject newPiece)
    {
        //first piece is always valid
        if (path.Count == 0)
        {
            return true;
        }
        //pieces already in the list not valid
        else if (path.FindIndex(delegate (GameObject go) { return go == newPiece; }) != -1)
        {
            return false;
        }
        int x = path[path.Count - 1].GetComponent<Piece>().x; int y = path[path.Count - 1].GetComponent<Piece>().y;
        int pieceX = newPiece.GetComponent<Piece>().x; int pieceY = newPiece.GetComponent<Piece>().y;
        //using XOR to exclude diagonal pieces.
        return (((pieceX == x - 1 || pieceX == x + 1) && pieceY == y) ^ ((pieceY == y - 1 || pieceY == y + 1) && pieceX == x));
    }
}
