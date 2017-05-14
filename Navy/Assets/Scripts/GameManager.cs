
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject red;
	public GameObject blue;

	public GameObject[] boats=new GameObject[60];
	void Start () {
		bool color=true;
		int size = GetComponent<FlowField> ().size/2;
		for  (int i = 0; i<boats.Length; i++) 
		{
			if(color)
			{
				Vector3 pos = new Vector3(Random.Range(-size,size),0.8f,Random.Range(-size,size));
				Quaternion rot = Quaternion.Euler(0,Random.Range(0,180),0);
				boats[i] = (GameObject)(Instantiate(blue,pos,rot));
				color =false;
			}
			else
			{
					Vector3 pos = new Vector3(Random.Range(-size,size),0.8f,Random.Range(-size,size));
				Quaternion rot = Quaternion.Euler(0,Random.Range(0,180),0);
					boats[i]=(GameObject)(Instantiate(red,pos,rot));
				color = true;
			}
		}
	}
	
	
	void Update () {

	}
}
