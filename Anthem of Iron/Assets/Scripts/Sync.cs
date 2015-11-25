using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Sync : NetworkBehaviour
{
    [SerializeField]
    Transform myTrans;

    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;

    [SerializeField]
    private List<Vector3> syncPosList = new List<Vector3>();
    [SerializeField]
    bool useHistory = false;

    float rate = 5f;
    float normalLerpRate = 15f;
    float fasterLerpRate = 50f;

	[SyncVar]
    float tRotation;
	[SyncVar]
	float bRotation;

    Vector3 prevPos;
    float threshold = .5f;
	PlayerMove p;
	// Use this for initialization
	void Start ()
    {
        prevPos = myTrans.position;
		p = this.gameObject.GetComponent<PlayerMove> ();
	}
	
	// Send movement and rotation information over the network
	void FixedUpdate ()
    {
		TransmitPosition ();
		TransmitRotation ();
	}
    void Update()
    {
        if (useHistory && !isLocalPlayer)
        {
            //move networked player based on a history of positions
			HistoricalLerp();
        }
        else
        {
            //move networked player to most recent position
			LerpPos();
        }
		Rotate ();
    }
    void LerpPos()
    {
        if(!isLocalPlayer)
        {
            Vector3 target = new Vector3((syncPos.x - myTrans.position.x), 0, (syncPos.z - myTrans.position.z))*2;
            target = myTrans.position + target;
            myTrans.position = Vector3.Lerp(myTrans.position, target, Time.deltaTime*rate);            
        }
    }
    [Command]
    void CmdSendPos(Vector3 pos)		//send position over the network
    {
        syncPos = pos;
		SyncPositionValues (syncPos);
    }
	[Command]
	void CmdSendRot(float t, float b)	//send rotation over the network
	{
		tRotation = t;
		bRotation = b;
	}
    [ClientCallback]
    void TransmitPosition()
    {
        //checks if the player is local, then sends position information if player has moved far enough
		if(isLocalPlayer && Vector3.Distance(myTrans.position,prevPos)>threshold)
        {
			CmdSendPos(myTrans.position);
            prevPos = myTrans.position;
        }        
    }
	[ClientCallback]
	void TransmitRotation()
	{
		//checks if player is local, then sends rotation information
		if(isLocalPlayer)
		{
			CmdSendRot (p.tRotation, p.bRotation);
		}
	}
    [ClientCallback]
    void SyncPositionValues(Vector3 latest)
    {
        //adds position to the history of positions for accurate movement
		if (!isLocalPlayer && Vector3.Distance(latest,prevPos)>threshold)
        {
            //syncPos = latest;
            syncPosList.Add(latest);
        }
    }

    void HistoricalLerp()
    {
		//checks to see if a new position is available
		if(syncPosList.Count >0)
        {
            //myTrans.position = Vector3.Lerp(myTrans.position, syncPosList[0], Time.deltaTime * rate);
            
            if (Vector3.Distance(myTrans.position, syncPosList[0]) < threshold)
            {
                syncPosList.RemoveAt(0); //removes position from the list once reached
            }
            
            if (syncPosList.Count > 5)  //moves player at a faster rate if too far behind, useful for lag spikes
            {
                rate = fasterLerpRate;
                myTrans.position = Vector3.Lerp(myTrans.position, syncPosList[0], Time.deltaTime * rate);
            }
            else 						//moves player to next position
            {
                rate = normalLerpRate;
                Vector3 target = new Vector3((syncPosList[0].x - myTrans.position.x), 0, (syncPosList[0].z - myTrans.position.z)) * 2;
                target = myTrans.position + target;
                myTrans.position = Vector3.Lerp(myTrans.position, target, Time.deltaTime * rate);
            }
        }
    }
	void Rotate()
	{
		//tells the networked player to rotate
		if(!isLocalPlayer)
		{
			p.NetRotate(tRotation,bRotation);
		}
	}
}
