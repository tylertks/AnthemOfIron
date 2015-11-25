using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Sight : NetworkBehaviour {
	private float visionDistance = 10;
	string team;
	string oTeam;
	public List<GameObject> visible = new List<GameObject>();
	ObjectVisibility[] objects = new ObjectVisibility[5]{null,null,null,null,null};
	bool blind = false;
	// Use this for initialization
	void Start () 
	{
		if (isLocalPlayer) 
		{
			team = this.tag;
			//set allied and enemy teams
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
		GameObject[] g = GameObject.FindGameObjectsWithTag (oTeam);
		for (int i = 0; i < 5; i++) 
		{
			objects[i].AssignObj(g[i]);
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void LineOfSight()
	{
		for (int i = 0; i < objects.Length; i++) 
		{
			//my position
			Vector3 a = transform.position;
			//enemy position
			Vector3 b = objects[i].GetObj().transform.position;
			//coords to look at the enemy
			Vector3 dir = Vector3.zero;
			//size of the enemy
			float r = objects[i].width;
			if(Vector3.Distance(a,b)<visionDistance)
			{
				for (int j = 0; j < 5; j++) 
				{
					//look at enemy position, plus 4 edges
					switch(j)
					{
					case 0:
						dir = new Vector3(b.x - a.x, 0, b.z - a.z).normalized;
						break;
					case 1:
						dir = new Vector3(b.x + r - a.x, 0, b.z +r - a.z).normalized;
						break;
					case 2:
						dir = new Vector3(b.x -r - a.x, 0 , b.z +r - a.z).normalized;
						break;
					case 3:
						dir = new Vector3(b.x -r - a.x, 0, b.z -r - a.z).normalized;
						break;
					case 4:
						dir = new Vector3(b.x +r - a.x , 0, b.z -r - a.z).normalized;
						break;
					default:
						Debug.Log ("error");
						break;
					}
					//if enemy is within line of sight, make visible
					if(Physics.Raycast(transform.position,dir,visionDistance))
					{
						if(!objects[i].IsVisible())
						{
							objects[i].ChangeVisiblity(true);
						}
						break;
					}
					//if not within line of sight, make invisible
					else
					{
						if( j >=4 && objects[i].IsVisible())
						{
							objects[i].ChangeVisiblity(false);
							break;
						}
					}
				}

			}
		}
	}
	//send visible enemies to teammates
	void SendVisibility()
	{
		if (!blind)
		{

		}
	}
	//make enemies visible based on teammate visibility
	void SyncVisibility()
	{
		if (!blind)
		{

		}
	}
}
[System.Serializable]
//class holding enemies and their visibility
public class ObjectVisibility
{
	//enemy object
	private GameObject obj;
	//is enemy visible or not
	private bool isVisible;
	//enemy model
	private MeshRenderer meshObj;
	//has the visibility changed
	bool changed= false;
	//enemy size
	public float width;

	//assign the enemy and related values
	public void AssignObj(GameObject other)
	{
		obj = other;
		meshObj = other.GetComponent<MeshRenderer> ();
		width = other.GetComponent<CharacterController> ().radius;
	}
	//sends enemy object to outside class
	public GameObject GetObj()
	{
		return obj;
	}
	//turn visibility on or off
	public void ChangeVisiblity(bool b)
	{
		isVisible = b;
		changed = true;
	}
	//sends visiblity information to outside class
	public bool IsVisible()
	{
		return isVisible;
	}
	//check the visibility
	public void Visiblity()
	{
		//has the visibility changed since last frame
		if (changed) 
		{
			//make visible
			if (isVisible) 
			{
				meshObj.enabled = true;
			} 
			//make invisible
			else 
			{
				meshObj.enabled = false;
			}
		}
	}
}
