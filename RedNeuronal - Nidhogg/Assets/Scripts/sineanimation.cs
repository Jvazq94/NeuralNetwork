using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sineanimation : MonoBehaviour {
    Vector3 originalPosition, movementSin;
    public GameObject parent;
	// Use this for initialization
	void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        movementSin = new Vector2
            (
                Mathf.Sin(Time.time) * 0.1f,
                Mathf.Sin(Time.time*10) * 0.05f
            );
        transform.position = movementSin + parent.transform.position;
    }
}
