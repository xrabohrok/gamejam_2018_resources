using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Overhead2DController : MonoBehaviour
{

    public float MaxSpeed = 5.0f;
    public float secondsToMaxSpeed = 2.0f;
    public AnimationCurve moveAcceleration;
    public float graceTimeForStopping = .03f;

    private ConsumableDirection directionRequest;
    private Rigidbody2D body;

    private float secondsAccelerating = 0;
    private float secondsStopping = .05f;
    private float maxTimeCurve;

    public Vector2 Velocity
    {
        get { return body.velocity; }
    }


	// Use this for initialization
	void Start ()
	{
	    body = this.GetComponent<Rigidbody2D>();
	    secondsAccelerating = secondsToMaxSpeed;
	    directionRequest = new ConsumableDirection(Vector2.zero) {consumed = true};
	    maxTimeCurve = moveAcceleration[moveAcceleration.length - 1].time;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SimpleMove(Vector2 direction)
    {
        directionRequest.direction = new Vector2(direction.x, direction.y);
        directionRequest.consumed = false;
    }

    void FixedUpdate()
    {
        if (directionRequest != null && !directionRequest.consumed)
        {
            secondsStopping = 0;
            float slice = ( maxTimeCurve) *
                                         ((secondsToMaxSpeed - secondsAccelerating) / secondsToMaxSpeed);
            float speed = MaxSpeed * moveAcceleration.Evaluate(slice);

            body.velocity = directionRequest.direction * speed;
            directionRequest.consumed = true;

            if (secondsAccelerating >= 0)
            {
                secondsAccelerating -= Time.deltaTime;
            }
            if (secondsAccelerating < 0)
            {
                secondsAccelerating = 0.0f;
            }
        }
        else
        {
            if(secondsStopping > graceTimeForStopping)
            {
                secondsAccelerating = secondsToMaxSpeed;
            }
            else
            {
                secondsStopping += Time.deltaTime;
            }
        }
    }

    class ConsumableDirection
    {
        public ConsumableDirection(Vector2 dir)
        {
            direction = new Vector2(dir.x, dir.y);
            consumed = false;
        }

        public bool consumed = false;
        public Vector2 direction;

    }
}
