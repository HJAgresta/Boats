using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {
	//all of this is unused, couldnt get it to quite work


	//holds velocity of the shot
	private Vector3 velocity;
	public float speed = 10;
	public float life=30;
	public float time;

	// Use this for initialization
	void Start () 
	{

	}

	/// <summary>
	/// spawns the shot
	/// </summary>
	/// <param name="boat">boat it comes from</param>
	/// <param name="right">If set to <c>true</c> shoots to the right.</param>
	public void Spawn (Boat otherBoat, bool right)
	{
		//sends shot at the other ship

		velocity = (otherBoat.transform.position-transform.position).normalized*speed;
	}
	
	// Update is called once per frame
	void Update () {
		time++;
		//limit vel to max speed
		transform.position = transform.position + velocity;
		if (life < time)
		{
			Destroy (gameObject);
		}
			

	}

	void OnCollisionEnter(Collision col)
	{
		if (tag == "Blue" && col.gameObject.tag == "Red" || tag == "Red" && col.gameObject.tag == "Blue") 
		{
			Debug.Log ("hit");
			Destroy(col.gameObject);
			Destroy (gameObject);
		}

	}
}
