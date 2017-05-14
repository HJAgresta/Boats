using UnityEngine;
using System.Collections;

public class FlowField : MonoBehaviour {
	//width and height of the flowfeild, should be devided by the scale

	//this changes the size of the area the ships are allowed in, it is the width and the height, despite its name
	static int width =250;

	private Vector3[,] field = new Vector3[width,width];

	void Start () {
		//loops through and puts in values for the flow field
		float xoff = 0;
		for (int i = 0; i < width; i++) 
		{
			float yoff = 0;
			for (int j = 0; j < width; j++)
			{
				//perlin noise returns a value from 0 to 1 so multiply by
				float theta = Mathf.PerlinNoise(xoff,yoff)*360;
				field[i,j] = new Vector3(Mathf.Cos(theta),0,Mathf.Sin(theta));
				field[i,j].Normalize();
				//go to next y value in 2d perlin nose map
				yoff+=.01f;
			}
			//go to next x value in the 2d perlin noise map
			xoff+=.01f;
		}
	}

	public Vector3 getField(Vector3 position)
	{
		//reuturns value from array field using x and z position
		return field[(int)(position.x+width/2),(int)(position.z+width/2)];
	}

	public int size 
	{
		get
		{
			return width;
		}
	}


}
