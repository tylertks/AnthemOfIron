using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetManager : NetworkManager 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void OnGUI()
	{

	}
	void StartHost()
	{
		NetworkManager.singleton.StartHost ();
	}
}
