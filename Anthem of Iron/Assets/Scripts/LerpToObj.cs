using UnityEngine;
using System.Collections;

public class LerpToObj : MonoBehaviour {
    public GameObject target;
    public float moveSpeed;
    bool go = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetButtonDown("Jump"))
        {
            go = !go;
        }
        if (go)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
           // Debug.Log("lerping at " + moveSpeed + " towards " + target.transform.position);
        }
	}
}
