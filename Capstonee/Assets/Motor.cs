using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public float maxRotationOnY, maxRotationOnZ, rotspeed;
    public float RotationYMultiplayer, RotationZMultiplayer;
    Rigidbody rb;
    Vector3 rot;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //y kanan znya kiri
        float a = Input.GetAxisRaw("Horizontal") * Time.deltaTime * rotspeed;
         rot = transform.rotation.eulerAngles + new Vector3(0, a * RotationYMultiplayer, -a * RotationZMultiplayer); //use local if your char is not always oriented Vector3.up
        rot.y = Mathf.Clamp(rot.y, -maxRotationOnY, maxRotationOnY);/*ClampAngle(rot.y, -maxRotationOnY, maxRotationOnY);*/
        rot.z = Mathf.Clamp(rot.z, -maxRotationOnZ , maxRotationOnZ);

        transform.eulerAngles = rot;
        //rb.AddTorque(new Vector3(0, 1, -1) * a);
        //transform.eulerAngles = new Vector3(0, Mathf.Clamp(transform.eulerAngles.y, -maxRotationOnY, maxRotationOnY), Mathf.Clamp(transform.eulerAngles.z, -maxRotationOnZ, maxRotationOnZ));
    }
    private void OnGUI()
    {
        GUILayout.TextField($"Vector clamp:{rot}");
    }
    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    void RotateInFrame()
    {
        //float mx = Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed;
        //float my = Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed;

        //Vector3 rot = transform.rotation.eulerAngles + new Vector3(-my, mx, 0f); //use local if your char is not always oriented Vector3.up
        //rot.x = ClampAngle(rot.x, -60f, 60f);

        //transform.eulerAngles = rot;
    }
}

