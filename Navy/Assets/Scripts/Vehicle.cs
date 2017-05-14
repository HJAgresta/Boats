using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

	protected Vector3 acceleration;
	protected Vector3 velocity;
	protected Vector3 desired;


	//save this for later to save a little power
	private Vector3 sum = new Vector3(0,0,0);
	int count = 0;
	float d = 0f;

	public GameObject gm;

	public Vector3 Velocity
	{
		get{return velocity;}
	}


	//public for changing in inspector, movemnt behaviours
	public float maxSpeed;
	public float maxForce;
	public float mass;
	public float radius;
	public float desiredseparation = 10;
	public float neighbordist = 1;
	private float boundingbox;
	public float safezone;

	CharacterController charControl;

	abstract protected void CalcSteeringForces();

	protected void Applyforce (Vector3 steeringForce)
	{
		acceleration += steeringForce / mass;
	}


	virtual public void Start(){
		gm = GameObject.Find ("GameManagerGO");
		acceleration = new Vector3 (0,0,0);
		velocity = transform.forward;
		charControl = GetComponent <CharacterController>();
		//finds bounding box based of the size of the flowfield with a safezone
		boundingbox = (gm.GetComponent<FlowField> ().size/2-safezone);
	}

	
	// Update is called once per frame
	virtual public void Update () {
		//calculate all necessary steering forces
		CalcSteeringForces();
		//add accel to vel
		velocity += acceleration * Time.deltaTime;
		velocity.y = 0; //keeping us on same plane
		//limit vel to max speed
		velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
		//move the character based on velocity
		charControl.Move(velocity * Time.deltaTime);
		//reset acceleration to 0
		acceleration = Vector3.zero;

		transform.forward = velocity.normalized;
	}

	/*protected Vector3 Seek (Vector3 targetPosition)
	{
		desired = targetPosition-transform.position;
		desired = desired.normalized*maxSpeed;
		desired -= velocity;
		desired.y = 0;
		return desired;
	}*/


	protected Vector3 AvoidObstacle(GameObject ob, float safe) {
		//reset desired velocity
		desired = Vector3.zero;
		//get radius from obstacle's script
		float obRad = ob.GetComponent<Boat>().radius;
		//get vector from vehicle to obstacle
		Vector3 vecToCenter = ob.transform.position - transform.position;
		//zero-out y component (only necessary when working on X-Z plane)
		vecToCenter.y = 0;
		//if object is out of my safe zone, ignore it
		if(vecToCenter.magnitude > safe)
		{
			return Vector3.zero;
		}
		//if object is behind me, ignore it
		if(Vector3.Dot(vecToCenter, transform.forward) < 0)
		{
			return Vector3.zero;
		}
		//if object is not in my forward path, ignore it
		if(Mathf.Abs(Vector3.Dot(vecToCenter, transform.right)) > obRad + radius)
		{
			return Vector3.zero;
		}
		
		//if we get this far, we will collide with an obstacle!
		//object on left, steer right
		if (Vector3.Dot(vecToCenter, transform.right) < 0) 
		{
			desired = transform.right;
			//debug line to see if the dude is avoiding to the right
			Debug.DrawLine(transform.position, ob.transform.position, Color.red);
		}
		else 
		{
			desired = transform.right;
			//debug line to see if the dude is avoiding to the left
			Debug.DrawLine(transform.position, ob.transform.position, Color.green);
		}

		return desired;
	}

	/// <summary>
	/// Avoids the obstacles with velocity
	/// </summary>
	/// <returns>The obstacle.</returns>
	/// <param name="ob">Ob.</param>
	/// <param name="safe">Safe.</param>
	protected Vector3 AvoidCollision(GameObject ob, float safe) {
		//reset desired velocity
		desired = Vector3.zero;
		//get radius from obstacle's script
		float obRad = ob.GetComponent<Vehicle>().radius;

		Vector3 obPosition = ob.GetComponent<Vehicle>().transform.position + ob.GetComponent<Vehicle>().velocity;

		Vector3 futPosition = this.transform.position + this.GetComponent<Vehicle>().velocity;

		//get vector from vehicle to obstacle
		Vector3 vecToCenter = obPosition - futPosition;
		//zero-out y component (only necessary when working on X-Z plane)
		vecToCenter.y = 0;
		//if object is out of my safe zone, ignore it
		if(vecToCenter.magnitude > safe)
		{
			return Vector3.zero;
		}
		//if object is behind me, ignore it
		if(Vector3.Dot(vecToCenter, transform.forward) < 0)
		{
			return Vector3.zero;
		}
		//if object is not in my forward path, ignore it
		if(Mathf.Abs(Vector3.Dot(vecToCenter, transform.right)) > obRad + radius)
		{
			return Vector3.zero;
		}
		
		//if we get this far, we will collide with an obstacle!
		//object on left, steer right
		if (Vector3.Dot(vecToCenter, transform.right) < 0) 
		{
			desired = transform.right;
			//debug line to see if the dude is avoiding to the right
			Debug.DrawLine(futPosition, obPosition, Color.red);
		}
		else 
		{
			desired = transform.right;
			//debug line to see if the dude is avoiding to the left
			Debug.DrawLine(futPosition, obPosition, Color.green);
		}
		
		return desired;
	}

	/// <summary>
	/// takes an array of vehicles and returns a vector pointing away from the neighbors
	/// </summary>
	/// <param name="vehicles">Vehicles.</param>
	protected Vector3 Separation(GameObject[] vehicles)
	{
		//summ of vectors pointing away from ones too close
		sum = new Vector3(0,0,0);
		//howmany are too close
		count = 0;
		//check all vehcles
		for(int i = 0; i < vehicles.Length; i++)
		{
		//finds distance between vehicsel
		d = Vector3.Distance(this.transform.position, vehicles[i].transform.position);
		//if its too close
		if ((d > 0) && (d < desiredseparation)) 
			{
				//find vector away form this vehicle
				Vector3 diff = this.transform.position-vehicles[i].transform.position;
				diff.Normalize();
				//add away vector to the sum, increments count
				sum = sum+diff;
				count++;
			}
		}

		if(count>0)
		{
			return sum/count;
		}
		else
		{
			return sum;
		}
	}

	/// <summary>
	/// Aligment takes an array of gameobjects and returns the sum of the closer ones velocities
	/// </summary>
	/// <param name="vehicles">Vehicles.</param>
	public Vector3 Alignment(GameObject[] vehicles)
	{
		//summ of velocityies of velicels too close
		sum = new Vector3(0,0,0);
		//check all vehcles
		for(int i = 0; i < vehicles.Length; i++)
		{
			//if the vehicle is of the same group
			if(tag=="Blue" &&vehicles[i].tag=="Blue"||tag=="Red" &&vehicles[i].tag=="Red")
			{
				//finds distance between vehicsel
				d = Vector3.Distance(this.transform.position, vehicles[i].transform.position);
				//if its too close
				if ((d > 0) && (d < neighbordist)) 
				{
					//add velocity vector to the sum, increments count
					sum = sum+vehicles[i].GetComponent<Vehicle>().velocity;
				}
			}
		}
		return sum;
	}


	/// <summary>
	/// Takes an array of vehicles, averages the velocity of neighboring vehicles
	/// </summary>
	/// <param name="vehicles">Vehicles.</param>
	public Vector3 Cohesion(GameObject[] vehicles)
	{
		//summ of vectors pointing away from ones too close
		sum = new Vector3(0,0,0);
		//howmany are too close
		count = 0;
		//check all vehcles
		for(int i = 0; i < vehicles.Length; i++)
		{
			//if the vehicle is of the same group
			if(tag=="Blue" &&vehicles[i].tag=="Blue"||tag=="Red" &&vehicles[i].tag=="Red")
			{
				//add vehicles velocity vector to the sum, increments count
				sum = sum+GetComponent<Vehicle>().velocity;
				count++;
			}
		}
		
		if(count>0)
		{
			return sum/count;
		}
		else
		{
			return sum;
		}
	}

	/// <summary>
	/// if the vehicle goes out of bounds it seeks the center
	/// </summary>
	/// <returns>The in bounds.</returns>
	public Vector3 StayInBounds()
	{

		if (this.transform.position.x > boundingbox) 
		{
			desired = new Vector3(0,0,0)-transform.position;
			desired = desired.normalized*maxSpeed;
			desired -= velocity;
			desired.y = 0;
			return desired;
		} 
		else if (this.transform.position.z > boundingbox) {
			desired = new Vector3(0,0,0)-transform.position;
			desired = desired.normalized*maxSpeed;
			desired -= velocity;
			desired.y = 0;
			return desired;
		}

		if (this.transform.position.z < -boundingbox) {
			desired = new Vector3 (0, 0, 0) - transform.position;
			desired = desired.normalized * maxSpeed;
			desired -= velocity;
			desired.y = 0;
			return desired;
		} else if (this.transform.position.x < -boundingbox) {
			desired = new Vector3(0,0,0)-transform.position;
			desired = desired.normalized*maxSpeed;
			desired -= velocity;
			desired.y = 0;
			return desired;
		}
		
		return new Vector3 (0, 0, 0);
	}
}
