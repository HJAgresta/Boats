using UnityEngine;
using System.Collections;

public class Boat : Vehicle {

	//public GameObject seekerTarget;

	public float safeDistance=10.0f;

	private Vector3 force;

	private GameObject[] boats;

	public GameObject blueshot;
	public GameObject redshot;
	private GameObject shot;


	//weighting
	//public float seekWeight=75f;
	public float avoidWeight = 75f;
	public float separateWeight = 75f;
	public float alignmentWeight = 75f;
	public float cohesionWeight=75f;
	public float stayInBoundsWeight=75f;
	public float flowFieldWeight=75f;

	//used for shooting
	public float shotDistance = 75f;
	public float angleLeeway= 60f;
	public int reloadTime = 20;
	private int reload =0;

	//if its active
	public bool active = true;

	protected override void CalcSteeringForces ()
	{
		if (active) {
			//reset force to zero 
			force = Vector3.zero;
			//get seek force
			//add seek force to steering force
	
			//why have a million get components when you can have just one
			boats = gm.GetComponent<GameManager> ().boats;

			//runs separate, allignment and adds it to the force
			force += Separation (boats) * separateWeight;
			force += Alignment (boats) * alignmentWeight;
			force += Cohesion (boats) * cohesionWeight;
			force += StayInBounds () * stayInBoundsWeight;
			//flow field force
			force += gm.GetComponent<FlowField> ().getField (this.transform.position) * flowFieldWeight;
			//avoidance forces
			for (int i = 0; i < boats.Length; i++) {
				force += AvoidObstacle (boats [i], GetComponent<Boat> ().radius) * avoidWeight;
				force += AvoidCollision (boats [i], GetComponent<Boat> ().radius) * avoidWeight;
			}

			force = Vector3.ClampMagnitude (force, maxForce);
			//limit seekers steering force


			Applyforce (force);
			//applied the steering force to the vehicles acceleration
		}

	}
	// Call Inherited Start and then do our own
	override public void Start () {

		base.Start();

		//initialize
		force = Vector3.zero;
	}

	//call inherited update and do our own
	override public void Update () {

		base.Update();

		//detect if boats are in the area and fire
		reload++;

		if (reloadTime < reload) 
		{
			for (int i = 0; i < boats.Length; i++) 
			{
				//if the vehicle is of the opposite group
				if (tag == "Blue" && boats [i].tag == "Red" || tag == "Red" && boats [i].tag == "Blue") 
				{
					if (Vector3.Distance (transform.position, boats [i].transform.position) < shotDistance) 
					{
						if (tag == "Blue") 
						{
							shot = (GameObject)Instantiate (blueshot, transform.position, transform.rotation);
							shot.GetComponent<Shot> ().Spawn (boats [i].GetComponent<Boat>(), true);
							reload=0;
						} 
						else if (tag == "Red") 
						{
							shot = (GameObject)Instantiate (redshot, transform.position, transform.rotation);
							shot.GetComponent<Shot> ().Spawn (boats [i].GetComponent<Boat>(), true);
							reload=0;
						}
					}

				}
			}
		}
	}
}