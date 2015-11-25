using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerMove : NetworkBehaviour{

    private Vector3 move;
    [SerializeField]
    private float speed = 15;
    [SerializeField]
    Camera cam;
    [SerializeField]
    AudioListener a;
    [SerializeField]
    float size = .5f;
	[SerializeField]
	GameObject top;
	[SerializeField]
	GameObject bottom;
	[SerializeField]
	public float tRotation;
	[SerializeField]
	public float bRotation;
    private float maxCamDist = 7;
    private float visionDistance = 10;
	string team;
	string oTeam;
	public List<GameObject> visible = new List<GameObject>();
	// Use this for initialization
	void Start ()
    {
        //if networked player, turn off camera and audio listener
		if(!isLocalPlayer)
        {
            this.GetComponent<PlayerMove>().enabled = false;
            //Debug.Log("not me");
            cam.enabled = false;
            a.enabled = false;
        }
		/*if local player, set the following:
		 * lock cursor
		 * mech size
		 * rotation
		 * ally and enemy team*/
        if(isLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Confined;
            this.GetComponent<CharacterController>().radius = size;
            this.GetComponent<CapsuleCollider>().radius = size;
			tRotation = 0;
			bRotation = 0;
			team = this.tag;
			if(team == "Team1")
			{
				oTeam = "Team2";
			}
			else if(team == "Team2")
			{
				oTeam = "Team1";
			}
			else
			{
				Debug.Log ("no team");
			}
        }
		
	}

	void FixedUpdate ()
    {
        //move the player at a set update rate
		Move();        
	}
	void Update()
	{
		//move the camera and rotate the player once every drawn frame
		CameraMove();
		Rotate ();
	}
    void CameraMove()
    {
        //get mouse position
		Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
        //set the camera distance to 35% of the distance from the mouse to the player
		float dist = Vector3.Distance(transform.position, mousePos);
        dist *= .35f;
        Vector3 newPos = Vector3.Normalize(new Vector3(mousePos.x - transform.position.x, cam.transform.position.y * 2, mousePos.z - transform.position.z))*dist;
        cam.transform.position = new Vector3(transform.position.x + newPos.x, cam.transform.position.y, transform.position.z + newPos.z);
        //sets the maximum camera distance from the player
		if(Vector3.Distance(transform.position,cam.transform.position)>maxCamDist)
        {
            Vector3 v = Vector3.Normalize(new Vector3(cam.transform.position.x - transform.position.x, cam.transform.position.y, cam.transform.position.z - transform.position.z))*maxCamDist;
            cam.transform.position = new Vector3(v.x + transform.position.x, cam.transform.position.y, v.z + transform.position.z);
        }
    }
	//assign movement based on key presses
    void Move()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (move.magnitude > 1)
        {
            move = move.normalized;
        }
		//check collisions
        RayCast();
        transform.position += move * Time.deltaTime * speed;
    }

    void RayCast()
    {
        //returns the information from a cast hitting an object
		RaycastHit hit;
		//my position
        Vector3 pos = transform.position;
        //if an object is too close(touching), the player cannot move in that objects direction
        if(Physics.SphereCast(pos,size,Vector3.forward,out hit, 3))
        {
            if (move.z > 0 && hit.distance < size / 2)
            {
                move = new Vector3(move.x, move.y, 0);
            }
        }
        if (Physics.SphereCast(pos, size, Vector3.back, out hit, 3))
        {
            if(move.z < 0 && hit.distance < size / 2)
            {
                move = new Vector3(move.x, move.y, 0);
            }
        }
        if (Physics.SphereCast(pos, size, Vector3.right, out hit, 3))
        {
            if(move.x > 0 && hit.distance < size / 2)
            {
                move = new Vector3(0, move.y, move.z);
            }
        }
        if (Physics.SphereCast(pos, size, Vector3.left, out hit, 3))
        {
            if(move.x < 0 && hit.distance < size / 2)
            {
                move = new Vector3(0, move.y, move.z);
            }
        }
    }
	void Rotate()
	{
		if (isLocalPlayer) 
		{
			//find the mouse and make the top of the mech face that direction
			Vector3 mousePos = cam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
			top.transform.LookAt (mousePos);
			tRotation = top.transform.rotation.eulerAngles.y;

			//rotate the bottom of the mech based on movement direction (8-way movement)
			if (move.x > 0) {
				if (move.z > 0) {
					bRotation = 45;
				} else if (move.z < 0) {
					bRotation = 135;
				} else if (move.z == 0) {
					bRotation = 90;
				}
			} else if (move.x < 0) {
				if (move.z > 0) {
					bRotation = -45;
				} else if (move.z < 0) {
					bRotation = -135;
				} else if (move.z == 0) {
					bRotation = -90;
				}
			} else if (move.x == 0) {
				if (move.z > 0) {
					bRotation = 0;
				} else if (move.z < 0) {
					bRotation = 180;
				} else if (move.z == 0) {
				
				}
			}
			Quaternion q = new Quaternion (0, 0, 0, 0);
			q.eulerAngles = new Vector3 (0, bRotation, 0);
			bottom.transform.rotation = q;
		}
	}
	public void NetRotate(float t, float b)
	{
		//rotate the top and bottom for networked players
		if(!isLocalPlayer)
		{
			Quaternion q1 = new Quaternion (0, 0, 0, 0);
			q1.eulerAngles = new Vector3 (0, b, 0);
			bottom.transform.rotation = q1;
			Quaternion q2 = new Quaternion (0, 0, 0, 0);
			q2.eulerAngles = new Vector3 (0, t, 0);
			top.transform.rotation = q2;
			//Debug.Log ("network rotate: " + t + ", " +b);
		}
	}

}
