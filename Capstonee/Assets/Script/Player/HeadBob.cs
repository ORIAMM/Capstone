using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeadBob : MonoBehaviour
{
    [Header("Bob Setting")]
    [Range(0.001f, 0.01f)]
    public float Amount = 0.002f;
    [Range(1f, 30f)]
    public float FrequencyWalk = 10.0f;
    [Range(1f, 30f)]
    public float FrequencySprint = 20.0f;
    [Range(10f, 100f)]
    public float Smooth = 10.0f;

    [SerializeField] float Frequency;

    Vector3 StartPos;

    [Header("References")]
    public PlayerMovement PlayerMovement;

    [Header("Footsteps")]
    public UnityEvent onFootStep;

    float Sin;
    bool StepTriggerd = false;

    void Start()
    {
        StartPos = transform.localPosition;

    }

    void Update()
    {
        if (PlayerMovement.state == PlayerMovement.MovementState.sprinting)
        {
            Frequency = FrequencySprint;
        }
        else if (PlayerMovement.state == PlayerMovement.MovementState.walking)
        {
            Frequency = FrequencyWalk;
        }

        CheckForHeadbobTrigger();
        StopHeadbob();


    }

    private void CheckForHeadbobTrigger()
    {
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;

        if (inputMagnitude > 0)
        {
            StartHeadBob();
        }
    }

    private Vector3 StartHeadBob()
    {

        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * Frequency) * Amount * 1.4f, Smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * Frequency / 2f) * Amount * 1.6f, Smooth * Time.deltaTime);
        transform.localPosition += pos;

        Sin = Mathf.Sin(Time.time * Frequency);

        if (Sin > 0.97f && StepTriggerd == false)
        {
            Debug.Log("tic");
            StepTriggerd = true;
            onFootStep.Invoke();
        }
        else if (StepTriggerd == true && Sin < -0.97f)
        {
            StepTriggerd = false;
        }



        return pos;
    }

    private void StopHeadbob()
    {
        if (transform.localPosition == StartPos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, StartPos, 1 * Time.deltaTime);
    }
}
