using UnityEngine;
using System.Collections;

public class LerpToObj : MonoBehaviour {
    public GameObject target;
    public float moveSpeed;
    bool go = false;
	public bool slerp;
	public bool altSlerp = false;
	private bool halfWayThere = false;
	public float journeyTime = 1;
	public int rect = 0;
	Vector3 half;
	Vector3 start;
	Vector3 targetPos;
	float startTime;
	bool notStarted = false;
	float completed;
	float other;
	// Use this for initialization
	void Start () 
	{
		half = (transform.position + target.transform.position) * .5f;
		start = transform.position;
		targetPos = target.transform.position;
		//Debug.Log (half);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Vector3.Distance(transform.position,target.transform.position)<1)
		{
			go = false;
		}
		if(Input.GetButtonDown("Jump"))
        {
            go = !go;
			if(notStarted)
			{
				startTime = Time.time;
				//completed = journeyTime;
			}
        }
        if (go)
        {
			other+=Time.deltaTime;
			//Debug.Log ("independent time: "+other);
			if(slerp)
			{
				if(altSlerp)
				{
					completed += Time.deltaTime;
					transform.position = Vector3.Slerp (start,targetPos,completed/journeyTime);
				}
				else
				{
					half -= Vector3.up;
					Vector3 startRel = start - half;
					Vector3 targetRel = targetPos - half;
					completed =(Time.time - startTime)/journeyTime;
					transform.position = Vector3.Slerp (startRel, targetRel,completed);
					transform.position+=half;
					//Debug.Log ("percent complete: " + completed*100 +"%");
				}
			}
			else
			{
				if(!altSlerp)
				{
					transform.position = Vector3.Lerp(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
				}
				else
				{
					if(!halfWayThere)
					{
						transform.position = Vector3.Lerp(transform.position,half, moveSpeed*2 * Time.deltaTime);
						if(Vector3.Distance(transform.position, half)<1)
							halfWayThere = true;
					}
					else
					{
						transform.position = Vector3.Lerp(transform.position, target.transform.position, moveSpeed*2 * Time.deltaTime);
					}
				}
	           // Debug.Log("lerping at " + moveSpeed + " towards " + target.transform.position);
			}
        }

	}
	void OnGUI()
	{
		Rect r = new Rect (rect * 100, 50, 100, 50);
		GUI.Label (r, other.ToString());
	}
}
