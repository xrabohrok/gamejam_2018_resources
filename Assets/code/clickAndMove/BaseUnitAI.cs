using UnityEngine;


public class BaseUnitAI : MonoBehaviour {
    public float stopDist = .1f;
    public float speed = 1.0f;
    public float rotateSpeed = Mathf.PI;
    private bool selected = false;
    private bool onTheMove = false;
    public bool IsSelected
    {
        get { return selected; }
    }


    Vector3 targetLocation;

    private CharacterController controller;
    
	// Use this for initialization
	void Start () {
        controller = this.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
	
        //don't look up or down
        targetLocation = new Vector3(targetLocation.x, this.transform.position.y, targetLocation.z );

        if(onTheMove)
        {

            //move
            var step = speed * Time.deltaTime;
            var floatingDirection = (targetLocation - controller.transform.position).normalized * step;
            var finalDirection = new Vector3(floatingDirection.x, -3, floatingDirection.z);
            controller.Move(finalDirection);


            //Don't get too close
            if(Vector3.Distance(this.transform.position, targetLocation) <= stopDist)
            {
                onTheMove = false;
            }
            else
            {
                //rotate
                var newRotation = Vector3.RotateTowards(controller.transform.forward, targetLocation, rotateSpeed * Time.deltaTime, 0f );
                this.transform.rotation = Quaternion.LookRotation(newRotation);
                this.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); 
            }
        }
        else
        {
            //idle here
        }
	}

    public void WasSelected()
    {
        selected = true;
    }

    public void WasDeSelected()
    {
        selected = false;
    }

    public void RecieveOrderTo(Vector3 location)
    {
        if (selected)
        {
            targetLocation = new Vector3(location.x, controller.transform.position.y, location.z);
            onTheMove = true;
        }
    }

}
