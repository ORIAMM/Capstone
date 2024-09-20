using UnityEngine;

public class HoverDevice2 : MonoBehaviour
{
    [Header("Hover Settings")]
    public float ForceMultiplier;
    public float ForceDampValue;
    [Range(0,1000)]
    public float distance;
    public Rigidbody rb;

    //default = 9.81f * mass keatas
    public const float GRAVITY = 9.81f;
    float Normal => GRAVITY * rb.mass;
    float Force;
    RaycastHit hit;
    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1000))
        {
            rb.AddForceAtPosition(transform.up * CalculateForce(hit.distance), transform.position);
        }
    }
    //ini stable statenya, kalo makin kebawah forcenya ditambah, kalo keatas dikacangin aj (soalnya gravity)
    //klo menurut gw, strengthnya harus relatif lebih kecil dari Normal forcenya jd
    private float CalculateForce(float hitDistance)
    {
        float newDistance = distance - hitDistance >= 0 ? Mathf.Pow((distance - hitDistance)/distance, ForceDampValue) * 2: (distance - hitDistance)/distance * ForceDampValue;
        Force = Normal + Normal * newDistance * ForceMultiplier;
        Force = Mathf.Clamp(Force, 0f, Force);
        return Force;
    }
    private void OnGUI()
    {
        GUI.TextArea(new Rect(0, 0, 100, 100), Force.ToString("Force : #.##"));
    }
}
