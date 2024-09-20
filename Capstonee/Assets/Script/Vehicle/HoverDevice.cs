using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
public class HoverDevice : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] float strength;
    [SerializeField] float damping;
    [Tooltip("Seberapa jauhnya hoverdevice dengan tanah")]
    [Range(0f, 100f)]
    [SerializeField] float distance;
    [SerializeField] Rigidbody rb;
    RaycastHit hit;
    float lasthitdistance;
    private void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 100))
        {
            if(hit.distance <= distance)rb.AddForceAtPosition(transform.up * HookeLaw(hit.distance), transform.position);
            else lasthitdistance = hit.distance;
        }
    }
    float HookeLaw(float hit_distance)
    {
        //k * x (Koefisien pegas) * jarak
        float Force = strength * (distance - hit_distance)/distance + damping * (lasthitdistance - hit_distance);
        //Force = Mathf.Clamp(Force, 0, Force);
        lasthitdistance = hit_distance;
        return Force;
    }
    void OnValidate()
    {
        strength = Mathf.Clamp(strength, 0f, strength);
    }
}
