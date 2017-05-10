using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif

public class Puzzle : MonoBehaviour {

    public enum BoardState { Ready, Moving, Solved }
    public static BoardState boardState = BoardState.Ready;

    //initialized so board can be created properly on editor awake & game awake
    [SerializeField]
    byte col = 1;
    [SerializeField]
    byte row = 1;
    [SerializeField]
    List<Row> board = new List<Row>();
    [SerializeField]
    bool updatePuzzle = false;

    float unit;
    Vector3 offset;

    //distinct from 'board,' actually contains piece OBJECTS
    public List<List<GameObject>> puzzle;

    //holds path to set board to-- stored locally so animations can be performed first
    List<GameObject> currentPath;

    //animation-relevant members
    const float moveTimeLimit = .8f;
    float moveTime;
    List<Vector3> pathPositions;
	
	void Awake() {
        Vector3 screenMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        float xScale = screenMax.x / (3 * col + 1); float yScale = screenMax.y / (3 * row + 1);
        unit = Mathf.Min(xScale, yScale);
        offset = new Vector3((screenMax.x - (3 * col + 1) * unit), -(screenMax.y - (3 * row + 1) * unit)) / 2;
        RefreshBoard();
        PopulatePuzzle();

        currentPath = new List<GameObject>();
        moveTime = 0;
        pathPositions = new List<Vector3>();
	}
	

	void Update () {
#if UNITY_EDITOR
        //board resize, recreate the puzzle.
        if (RefreshBoard() || updatePuzzle)
        {
            PopulatePuzzle();
            updatePuzzle = false;
        }
#endif
        switch (boardState)
        {
            case BoardState.Moving:
                AnimatePath();
                break;
            default:break;
        }
    }

    bool RefreshBoard()
    {
        bool isEdited = false;
        //strange "while" logic-- it runs in both editor and in game.

        //update number of rows in the board
        if (board.Count != row)
        {
            isEdited = true;
            while (board.Count < row)
            {
                board.Add(new Row());
            }
            while (board.Count > row)
            {
                board.RemoveAt(board.Count - 1);
            }
        }
        //update number of members in each row
        if (board[row - 1].shapes.Count != col)
        {
            isEdited = true;
            if (board[row - 1].shapes.Count < col)
            {
                for (int i = 0; i < board.Count; i++)
                {
                    while (board[i].shapes.Count < col)
                    {
                        board[i].shapes.Add(Piece.Shape.Empty);
                    }
                }
            }
            else if (board[row - 1].shapes.Count > col)
            {
                for (int i = 0; i < board.Count; i++)
                {
                    while (board[i].shapes.Count > col)
                    {
                        board[i].shapes.RemoveAt(board[i].shapes.Count - 1);
                    }
                }
            }
        }
        return isEdited;
    }

    void PopulatePuzzle()
    {
        //kill old puzzle
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Piece"))
        {
            DestroyImmediate(p);
        }
        //start fresh
        puzzle = new List<List<GameObject>>();
        for (int i = 0; i < row; i++)
        {
            puzzle.Add(new List<GameObject>());
            for (int j = 0; j < col; j++)
            {
                GameObject tempPiece = new GameObject();
                tempPiece.AddComponent<Piece>();
                tempPiece.GetComponent<Piece>().Instantiate(j, i, board[i].shapes[j], gameObject.transform);
                puzzle[puzzle.Count - 1].Add(tempPiece);
            }
        }
        PlacePieces();
    }

    void PlacePieces()
    {
        
        //then draw, pieces own their own spriterenderers
        foreach (List<GameObject> list in puzzle)
        {
            foreach(GameObject p in list)
            {
                p.GetComponent<Piece>().DrawAtPosAndScale(unit, offset);
            }
        }
    }

    public void ReceivePath(List<GameObject> path)
    {
        currentPath = new List<GameObject>();
        currentPath.AddRange(path);
        SetAnimationMembers(); //for animations.
        ChangeState(BoardState.Moving); //do them!
    }

    void ResetBoardByPath()
    {
        for (int i = 0; i < currentPath.Count / 2; i++)
        {
            int topX = currentPath[i].GetComponent<Piece>().x;
            int topY = currentPath[i].GetComponent<Piece>().y;
            int botX = currentPath[currentPath.Count - 1 - i].GetComponent<Piece>().x;
            int botY = currentPath[currentPath.Count - 1 - i].GetComponent<Piece>().y;

            board[topY].shapes[topX] = currentPath[currentPath.Count - 1 - i].GetComponent<Piece>().shape;
            board[botY].shapes[botX] = currentPath[i].GetComponent<Piece>().shape;
        }
        PopulatePuzzle();
        if (GetComponent<SolutionHandler>().IsSolved())
        {
            ChangeState(BoardState.Solved);
        }
    }

    void AnimatePath()
    {
        moveTime += Time.deltaTime;
        if (moveTime >= moveTimeLimit)
        {
            ChangeState(BoardState.Ready);
            return;
        }
        for(int i = 0; i < currentPath.Count; i++)
        {
            float progress = Mathf.Lerp(i, Mathf.Abs(i - (currentPath.Count - 1)), Mathf.Pow(Mathf.Sin(Mathf.PI * (moveTime / moveTimeLimit) / 2f),2));
            puzzle[currentPath[i].GetComponent<Piece>().y][currentPath[i].GetComponent<Piece>().x].transform.position =
                Vector3.Slerp(pathPositions[Mathf.FloorToInt(progress)],
                pathPositions[Mathf.CeilToInt(progress)],
                progress - Mathf.Floor(progress));
        }
    }

    void SetAnimationMembers()
    {
        moveTime = 0;
        pathPositions = new List<Vector3>();
        foreach(GameObject go in currentPath)
        {
            pathPositions.Add(go.transform.position);
        }
    }

    public void ChangeState(BoardState newState)
    {
        boardState = newState;
        switch (newState)
        {
            case BoardState.Ready:
                FindObjectOfType<EffectsHandler>().DestroySparks();
                ResetBoardByPath();
                break;
            case BoardState.Solved:
                FindObjectOfType<EffectsHandler>().AddSolvedFX();
                break;
            case BoardState.Moving:

                break;
        }
    }
}


//wrapper class so editor will serialize 'nested list'
[System.Serializable]
public class Row
{
    public List<Piece.Shape> shapes;

    public Row()
    {
        shapes = new List<Piece.Shape>();
    }
}

//pure editor GUI customizing
#if UNITY_EDITOR
[CustomEditor(typeof(Puzzle))]
[CanEditMultipleObjects]
public class PuzzleEditor : Editor
{
    SerializedProperty row, col, board, updatePuzzle;

    void OnEnable()
    {
        row = serializedObject.FindProperty("row");
        col = serializedObject.FindProperty("col");
        board = serializedObject.FindProperty("board");
        updatePuzzle = serializedObject.FindProperty("updatePuzzle");
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 45;
        serializedObject.Update();
        //update?
        updatePuzzle.boolValue = GUILayout.Button("Update");

        //get board width/height
        EditorGUILayout.LabelField("Board Dimensions");
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(row);
        EditorGUILayout.PropertyField(col);
        EditorGUILayout.EndHorizontal();

        //floor width & height at 1
        if (row.intValue < 1) row.intValue = 1;
        if (col.intValue < 1) col.intValue = 1;
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        //show board
        EditorGUILayout.PropertyField(board, includeChildren:true);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif