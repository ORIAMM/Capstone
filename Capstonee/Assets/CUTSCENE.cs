using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUTSCENE : MonoBehaviour
{
    public GameObject MAW;
    public Animator animatorPlayer;
    public Animator animatorMusuh;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) animatorPlayer.SetTrigger("jalankan");
        if (Input.GetKeyDown(KeyCode.K)) animatorPlayer.SetTrigger("jalankan2");
        if (Input.GetKeyDown(KeyCode.J))
        {
            MAW.SetActive(true);
            animatorMusuh.SetTrigger("jalankan");
        }
    }
}
