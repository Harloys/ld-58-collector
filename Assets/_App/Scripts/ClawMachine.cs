using UnityEngine;
using System.Collections;
using UnityEngine.Splines;

public class ClawMachine : MonoBehaviour
{
    public enum ClawState
    {
        Idle,
        Dropping,
        Lifting,
        MovingToRelease,
        Releasing,
        Returning
    }

    [Header("References")]
    public Transform Carrier;
    public Transform ClawRig;
    public Collider GrabZone;
    public Transform ReleasePosition;
    public Transform SplinePoint;
    public SplineContainer SplineContainer;
    public SplineContainer SplineContainer2;
    public SplineContainer SplineContainer3;
    public AudioSource Source; 
    public AudioSource Click; 
    public AudioSource ClickOut;


    public MeshRenderer Renderer;
    public Material Player;
    public Material AIMat;

    public Animator AnimatorClaw;
    
    [Header("Play Area")]
    public float RectWidth = 0.5f;
    public float RectHeight = 0.5f;

    [Header("Movement")]
    public float MaxSpeed = 2f;
    public float Acceleration = 5f;
    public float Deceleration = 5f;

    [Header("Claw Drop")]
    public float DropDistance = 1f;
    public float DropSpeed = 2f;

    [Header("Releasing")]
    public float MoveToReleaseSpeed = 2f;
    public float MoveToCenterSpeed = 1f;
    public Vector2 ReleasePoint = new Vector2(0.862f, 0.0791f);
    public Vector2 CenterPoint = new Vector2(0.5f, 0.5f);

    [Header("Carrier Rotation")]
    public Rigidbody carrierRb;       

    [Header("Release Chance")]
    [Range(0f, 1f)]
    public float GrabReleaseChance = 0.2f;

    public bool AI = false;
    
    [Header("Debug")]
    [SerializeField] private ClawState currentState = ClawState.Idle;

    public float PosX = 0.5f;
    public float PosZ = 0.5f;
    private Vector2 velocity;
    private Transform grabbedItem = null;
    private FixedJoint grabJoint;

    private Vector3 basePos;
    private Vector3 prevPos;
    private Vector3 currentVel;
    private float sourceVolume = 0f;

    private float aiTimer = 0f;
    private float aiNextActionTime = 0f;
    private Vector2 aiInputDir = Vector2.zero;
    private bool aiDrop = false;
    
    void Awake()
    {
        basePos = ClawRig.position;
        Source.Play();
    }
    
    private void FixedUpdate()
    {
        Renderer.material = AI ? AIMat : Player;
        
        Vector3 currentPos = ClawRig.localPosition;
        currentVel = (currentPos - prevPos) / Time.fixedDeltaTime;
        prevPos = currentPos;

        sourceVolume = Mathf.MoveTowards(sourceVolume, currentVel.magnitude > 0f ? 1f : 0f, Time.deltaTime * 2);
        Source.volume = sourceVolume;
        
        HandleCarrierRotation();

        var spline = SplineContainer.Spline;
        var spline1 = SplineContainer2.Spline;
        var spline2 = SplineContainer3.Spline;

        var knot = spline[0];
        var knot1 = spline1[0];
        var knot2 = spline2[0];

        Vector3 localPos = SplineContainer.transform.InverseTransformPoint(SplinePoint.position);
        knot.Position = localPos;
        knot1.Position = localPos;
        knot2.Position = localPos;

        spline[0] = knot;     
        spline1[0] = knot1;        
        spline2[0] = knot2;        

        if (currentState != ClawState.Idle)
            return;

        if (AI)
        {
            aiTimer += Time.fixedDeltaTime;
            if (aiTimer >= aiNextActionTime)
            {
                aiTimer = 0f;
                aiNextActionTime = Random.Range(1f, 3f);
                aiInputDir = Random.insideUnitCircle.normalized;
                if (Random.value < 0.3f) aiDrop = true;
            }
            HandleMovement(aiInputDir);
            if (aiDrop)
            {
                aiDrop = false;
                StartCoroutine(DropClaw());
            }
        }
        else
        {
            float hor = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            HandleMovement(new Vector2(hor, -vert).normalized);

            if (Input.GetKey(KeyCode.Space))
            {
                StartCoroutine(DropClaw());
            }
        }
    }

