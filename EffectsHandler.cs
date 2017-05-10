using UnityEngine;

public class EffectsHandler : MonoBehaviour {

    GameObject sparks;
    GameObject solvedX, solvedO;
    GameObject parent;

	// Use this for initialization
	void Start () {
        sparks = (GameObject)Resources.Load("VFX/Sparks");
        solvedX = (GameObject)Resources.Load("VFX/SolvedX");
        solvedO = (GameObject)Resources.Load("VFX/SolvedO");
        parent = new GameObject();
        parent.name = "VFX";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddSparks(Vector3 pos, float scale)
    {
        GameObject newSparks = Instantiate(sparks, pos + ((scale/2) * (Vector3.right + Vector3.down)), Quaternion.identity, parent.transform);
        ParticleSystem.ShapeModule shapeModule = newSparks.GetComponent<ParticleSystem>().shape;
        shapeModule.radius = scale / 2;
    }

    public void DestroySparks()
    {
        foreach(Transform child in parent.transform)
        {
            if (child.name == "Sparks(Clone)") Destroy(child.gameObject);
        }
    }

    public void AddSolvedFX()
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Piece"))
        {
            GameObject newEffect;
            ParticleSystem.ShapeModule shapeModule;
            switch (go.GetComponent<Piece>().shape)
            {
                case Piece.Shape.X:
                    newEffect = Instantiate(solvedX,
                        go.transform.position + (go.transform.localScale.x/2 * (Vector3.right + Vector3.down)),
                        Quaternion.identity,
                        parent.transform);
                    shapeModule = newEffect.GetComponent<ParticleSystem>().shape;
                    shapeModule.radius = go.transform.localScale.x * .8f;
                    break;
                case Piece.Shape.O:
                    newEffect = Instantiate(solvedO,
                        go.transform.position + (go.transform.localScale.x/2 * (Vector3.right + Vector3.down)),
                        Quaternion.identity,
                        parent.transform);
                    shapeModule = newEffect.GetComponent<ParticleSystem>().shape;
                    shapeModule.radius = go.transform.localScale.x * .8f;
                    break;
                default:break;
            }
        }
    }
}
