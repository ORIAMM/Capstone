using UnityEngine;

public class HoverCraft : MonoBehaviour
{
    RaycastHit hit;
    float moveInput, steerInput, raylength, currentVelocityOffset;

    [HideInInspector] public Vector3 velocity;

    public GameObject Handle;
    public float maxSpeed, acceleration, steerStrength, tiltAngle, gravity, bikeXTiltIncrement = .09f, zTiltAngle = 45f, handleRotVal = 30f, handleRotSpeed = .15f;
    [Range(1,10)]
    public float breakingFactor;
    public LayerMask derivableSurface;

    [SerializeField] Rigidbody BikeRB, SphereRB;
    private void Awake()
    {
        SphereRB.transform.SetParent(null);
        BikeRB.transform.SetParent(null);

        raylength = SphereRB.GetComponent<SphereCollider>().radius + 0.2f;
    }
    private void Update()
    {
        moveInput = Input.GetAxis("vertical");

        transform.position = SphereRB.transform.position;

        velocity = BikeRB.transform.InverseTransformDirection(BikeRB.velocity);
        currentVelocityOffset = velocity.z / maxSpeed;
    }
    private void FixedUpdate()
    {
        Movement();
    }
    void Movement()
    {
        if (Grounded())
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                Acceleration();
                Rotation();
            }
            else
            {
                Brake();
            }
        }
        else
        {
            Gravity();
        }
        BikeTIlt();
    }
    void Acceleration()
    {
        SphereRB.velocity = Vector3.Lerp(SphereRB.velocity, maxSpeed * moveInput * transform.forward, Time.fixedDeltaTime * acceleration);
    }
    void Rotation()
    {
        transform.Rotate(0, steerInput * moveInput * currentVelocityOffset * steerStrength * Time.fixedDeltaTime, 0, Space.World);
        Handle.transform.localRotation = Quaternion.Slerp(Handle.transform.localRotation, Quaternion.Euler(Handle.transform.localRotation.eulerAngles.x, handleRotVal * steerInput, Handle.transform.localRotation.eulerAngles.z), handleRotSpeed);
    }
    void BikeTIlt()
    {
        float xRot = (Quaternion.FromToRotation(BikeRB.transform.up, hit.normal) * BikeRB.transform.rotation).eulerAngles.x;
        float zRot = 0;

        if(currentVelocityOffset > 0)
        {
            zRot = -zTiltAngle * steerInput * currentVelocityOffset;
        }

        Quaternion targetRot = Quaternion.Slerp(BikeRB.transform.rotation, Quaternion.Euler(xRot, transform.eulerAngles.y, zRot), bikeXTiltIncrement);
        Quaternion newRotation = Quaternion.Euler(targetRot.x, transform.eulerAngles.y, targetRot.eulerAngles.z);

        BikeRB.MoveRotation(newRotation);
    }
    void Brake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SphereRB.velocity *= breakingFactor / 10;
        }
    }
    bool Grounded()
    {
        return Physics.Raycast(SphereRB.position, Vector3.down, out hit, raylength, derivableSurface) ;
    }
    void Gravity()
    {
        SphereRB.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
    }
}