    private void HandleMovement(Vector2 inputDir)
    {
        if (inputDir.sqrMagnitude > 0.01f)
        {
            velocity = Vector2.MoveTowards(
                velocity,
                inputDir * MaxSpeed,
                Acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            velocity = Vector2.MoveTowards(
                velocity,
                Vector2.zero,
                Deceleration * Time.fixedDeltaTime
            );
        }

        PosX += velocity.y * Time.fixedDeltaTime;
        PosZ += velocity.x * Time.fixedDeltaTime;

        PosX = Mathf.Clamp01(PosX);
        PosZ = Mathf.Clamp01(PosZ);

        float newX = (PosX - 0.5f) * RectWidth;
        float newZ = (PosZ - 0.5f) * RectHeight;
        
        ClawRig.localPosition = new Vector3(newX, 0, newZ);
    }
    
    private void HandleCarrierRotation()
    {
        var dir = new Vector3(velocity.x, 0, velocity.y);
        carrierRb.transform.localEulerAngles += dir;
    }

    public float SpeedMultiplyEffect = 0.25f;
    
    private IEnumerator DropClaw()
    {
        currentState = ClawState.Dropping;

        Vector3 startPos = ClawRig.localPosition;
        Vector3 targetPos = startPos + Vector3.down * DropDistance;

        var timer = 0f;
        var grabCalled = false;
        
        while (Vector3.Distance(ClawRig.localPosition, targetPos) > 0.01f)
        {
            ClawRig.localPosition = Vector3.MoveTowards(
                ClawRig.localPosition,
                targetPos,
                DropSpeed * Time.deltaTime + (EffectsController.Instance.SpeedMult * SpeedMultiplyEffect)
            );

            timer += Time.deltaTime;

            if (!grabCalled && timer > 1.5f)
            {
                AnimatorClaw.SetBool("Grab", true);
                grabCalled = true;
            }

            if (grabbedItem == null)
            {
                Grab();
            }
            else
            {
                break;
            }

            yield return null;
        }

        currentState = ClawState.Lifting;
        while (Vector3.Distance(ClawRig.localPosition, startPos) > 0.01f)
        {
            ClawRig.localPosition = Vector3.MoveTowards(
                ClawRig.localPosition,
                startPos,
                DropSpeed * Time.deltaTime + (EffectsController.Instance.SpeedMult * SpeedMultiplyEffect)
            );
            yield return null;
        }

        if (grabbedItem != null && ReleasePosition != null)
        {
            currentState = ClawState.MovingToRelease;
            yield return MoveToReleaseAndDrop();
        }

        AnimatorClaw.SetBool("Grab", false);

        currentState = ClawState.Returning;
        yield return MoveClawToCenter();

        currentState = ClawState.Idle;
    }
    
    private void Grab()
    {
        if (GrabZone == null)
            return;

        Collider[] hits = Physics.OverlapBox(
            GrabZone.bounds.center,
            GrabZone.bounds.extents,
            GrabZone.transform.rotation
        );

        foreach (Collider hit in hits)
        {
            if(!hit.CompareTag("Ball"))
                continue;
            
            if (hit.TryGetComponent<HingeJoint>(out var _))
                continue;
                
            if (hit != GrabZone && hit.attachedRigidbody != null)
            {
                grabbedItem = hit.transform;

                if (grabbedItem.TryGetComponent<Rigidbody>(out var rb))
                {
                    var joint = grabbedItem.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = GrabZone.GetComponent<Rigidbody>();
                    joint.autoConfigureConnectedAnchor = false;

                    joint.anchor = grabbedItem.InverseTransformPoint(grabbedItem.position);
                    joint.connectedAnchor = Vector3.zero;

                    Click.Play();
                    grabJoint = joint;
                }

                break;
            }
        }
    }

    private IEnumerator MoveToReleaseAndDrop()
    {
        float checkInterval = 0.3f;
        float timer = 0f;

        while (Mathf.Abs(PosX - ReleasePoint.x) > 0.001f || Mathf.Abs(PosZ - ReleasePoint.y) > 0.001f)
        {
            //PosX = Mathf.MoveTowards(PosX, ReleasePoint.x, Time.deltaTime * MoveToReleaseSpeed + (EffectsController.Instance.SpeedMult * SpeedMultiplyEffect));
            //PosZ = Mathf.MoveTowards(PosZ, ReleasePoint.y, Time.deltaTime * MoveToReleaseSpeed + (EffectsController.Instance.SpeedMult * SpeedMultiplyEffect));
            PosX = Mathf.MoveTowards(PosX, ReleasePoint.x, Time.deltaTime * MoveToReleaseSpeed);
            PosZ = Mathf.MoveTowards(PosZ, ReleasePoint.y, Time.deltaTime * MoveToReleaseSpeed);
            
            PosX = Mathf.Clamp01(PosX);
            PosZ = Mathf.Clamp01(PosZ);

            float newX = (PosX - 0.5f) * RectWidth;
            float newZ = (PosZ - 0.5f) * RectHeight;

            ClawRig.localPosition = new Vector3(newX, 0, newZ);

            timer += Time.deltaTime;

            if (grabbedItem != null && grabJoint != null && timer >= checkInterval)
            {
                timer = 0f;

                if (Random.value < GrabReleaseChance)
                {
                    Destroy(grabJoint);
                    grabJoint = null;
                    grabbedItem = null;
                    Debug.Log("Item dropped prematurely!");
                    ClickOut.Play();
                }
            }

            yield return null;
        }

        currentState = ClawState.Releasing;
        AnimatorClaw.SetBool("Grab", false);

        yield return new WaitForSeconds(0.3f);

        if (grabbedItem != null)
        {
            if (grabJoint != null)
            {
                Destroy(grabJoint);
                grabJoint = null;
            }

            grabbedItem = null;
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator MoveClawToCenter()
    {
        while (Mathf.Abs(PosX - CenterPoint.x) > 0.001f || Mathf.Abs(PosZ - CenterPoint.y) > 0.001f)
        {
            PosX = Mathf.MoveTowards(PosX, CenterPoint.x, Time.deltaTime * MoveToCenterSpeed);
            PosZ = Mathf.MoveTowards(PosZ, CenterPoint.y, Time.deltaTime * MoveToCenterSpeed);
            
            PosX = Mathf.Clamp01(PosX);
            PosZ = Mathf.Clamp01(PosZ);

            float newX = (PosX - 0.5f) * RectWidth;
            float newZ = (PosZ - 0.5f) * RectHeight;
            
            ClawRig.localPosition = new Vector3(newX, 0, newZ);
            
            yield return null;
        }
        
        velocity = Vector2.zero;
    }
}
