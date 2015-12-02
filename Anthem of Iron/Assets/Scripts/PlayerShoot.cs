using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour 
{
	public string explosiveType;
	public string energyType;
	float heatLevel;
	float heatGauge = 100;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	void EnergyFire()
	{

	}
	void ExplosiveFire()
	{

	}
}
[System.Serializable]
public class Weapon
{
	public string name;
	public float range;
	public float rateOfFire;
	float fireTime;
	public bool energy;
	public bool spinUp;
	public float spinTime;
	float spin;
	public bool heat;

	public GameObject bulletObject;
	public GameObject muzzleFlash;
	public GameObject hitObject;

	public GameObject shotSound;

	public GameObject weaponMesh;

	public void Fire(Vector3 dir)
	{
		if (fireTime > rateOfFire) {
			if(energy)
			{
				EnergyFire(dir);
			}
			else
			{
				ExplosiveFire(dir);
			}
		}
	}
	void EnergyFire(Vector3 dir)
	{

	}
	void ExplosiveFire(Vector3 dir)
	{

	}
	public void Start()
	{

	}
}