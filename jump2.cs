using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static System.Collections.IEnumerable;
using System.Linq;
using UnityEngine.UI;


public class jump2 : MonoBehaviour
{

    public float speed;
    public float distrot;
    public float timeforjump = 3;
    public float jump_power;
    public float fieldofview;
    public float speed_to_angle;
    public float DIFF;
    public float x = 0;
    public float z = 0;
    public float friction = 0.8f;
    public float prex = 0;
    public float prez = 0;
    public float robturn;
    public float robgo;
    public float robjump;
    private float origfeet = 0;
    public float startjumppos;
    public float startjumpdif;
    Vignette vignette = null;
    public bool hitting = true;
    public Vector3 jumpdir;
    public float gravity = 9.81f;
    public bool flying = false;
    private static Vector3 prevel;
    private static bool firstbug = false;
    public float basepos;


    public float velocity = 3.0f;
    public float maxDist = 10000.0f;
    public bool showTrail = true;
    public GameObject hitDecal;
    public bool preparejump = false;
    private float addjump = 0;
    private int i;
    private float difangcur;
    private float predifang = 0;

    private float _wallDisplacement = 0.001f;
    private RaycastHit hit;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 direction;
    private float distance;
    private float distTraveled;
    private float y = 0.0f;
    //public float headjudge;
    private Vector3 downward = new Vector3(0f, 0f, 0f);
    private Vector3 vecbase;
    private Vector3 vecrot;
    public GameObject predp;
    private float time = 0f;
    public float threshjump;
    private float origturn;
    public GameObject obj;
    Vector3 savevel = new Vector3(0f, 0f, 0f);
    private float origx = 0;
    private float origz = 0;
    public GameObject head;
    public GameObject rhand;
    public GameObject lhand;
    public GameObject eye;
    private Quaternion origheadrot;
    public float offsetx;
    public float offsetz;
    public int precount = 0;
    private List<float> jumplist = new List<float>();
    private List<float> anglelist = new List<float>();
    private List<float> rawxlist = new List<float>();
    private List<float> rawzlist = new List<float>();
    public int timerot = 20;
    public int timepos = 20;
    public float jumpangle;


    private float a, b = 0;//delete it later,after finishing the jump test
    public Text text;


