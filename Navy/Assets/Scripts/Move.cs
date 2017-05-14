using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CharacterController))]

public class Move : MonoBehaviour {
    [SerializeField]private Vector3 position, velocity, acceleration, direction, desired = new Vector3(0,0,0);
    [SerializeField] private float speed, maxspeed, rotation, slow  = 0;
    private Rigidbody rigid;
    [SerializeField]
    private bool upkey, leftkey, rightkey, firekey;
    // Use this for initialization
    void Start() {
        rigid = gameObject.GetComponent<Rigidbody>();
        position = new Vector3(0, 0.75f, 0);
        direction = rigid.transform.eulerAngles;
        direction = transform.forward;
        gm = GameObject.Find("GameManagerGO");
        acceleration = new Vector3(0, 0, 0);
        velocity = transform.forward;
        charControl = GetComponent<CharacterController>();
        //finds bounding box based of the size of the flowfield with a safezone
        boundingbox = (gm.GetComponent<FlowField>().size / 2 - safezone);
        speed = 2;
    }
	private void input()
    {
        //upkey or forward input
        if(Input.GetKeyDown(KeyCode.W) || Input.GetButtonDown("upkey"))
        {
            upkey = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            upkey = false;

        }

        //leftkey or turn left input
        if (Input.GetKeyDown(KeyCode.A) || 0>Input.GetAxis("horizontal"))
        {
            leftkey = true;
        }
        if(Input.GetKeyUp(KeyCode.A))
        {
            leftkey = false;
        }

        //rightkey or turn right input
        if (Input.GetKeyDown(KeyCode.D) || 0<Input.GetAxis("horizontal"))
        {
            rightkey = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            rightkey = false;
        }

        //firekey or fire input
        if (Input.GetKeyDown("space")||Input.GetButtonDown("firekey"))
        {
            firekey = true;
        }
        else if (Input.GetKeyUp("space"))
        {
            firekey = false;
        }
    }
    // Update is called once per frame
    void Update() {
        input();
        //if the left key is pressed it rotates counterclockwise
        if (leftkey)
        {
            direction = new Vector3(0,transform.eulerAngles.y - Time.deltaTime* 45,0);
        }
        //if the right key is pressed it rotates clockwise
        if (rightkey)
        {
            direction = new Vector3(0, transform.eulerAngles.y + Time.deltaTime * 45, 0);
        }
        //if the upkey is pressed velocity increases
        if (upkey)
        {
            acceleration = new Vector3(Mathf.Sin(direction.y * Mathf.PI / 180), 0, Mathf.Cos(direction.y * Mathf.PI / 180));
            velocity = velocity + acceleration * Time.deltaTime;
            velocity.y = 0; //keeping us on same plane
                            //limit vel to max speed
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            //move the character based on velocity
            this.GetComponent<CharacterController>().Move(velocity * Time.deltaTime * speed);
            //reset acceleration to 0
            acceleration = Vector3.zero;

            transform.forward = velocity.normalized;
        }
        else
        {
            if(velocity.magnitude>0.5)
            {
                velocity = new Vector3(0,0,0);
            }
            else
            {
                velocity = velocity * 0.8f;
            }
        }
        transform.eulerAngles = direction;

        
    }



    //save this for later to save a little power
    private Vector3 sum = new Vector3(0, 0, 0);
    int count = 0;
    float d = 0f;

    public GameObject gm;



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

    protected void Applyforce(Vector3 steeringForce)
    {
        acceleration += steeringForce / mass;
    }





    /// <summary>
    /// if the vehicle goes out of bounds it seeks the center
    /// </summary>
    /// <returns>The in bounds.</returns>
    public Vector3 StayInBounds()
    {

        if (this.transform.position.x > boundingbox)
        {
            desired = new Vector3(0, 0, 0) - transform.position;
            desired = desired.normalized * maxSpeed;
            desired -= velocity;
            desired.y = 0;
            return desired;
        }
        else if (this.transform.position.z > boundingbox)
        {
            desired = new Vector3(0, 0, 0) - transform.position;
            desired = desired.normalized * maxSpeed;
            desired -= velocity;
            desired.y = 0;
            return desired;
        }

        if (this.transform.position.z < -boundingbox)
        {
            desired = new Vector3(0, 0, 0) - transform.position;
            desired = desired.normalized * maxSpeed;
            desired -= velocity;
            desired.y = 0;
            return desired;
        }
        else if (this.transform.position.x < -boundingbox)
        {
            desired = new Vector3(0, 0, 0) - transform.position;
            desired = desired.normalized * maxSpeed;
            desired -= velocity;
            desired.y = 0;
            return desired;
        }

        return new Vector3(0, 0, 0);
    }
}


