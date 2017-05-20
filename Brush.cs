using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour {

    Vector3 lerpTarget;
    ParticleSystem.MinMaxCurve rate;

    void Awake()
    {
        SetPosition(FindObjectOfType<InputHandler>().gameObject.transform.position);
        lerpTarget = transform.position;

        ParticleSystem.EmissionModule emod = GetComponent<ParticleSystem>().emission;
        rate = emod.rateOverDistance;
        emod.rateOverDistance = 0;
    }
	

	void Update () {
        //transform.position = Vector3.Lerp(transform.position, lerpTarget, 0.2f);
        SetPosition(FindObjectOfType<InputHandler>().gameObject.transform.position);

        if (Input.GetMouseButtonUp(0))
        {
            LiftBrush();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            UseBrush();
        }
    }

    void UseBrush ()
    {
        ParticleSystem.EmissionModule emod = GetComponent<ParticleSystem>().emission;
        emod.rateOverDistance = rate;
    }

    void LiftBrush()
    {
        ParticleSystem.EmissionModule emod = GetComponent<ParticleSystem>().emission;
        emod.rateOverDistance = 0;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetLerpTarget(Vector3 position)
    {
        lerpTarget = position;
    }
}
