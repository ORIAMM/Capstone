using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CD_UI : MonoBehaviour
{
    public Animator animator;
    public TimeManager timeManager;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeManager.isStopped)
        {
            PlayCDAnim();
        }
    }
    public void PlayCDAnim()
    {
        animator.Play("Cooldown_anim");
    }
}
