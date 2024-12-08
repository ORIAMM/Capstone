using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerTP : MonoBehaviour
{
    [Header("Player Stat")]
    [SerializeField] private float initial_time = 300f;
    [SerializeField] private float DmgReduct = 0.5f;
    public float HealthPlayer;

    public static PlayerTP instance;

    public void Start()
    {
        instance = this;
        HealthPlayer = Time.time + initial_time;
    }

    private PlayerInputManager playerInputManager;

    public void Update()
    {
        StartTicking();
    }
    public void FloatToTimeConverse()
    {
        int totalSeconds = Mathf.CeilToInt(HealthPlayer); 
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        TimerText.text = $"{minutes:00}:{seconds:00}";
    }
    public TextMeshProUGUI TimerText;
    private Coroutine tickingCoroutine;
    public void StartTicking()
    {
        if (tickingCoroutine == null)
        {
            tickingCoroutine = StartCoroutine(TickingCoroutine());
        }
    }
    private IEnumerator TickingCoroutine()
    {
        while (HealthPlayer > 0)
        {
            yield return new WaitForSeconds(1f);
            HealthPlayer--;
            FloatToTimeConverse();
        }

        HealthPlayer = 0;
    }


}
