
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIController : MonoBehaviour
{
    [SerializeField] private GameObject fallenPlayerPrefab;
    [SerializeField] private GameObject fallenPlayerInstance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource DriveSound;
    [SerializeField] private AudioSource LandSound;
    [SerializeField] private AudioSource JumpSound;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider box;
    [SerializeField] private Collider frontWheel;
    [SerializeField] private Collider backWheel;

    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite idle;
    [SerializeField] private Sprite idle2;
    [SerializeField] private Sprite turnDown;
    [SerializeField] private Sprite turnUp;
    [SerializeField] private Sprite emptyBike;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite[] recoveryAnim;
    [SerializeField] private AngleSprite[] angleList;

    [SerializeField] private float Speed;
    [SerializeField] private float deceleration = 0.5f;
    [SerializeField] private float Turn;
    [SerializeField] private float yMovement;
    [SerializeField] private float driveSpeed = 4;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float fakeAngle = 0;
    [SerializeField] private float fallOverAngle = 0;
    [SerializeField] private float flyOffDist = 0;
    [SerializeField] private float flyOffTargetDist = 4;
    [SerializeField] private float bikeRecoverTime = 1;
    [SerializeField] private float bikeRecoverTimer = 0;
    [SerializeField] private float maxAngleSpeed = 3;

    [SerializeField] private int racingLane = 3;
    [SerializeField] private int targetLane = 3;
    private int currentFrame = 0;

    [SerializeField] private bool grounded = false;
    [SerializeField] private bool previousGrounded = false;
    [SerializeField] private bool isFlatGround = false;
    [SerializeField] private bool fallOver = false;
    [SerializeField] private bool fallOff = false;
    [SerializeField] private bool flyingOffBike = false;
    [SerializeField] private bool runToBike = false;
    [SerializeField] private bool changingLane = false;
    [SerializeField] private bool gettingOnBike = false;
    [SerializeField] private bool playingDrive = false;
    public bool isFinished = false;


    void Start()
    {
        rb.maxAngularVelocity = maxAngleSpeed;
        targetLane = racingLane;
    }

    void Update()
    {
        RaycastHit hit;
        if (!isFinished)
        {
            if (Physics.SphereCast(transform.position, 0.25f, transform.right, out hit, 4, LayerMask.GetMask("Bikers")))
            {
                if (!changingLane)
                {
                    if (Random.Range(0, 50) == 0)
                    {
                        PickLane(new List<int>() { racingLane - 1, racingLane + 1 });
                    }
                }
            }
            else if (Random.Range(0, 1500) == 0 && !changingLane)
            {
                PickLane(new List<int>() { racingLane - 1, racingLane + 1 });
            }
        }

        if (!fallOver)
        {
            if (!isFinished)
            {
                Speed = Mathf.Min(Speed + driveSpeed, maxSpeed);
                if (!playingDrive)
                {
                    DriveSound.Play();
                    DriveSound.volume = 0.25f;
                    playingDrive = true;
                }
            }
            else
            {
                Speed = Mathf.Max(Speed - deceleration, 0);
            }

            if (grounded == true)
            {
                if (!changingLane)
                {
                    //Change the target Racing Lane
                    racingLane = targetLane;
                }
            }
        }

        if (fallOver)
        {
            if (!fallOff)
            {
                if (grounded)
                {
                    if (isFlatGround)
                    {
                        Speed *= 0.9f;
                        if (Speed < 0.1f)
                        {
                            FallOffBike();
                        }
                    }
                    else
                    {
                        Speed = Mathf.Max(Speed, 2);
                    }
                }

                fallOverAngle += Speed;
            }
        }
        else
        {
            //rb.AddTorque(new Vector3(0, 0, horizontalAxis * -angleForce));
        }


        fakeAngle = transform.localEulerAngles.z;
        if (!fallOver)
        {
            if (fakeAngle > 180 && fakeAngle < 360)
            {
                fakeAngle = -(360 - fakeAngle);
            }
            fakeAngle = Mathf.Clamp(fakeAngle, -74, 80);
        }
        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, -fakeAngle);
        if (fallenPlayerInstance != null)
        {
            fallenPlayerInstance.transform.localEulerAngles = new Vector3(0, 0, -fakeAngle);
        }

        if (!isFinished)
        {
            if (!fallOff)
            {
                spriteRenderer.sprite = GetSpriteFromAngle(fakeAngle - fallOverAngle);
            }
            else
            {
                spriteRenderer.sprite = emptyBike;
                if (flyingOffBike)
                {
                    if (fallenPlayerInstance != null)
                    {

                        flyOffDist = Mathf.Clamp(flyOffDist + 0.1f, 0, flyOffTargetDist);
                        if (flyOffDist > flyOffTargetDist - 0.01f)
                        {
                            flyingOffBike = false;
                            fallenPlayerInstance.GetComponent<FallenPlayer>().enabled = true;
                            runToBike = true;
                        }
                    }
                    fallenPlayerInstance.transform.localPosition = new Vector3(0, 0, flyOffDist);
                }

                if (runToBike)
                {
                    flyOffDist = Mathf.Clamp(flyOffDist - 0.03f, 0, 5);
                    if (flyOffDist < 0.01f)
                    {
                        MountBike();
                    }
                    fallenPlayerInstance.transform.localPosition = new Vector3(0, 0, flyOffDist);
                }
            }
        }
        else
        {
            spriteRenderer.sprite = winSprite;
            fallOver = false;
            fallOff = false;
            DriveSound.Stop();
        }

//Lane Swapping
float zDistance = ((racingLane * 2) - 3) - transform.position.z;

        if (Mathf.Abs(zDistance) > 0.05f) // Lane Proximity Threshhold
        {
            changingLane = true;
            if (!fallOver)
            {
                if (zDistance >= 0)
                {
                    spriteRenderer.sprite = turnUp;
                }
                else if (zDistance <= 0)
                {
                    spriteRenderer.sprite = turnDown;
                }
            }
            Turn = Mathf.Sign(zDistance) * turnSpeed;
        }
        else
        {
            Turn = 0;
            changingLane = false;
            //Snap to exact lane
            Vector3 pos = transform.position;
            pos.z = ((racingLane * 2) - 3);
            transform.position = pos;


        }

        if (gettingOnBike)
        {
            bikeRecoverTimer += Time.deltaTime;

            int animationIndex = Mathf.FloorToInt(Mathf.Min(recoveryAnim.Length - 1, bikeRecoverTimer / bikeRecoverTime * recoveryAnim.Length));

            spriteRenderer.sprite = recoveryAnim[animationIndex];

            if (bikeRecoverTimer >= bikeRecoverTime)
            {
                gettingOnBike = false;
                fallOver = false;
                fallOff = false;
            }
        }

        //This line should always stay at the bottom.
        rb.linearVelocity = new Vector3(Speed, rb.linearVelocity.y, Turn);
    }

    private void FixedUpdate()
    {
        if (fallOver != true)
        {
            //Clamp the rotation between 90 and -75 degrees.
            if (rb.rotation.eulerAngles.z > 180 && rb.rotation.eulerAngles.z < 360)
            {
                if (-(360 - rb.rotation.eulerAngles.z) < -75)
                {
                    rb.rotation = Quaternion.Euler(new Vector3(0, 0, -75));
                    rb.angularVelocity = Vector3.zero;
                }
            }
            else if (rb.rotation.eulerAngles.z > 80)
            {
                rb.rotation = Quaternion.Euler(new Vector3(0, 0, 80));
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (!previousGrounded && grounded)
        {
            LandSound.Play();
            LandSound.volume = 0.25f;
        }
        if (previousGrounded && !grounded)
        {
            JumpSound.Play();
            JumpSound.volume = 0.25f;
        }

        previousGrounded = grounded;
        grounded = false;
        isFlatGround = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            if (!isFinished) { CrashOut(); }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision);
    }

    private void HandleCollision(Collision collision)
    {
        grounded = true;

        Vector3 normal = Vector3.zero;

        for (int i = 0; i < collision.contactCount; i++)
        {
            normal += collision.contacts[i].normal;
        }

        normal /= collision.contactCount;

        isFlatGround = Vector3.Dot(Vector3.up, normal) > 0.99f;

        if (Vector3.Dot(transform.up, normal) < -0.01f)
        {
            if (fallOver != true) { CrashOut(); }
        }
    }

    private void CrashOut()
    {
        fallOver = true;
        fallOverAngle = 0;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    private void FallOffBike()
    {
        fallOff = true;
        flyingOffBike = true;
        Speed = 0;
        flyOffDist = 0;
        rb.rotation = Quaternion.Euler(Vector3.zero);
        fallOverAngle = 0;
        racingLane = 4;
        fallenPlayerInstance = Instantiate(fallenPlayerPrefab, transform, false);
        fallenPlayerInstance.GetComponent<SpriteRenderer>().material = spriteRenderer.material;
        fallenPlayerInstance.transform.localPosition = Vector3.zero;
    }

    private void MountBike()
    {
        runToBike = false;
        racingLane = 3;
        gettingOnBike = true;
        bikeRecoverTimer = 0f;
        if (fallenPlayerInstance != null)
        {
            Destroy(fallenPlayerInstance);
        }
    }

    private Sprite GetSpriteFromAngle(float angle)
    {
        float smallestDelta = Mathf.Infinity;

        Sprite closestSprite = null;

        for (int i = 0; i < angleList.Length; i++)
        {
            float delta = Mathf.Abs(Mathf.DeltaAngle(angleList[i].angle, angle));

            if (delta < smallestDelta)
            {
                smallestDelta = delta;
                closestSprite = angleList[i].sprite;
            }
        }
        if (closestSprite == idle)
        {
            if (Speed >= 0.1f && grounded)
            {
                if (Time.frameCount % 10 == 0)
                {
                    currentFrame = (currentFrame + 1) % 2;
                }

                if (currentFrame == 1)
                {
                    closestSprite = idle2;
                }
            }
        }

        return closestSprite;
    }

    private void PickLane(List<int> lanes)
    {
        List<int> possibleLanes = new List<int>();
        for(int i = 0; i < lanes.Count; i++)
        {
            if (lanes[i] >= 0 && lanes[i] <= 3)
            {
                possibleLanes.Add(lanes[i]);
            }
        }

        targetLane = possibleLanes[Random.Range(0, possibleLanes.Count)];
    }
}