    public GameObject feet;
    public float origheadpos;
    [SerializeField] GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("postvolume").GetComponent<PostProcessVolume>().profile.TryGetSettings(out vignette);

}

    // Update is called once per frame
    void Update()
    {


        if ((Time.realtimeSinceStartup > 20f) && !firstbug)
        {
            if((Time.realtimeSinceStartup < 22.5f))
            { 
            firstbug = true;
            offsetx = (rhand.transform.position.x + lhand.transform.position.x) / 2 - head.transform.position.x;
            offsetz = (rhand.transform.position.z + lhand.transform.position.z) / 2 - head.transform.position.z;
            origfeet = head.transform.position.y - (rhand.transform.position.y + lhand.transform.position.y) / 2;
            //origheadrot = (head.transform.rotation);
            //text.text = "";
            panel.SetActive(false);
            for (i = 0; i < timerot; i++) { anglelist.Add(0); }
            for (i = 0; i < timepos; i++)
            {
                rawxlist.Add(0);
                rawzlist.Add(0);
            }
            for (i = 0; i < (int)(timeforjump / Time.fixedDeltaTime); i++) { jumplist.Add(origfeet); }
            vecrot = (new Vector3(origx, 0, origz));
            vecbase = head.GetComponent<Transform>().forward;
            vecbase.y = 0;
            origheadpos = head.transform.position.y - feet.transform.position.y;
            Quaternion q1 = Quaternion.FromToRotation(vecrot, vecbase);
            origturn = q1.eulerAngles.y;
            if (origturn > 180) { origturn -= 360; }
        }
        }
        else if (Time.realtimeSinceStartup > 18f && !firstbug)
        {
            text.text = "制限時間は９００秒です。";
        }
        else if (Time.realtimeSinceStartup > 15.5f&&!firstbug)
        {
            text.text = "この街で一番高い建物の\n屋上へ向かってください";
        }
        else if (Time.realtimeSinceStartup > 10.5f && !firstbug)
        {
            text.text = "ひざを曲げてから立ち上がると、\n向いている方向にジャンプできます\n着地予測地点には黄色い目印が現れます";
        }
        else if (!firstbug)
        {
            text.text = "腕をおろしてください。";
            panel.SetActive(true);
            //it might be better to ask player put arms to forward direction a little so that it becomes easier to put arms at zero point
        }
        else
        {


            //prex=x;
            //prez=z;

            origx = (rhand.transform.position.x + lhand.transform.position.x) / 2 - head.transform.position.x - offsetx;
            origz = (rhand.transform.position.z + lhand.transform.position.z) / 2 - head.transform.position.z - offsetz;
            //Debug.Log(z);
            //Debug.Log(x);
            rawxlist.RemoveAt(0);
            rawxlist.Add(origx);
            rawzlist.RemoveAt(0);
            rawzlist.Add(origz);
            /*if(Mathf.Abs(origx)-robgo>0){x=origxx-Mathf.Sign(origx)*robgo;
              }else{x=0f;}
            if(Mathf.Abs(origz)-robgo>0){z=origz-Mathf.Sign(origz)*robgo;
              }else{z=0f;}*/
            float usex = 0;
            float usez = 0;
            for (i = 0; i < timepos; i++)
            {
                usex += rawxlist[i] * (i + 1) / timepos / (timepos + 1) * 2;
                usez += rawzlist[i] * (i + 1) / timepos / (timepos + 1) * 2;
            }
            if (Mathf.Abs(usex) - robgo > 0)
            {
                x = usex - Mathf.Sign(usex) * robgo;
            }
            else { x = 0f; }
            if (Mathf.Abs(usez) - robgo > 0)
            {
                z = usez - Mathf.Sign(usez) * robgo;
            }
            else { z = 0f; }



            /*
            float difsize=Mathf.Sqrt(x*x+z*z)
            if(difsize)>robgo){
            x=vel*x/difsize;
            x=vel*x/difsize;

            }*/
            //use here if you want to keep velocity, and set vel


            //Debug.Log(z);
            //Debug.Log(x);
            var ispeed = ((this.gameObject.GetComponent<Rigidbody>().velocity.magnitude) / speed_to_angle);
            if (ispeed > 1f) { ispeed = 1f; }
            vignette.intensity.Override(ispeed);
        }
    }

    void FixedUpdate()
    {

        if (Time.realtimeSinceStartup > 920.0f)
        {
            text.text = "時間切れです。";
            Debug.Log("finished");
            firstbug = false;
            panel.SetActive(true);
            Invoke("Quit", 5.5f);
        }
        if (firstbug)
        {
            text.text = /*.ToString() + "   " +hitting.ToString()+"  "+ preparejump.ToString() + "  " + flying.ToString();*/"残り時間: " + ((int)(920- Time.realtimeSinceStartup)).ToString()  ;


            //a = (Mathf.Sin(Time.time) * origfeet + origfeet * 1.2f) / 2;
            //Debug.Log((feet.transform.position.y-body.transform.position.y)/origfeet);
            Rigidbody rigidbody = this.GetComponent<Rigidbody>();
            //Debug.Log(rigidbody.velocity);
            if (flying) { preparejump = false; }
            //rigidbody.AddForce(new Vector3(x*speed,0,z*speed));
            if (hitting)
            {
                //Debug.Log((head.transform.position.y - (rhand.transform.position.y + lhand.transform.position.y) / 2) / origfeet);
                rigidbody.useGravity = true;
                //rigidbody.velocity += this.gameObject.GetComponent<Transform>().right* speed*(x-prex)+this.gameObject.GetComponent<Transform>().forward* speed*(z-prez);
                //rigidbody.velocity += this.gameObject.GetComponent<Transform>().right* speed*(x-prex)+this.gameObject.GetComponent<Transform>().forward* speed*(z-prez);
                //rigidbody.velocity -= savevel;
                rigidbody.velocity *= friction;
                //if (rigidbody.velocity.magnitude <= 0.3f) { rigidbody.velocity *= 0f; }//to achieve friction effect
                //savevel=this.gameObject.GetComponent<Transform>().right* speed*(x)+this.gameObject.GetComponent<Transform>().forward* speed*(z);
                //savevel = new Vector3(x, 0f, z) * speed;
                //rigidbody.velocity += savevel;
                //rigidbody.velocity += this.gameObject.GetComponent<Transform>().right* speed*(x)+this.gameObject.GetComponent<Transform>().forward* speed*(z);
                //rigidbody.velocity=new Vector3(3f,0f,0f);/////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Debug.Log(rigidbody.velocity);
                //Vector3 q=this.gameObject.GetComponent<Transform>().eulerAngles;
                vecrot = (new Vector3(origx, 0, origz));
                vecbase = head.GetComponent<Transform>().forward;
                vecbase.y = 0;

                Quaternion q = this.gameObject.GetComponent<Transform>().localRotation;
                Quaternion q1 = Quaternion.FromToRotation(vecrot * 100f, vecbase * 100f);
                difangcur = q1.eulerAngles.y - origturn;
                if (difangcur > 180) { difangcur -= 360; }
                //float difang = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch).y/2 + OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).y/2-origturn;
                anglelist.RemoveAt(0);
                if (Mathf.Sqrt(origx * origx + origz * origz) - robgo * distrot > 0)
                {
                    anglelist.Add(difangcur);
                }
                else
                {
                    anglelist.Add(0);
                }
                //predifang = difangcur;
                //difangcur = q1.eulerAngles.y - origturn;
                float difang = 0;
                for (i = 0; i < timerot; i++) { difang += anglelist[i] * (i + 1) / timerot / (timerot + 1) * 2; }

                //Debug.Log(difang);
                if ((Mathf.Abs(difang) - robturn > 0) && (Mathf.Abs(difang)) < 170)
                {
                    //Debug.Log(difang);
                    //q = q * Quaternion.Euler(0f, difang*DIFF, 0f);
                    //if ((Mathf.Sqrt(origx * origx + origz * origz) - robgo * distrot > 0)/*&&(Quaternion.Angle(origheadrot,head.transform.rotation)/ Mathf.Abs(difang) < headjudge)*/){


                    // q = q * Quaternion.Euler(0f, -(difang) * DIFF, 0f);

                    //}
                    //q = q * Quaternion.Euler(0f, -(difang) * DIFF, 0f);
                }
                q = head.transform.rotation;
                //Debug.Log(difang);
                //Debug.Log(difangcur);
                //if (Mathf.Sign(difangcur) > 160) { Debug.Log(new List<float> { origx, origz }); }
                //Debug.Log(new Vector3(difang, x, z));
                this.gameObject.GetComponent<Transform>().localRotation = q;
                //Debug.Log(Quaternion.Angle(eye.transform.rotation,head.transform.rotation));
                //Debug.Log(new Vector3(Quaternion.Angle(origheadrot, head.transform.rotation) / Mathf.Abs(difang), Quaternion.Angle(origheadrot, head.transform.rotation), difang));
                //this.gameObject.GetComponent<Transform>().eulerAngles=q;
                /*if ((Input.GetKey(KeyCode.Z)&&(x==0))&&(z==0)) {
        //rigidbody.velocity=new Vector3(0f,0f,0f);
        savevel=new Vector3(0f,0f,0f);
        rigidbody.velocity*=0;
        //Debug.Log(savevel);

      }*/
                //if(Input.GetKeyUp(KeyCode.Space)){
                //if(((body.transform.position.y-feet.transform.position.y)/origfeet>=startjump)&&preparejump){
                jumplist.RemoveAt(0);
                //Debug.Log(new Vector3(jumplist.Max() - 1, (origheadpos - (head.transform.position.y - feet.transform.position.y)) / origfeet + 1, 0));
                //jumplist.Add((head.transform.position.y - (rhand.transform.position.y + lhand.transform.position.y) / 2) / origfeet);
                if (Mathf.Abs(jumplist[(int)(timeforjump / Time.fixedDeltaTime) - 2] - ((origheadpos - (head.transform.position.y - feet.transform.position.y)) / origfeet + 1)) >= 0.1f)
                {
                    jumplist.Add(-1 * Mathf.Sign(jumplist[(int)(timeforjump / Time.fixedDeltaTime) - 2] - ((origheadpos - (head.transform.position.y - feet.transform.position.y)) / origfeet + 1)) * 0.1f + jumplist[(int)(timeforjump / Time.fixedDeltaTime) - 2]);
                }
                else
                {
                    jumplist.Add((origheadpos - (head.transform.position.y - feet.transform.position.y)) / origfeet + 1);
                }
                //if ((/*(((head.transform.position.y-(rhand.transform.position.y+lhand.transform.position.y)/2)/origfeet)<=startjumppos)&&*/preparejump) && (((head.transform.position.y - (rhand.transform.position.y + lhand.transform.position.y) / 2) / origfeet) <= jumplist.Max() - startjumpdif))
                if ((preparejump) && (jumplist[(int)(timeforjump / Time.fixedDeltaTime) - 1] <= 1 + startjumpdif * robjump))

                {


                    //Debug.Log("spaceup");
                    preparejump = false;
                    precount++;


                    Debug.Log(new Vector3(jumplist[(int)(timeforjump / Time.fixedDeltaTime) - 1], jumplist[(int)(timeforjump / Time.fixedDeltaTime) - 2], jumplist[(int)(timeforjump / Time.fixedDeltaTime) - 3]));

                    rigidbody.position += new Vector3(0, 1, 0) * 0.05f;
                    rigidbody.velocity += jumpdir * addjump * jump_power;
                    //Debug.Log(hitting);
                    //Debug.Log(rigidbody.velocity);
                    //Debug.Log(addjump);
                    addjump = 0;
                    flying = true;
                    rigidbody.useGravity = false;
                    //prevel=rigidbody.velocity;

                    prevel = rigidbody.velocity;
                }

                //if(Input.GetKey(KeyCode.Space)){
                //if((jumplist.Max()-jumplist.Min())>=startjumpdif){
                //if ((((head.transform.position.y - (rhand.transform.position.y + lhand.transform.position.y) / 2) / origfeet) - jumplist.Min()) >= startjumpdif)
                if ((jumplist[(int)(timeforjump / Time.fixedDeltaTime) - 1] - 1) >= startjumpdif && !flying)
                {
                    //Debug.Log("spacdonw");
                    //Debug.Log((jumplist.Max()-jumplist.Min())/origfeet);
                    preparejump = true;
                    

                    //if(addjump<(body.transform.position.y-feet.transform.position.y)*jump_power){addjump=(body.transform.position.y-feet.transform.position.y)*jump_power;}
                    if (addjump < (jumplist.Max() - 1 - startjumpdif + basepos) && (jumplist.Max() - 1 - startjumpdif) < threshjump) { addjump = (jumplist.Max() - 1 - startjumpdif + basepos); }
                    //if (addjump < (jumplist.Max()- 1-startjumpdif+basepos)){if(jumplist.Max() - 1-startjumpdif < threshjump) { addjump = (jumplist.Max() - 1+basepos-startjumpdif); }else{addjump=threshjump+basepos;}}
                    prevel = rigidbody.velocity + jumpdir * addjump * jump_power;
                    showray();


                }

            }
            if (!hitting) { preparejump = false; }
            if (flying == true&&!hitting)
            {

                //when flying
                //Debug.Log(rigidbody.velocity);
                rigidbody.useGravity = false;
                Debug.Log(hitting);
                prevel -= new Vector3(0f, Time.fixedDeltaTime * gravity, 0f);

                rigidbody.velocity = prevel;
                //Debug.Log(prevel);

                //Debug.Log(Time.fixedDeltaTime);
                //Debug.Log("flying");
                showray();


            }
            else
            {
                //Debug.Log("not flying");
            }
            hitting = false;

            time += Time.fixedDeltaTime;

            if (time > Time.fixedDeltaTime * 2) { if (obj) { Destroy(obj); } }
        }
    }
    void showray()
    {
        distTraveled = 0.0f;
        startPosition = this.transform.position;
        y = 0f;
        //Debug.Log("showray");

        downward = new Vector3(0f, 1 * Time.fixedDeltaTime * gravity, 0f);
        //Debug.Log(direction);

        while (distTraveled <= maxDist)
        {
            velocity = (prevel + downward).magnitude;
            direction = (prevel + downward).normalized;
            //direction = transform.TransformDirection(new Vector3(0f,0f,1f));
            distance = velocity * Time.fixedDeltaTime;
            downward -= new Vector3(0f, Time.fixedDeltaTime * gravity, 0f);
            //Debug.Log(direction);
            if (Physics.Raycast(startPosition, (prevel + downward)/*direction*/, out hit, distance))
            {
                //Debug.Log(hit);

                //Debug.Log("break");

                time = 0f;
                if (hit.collider.transform.root.gameObject/*gameObject*/!= this.gameObject)
                {
                    if (obj) { Destroy(obj); }

                    obj = Instantiate(predp, hit.point, Quaternion.identity);
                    //Debug.Log(hit.collider.gameObject.name);
                    break;
                }
                /*if (hitDecal &&hit.transform.tag == "levelParts") {
                    Instantiate(hitDecal, hit.point + (hit.normal * _wallDisplacement), Quaternion.LookRotation(hit.normal));
                    Destroy (gameObject);
                }*/
            }

            //y -= gravity * Time.fixedDeltaTime;
            endPosition = startPosition + (prevel + downward) * Time.fixedDeltaTime;

            //if (showTrail){
            Debug.DrawLine(startPosition, endPosition, Color.green, Time.fixedDeltaTime);

            //Gizmos.DrawLine(startPosition, endPosition, Color.green, 3.0f);
            //  }
            distTraveled += Vector3.Distance(startPosition, endPosition);
            startPosition = endPosition;
            //if(!firstbug){Debug.Log(endPosition);}
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.transform.root.gameObject != this.gameObject)
        {
            //Debug.Log ("Collision Stay: " + collision.gameObject.name);
            hitting = true;
            if (flying)
            {
                vecrot = (new Vector3(origx, 0, origz));
                vecbase = head.GetComponent<Transform>().forward;
                vecbase.y = 0;

                Quaternion q = this.gameObject.GetComponent<Transform>().localRotation;
                Quaternion q1 = Quaternion.FromToRotation(vecrot * 100f, vecbase * 100f);
                difangcur = q1.eulerAngles.y - origturn;
                preparejump = false;
                for (i = 0; i < timerot; i++) { anglelist[i] = difangcur; }
                for (i = 0; i < timepos; i++)
                {
                    rawxlist[i] = origx;
                    rawzlist[i] = origz;
                }
                for (i = 0; i < (int)(timeforjump / Time.fixedDeltaTime); i++) { //jumplist[i] = ((origheadpos - (head.transform.position.y - feet.transform.position.y)) / origfeet + 1);
                    jumplist[i] = 1;
                }
            }
            flying = false;
            //Debug.Log(collision.gameObject.name);
            //jumpdir = head.transform.forward + jumpangle * new Vector3(0, 1, 0);
            Vector3 predir=head.transform.forward;
            predir.y=0;

            jumpdir = predir.normalized + jumpangle * new Vector3(0, 1, 0);
            //Debug.Log(jumpdir);
            //if (jumpdir.y<0){jumpdir*=-1;}
            //if (Vector3.Dot(jumpdir, (this.GetComponent<Rigidbody>().position - collision.contacts[0].point)) < 0) { jumpdir *= -1; }

        }
    }


    void Quit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
    UnityEngine.Application.Quit();
#endif
    }
}
