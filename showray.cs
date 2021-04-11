using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showray : MonoBehaviour
{
  public float velocity = 3.0f;
  public float maxDist = 10000.0f;
  public bool showTrail = true;
  public GameObject hitDecal;

  private float _wallDisplacement = 0.001f;
  private RaycastHit hit;
  private Vector3 startPosition;
  private Vector3 endPosition;
  private Vector3 direction;
  private float distance;
  private float distTraveled;
  private float gravity = 9.81f;
  private float y = 0.0f;
  private float downward=0.0f;
  public static bool firstbug=false;
    // Start is called before the first frame update
    void Start()
    {
      startPosition = this.transform.position;//transform.position;
        distTraveled = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
      distTraveled=0.0f;
      startPosition = this.transform.position;
      y=0f;
      downward=-1*Time.fixedDeltaTime*gravity;


        while (distTraveled <= maxDist)
        {
          velocity=this.GetComponent<Rigidbody>().velocity.magnitude;
            direction = this.GetComponent<Rigidbody>().velocity.normalized;
            //direction = transform.TransformDirection(new Vector3(0f,0f,1f));
            distance = velocity * Time.fixedDeltaTime;
            downward-=gravity*Time.fixedDeltaTime;
            //Debug.Log(direction);
            if (Physics.Raycast(startPosition, direction, out hit, distance));
            {
              //break;
                /*if (hitDecal &&hit.transform.tag == "levelParts") {
                    Instantiate(hitDecal, hit.point + (hit.normal * _wallDisplacement), Quaternion.LookRotation(hit.normal));
                    Destroy (gameObject);
                }*/
            }
            y -= gravity * Time.fixedDeltaTime;
            endPosition = startPosition + direction * distance*Time.fixedDeltaTime+ new Vector3(0f,downward,0f)*Time.fixedDeltaTime;

            if (showTrail){
             Debug.DrawLine(startPosition, endPosition,Color.green,Time.fixedDeltaTime/2);
                //Gizmos.DrawLine(startPosition, endPosition, Color.green, 3.0f);
              }
            distTraveled += Vector3.Distance(startPosition, endPosition);
            startPosition = endPosition;
            //if(!firstbug){Debug.Log(endPosition);}
        }
        firstbug=true;

    }
}
