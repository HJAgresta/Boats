using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour {
    Vector2 oldMouse;
    Vector2 curMouse;
	// Use this for initialization
	void Start () {
        oldMouse = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
        curMouse = Input.mousePosition;

        gameObject.transform.Rotate(new Vector3( 0, oldMouse.x-curMouse.x,0),Space.World);
        gameObject.GetComponentInChildren<Transform>().rotation.SetLookRotation(gameObject.transform.rotation.eulerAngles,new Vector3(0,1,0));
        /*
        x += Input.GetAxis("Mouse X") * xSpeed;
        y -= Input.GetAxis("Mouse Y") * ySpeed;
        

        transform.RotateAround(target.position, transform.up, x);
        transform.RotateAround(target.position, transform.right, y);
        */
        oldMouse = Input.mousePosition;
    }
}
