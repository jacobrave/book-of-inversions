using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Puzzle))]

public class SolutionHandler : MonoBehaviour {

    public bool IsSolved()
    {
        //grab all pieces
        List<GameObject> pieces = new List<GameObject>();
        pieces.AddRange(GameObject.FindGameObjectsWithTag("Piece"));

        List<GameObject> emptyPieces = new List<GameObject>();
        //first, remove all empty pieces.
        foreach(GameObject piece in pieces)
        {
            if (piece.GetComponent<Piece>().shape == Piece.Shape.Empty)
            {
                emptyPieces.Add(piece);
            }
        }
        foreach(GameObject emptyPiece in emptyPieces)
        {
            pieces.Remove(emptyPiece);
        }
        //then, check if it's solved
        while (pieces.Count > 0)
        {
            //get all valid adjacent nodes to first, mark first as visited
            //continue until the whole valid region is visited
            pieces[0].GetComponent<Piece>().visited = true;
            List<GameObject> adjacentPieces = new List<GameObject>();
            adjacentPieces.Add(pieces[0]);
            //loop until we're done
            while (true)
            {
                List<GameObject> tempAdjacentPieces = new List<GameObject>();
                //grab the adjacent pieces of every adjacent piece
                foreach (GameObject piece in adjacentPieces)
                {
                    tempAdjacentPieces.AddRange(GetValidAdjacentPieces(piece, pieces));
                }
                //if there were any adjacent pieces at all
                if (tempAdjacentPieces.Count != 0)
                {
                    adjacentPieces.AddRange(tempAdjacentPieces);
                }
                else
                {
                    break;
                }
            }

            //so that's every piece in this region. let's see if it contains every piece of its shape
            Piece.Shape regionShape = pieces[0].GetComponent<Piece>().shape;
            foreach (GameObject piece in adjacentPieces)
            {
                piece.GetComponent<Piece>().visited = false; //reset for later checks
                pieces.Remove(piece); //get rid of the whole region
                                      //if this is the last region in the puzzle, the containing while loop breaks (pieces is empty)
                                      //if not, continue with remaining pieces (no more of this shape will be included)
            }
            //if the region does not contain every piece of its shape, that shape will still remain
            foreach(GameObject piece in pieces)
            {
                if (piece.GetComponent<Piece>().shape == regionShape)
                {
                    return false;
                }
            }
        }

        return true;
    }

    List<GameObject> GetValidAdjacentPieces(GameObject current, List<GameObject> pieces)
    {
        List<GameObject> retList = new List<GameObject>();

        //get piece x & y values
        int x = current.GetComponent<Piece>().x; int y = current.GetComponent<Piece>().y;
        foreach(GameObject piece in pieces)
        {
            if (!piece.GetComponent<Piece>().visited)
            {
                int pieceX = piece.GetComponent<Piece>().x; int pieceY = piece.GetComponent<Piece>().y;
                //using XOR to exclude diagonal pieces.
                if (((pieceX == x - 1 || pieceX == x + 1) && pieceY == y) ^ ((pieceY == y - 1 || pieceY == y + 1) && pieceX == x))
                {
                    if (piece.GetComponent<Piece>().shape == current.GetComponent<Piece>().shape)
                    {
                        //make sure to mark as visited, no piece redundancy
                        retList.Add(piece);
                        piece.GetComponent<Piece>().visited = true;
                    }
                }
            }
        }

        return retList;
    }
	
}
