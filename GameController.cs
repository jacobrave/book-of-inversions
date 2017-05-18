using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public List<GameObject> puzzles;
    int currentIndex;
    GameObject currentPuzzle;

    public GameObject guiCanvas;

	void Start () {
        currentIndex = 0;
        currentPuzzle = Instantiate(puzzles[currentIndex]);
    }
	
	public void Next () {
        currentIndex++;
        currentIndex %= puzzles.Count;
        InstantiatePuzzle();
	}

    public void Retry()
    {
        InstantiatePuzzle();
    }

    void InstantiatePuzzle()
    {
        Destroy(currentPuzzle);
        currentPuzzle = Instantiate(puzzles[currentIndex]);
        FindObjectOfType<Puzzle>().ChangeState(Puzzle.BoardState.Ready);
        SetCanvasActive(false);
    }

    public void SetCanvasActive(bool isOn)
    {
        guiCanvas.SetActive(isOn);
    }
}
