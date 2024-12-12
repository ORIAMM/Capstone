using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
public class PlayerManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField]
    private List<Transform> startingPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject prefabHealth;
    [SerializeField] private Transform PanelHealth;
    [SerializeField] private Transform MultiDeath;
    [SerializeField] private GameObject prefabDeath;
    [SerializeField] private GameObject info;

    private PlayerInputManager playerInputManager;

    public int CountPlayer;


    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        CountPlayer++;
        Debug.Log("Add");
        players.Add(player);

        Transform playerParent = player.transform.parent;
        playerParent.position = startingPoints[players.Count - 1].position;

        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

        cam.gameObject.SetActive(false);
        playerUI.SetActive(true);
        info.SetActive(false);

        GameObject InstantiateHealth = Instantiate(prefabHealth, PanelHealth);
        if (InstantiateHealth == null)
        {
            Debug.LogError("Failed to instantiate prefabHealth!");
            return;
        }

        GameObject InstantiateDeath = Instantiate(prefabDeath, MultiDeath);

        var PLAYER = player.GetComponentInParent<Player2>();
        if (PLAYER == null)
        {
            Debug.LogError("Failed to instantiate prefabHealth!");
            return;
        }

        Slider slider = InstantiateHealth.GetComponentInChildren<Slider>();
        TextMeshProUGUI timeText = InstantiateHealth.GetComponentInChildren<TextMeshProUGUI>();
        if (slider == null || timeText == null)
        {
            Debug.LogError("Health UI prefab missing required components (Slider or TextMeshProUGUI).");
            return;
        }


        if (PLAYER != null)
        {
            PLAYER.HealthSlider = InstantiateHealth.GetComponentInChildren<Slider>();
            PLAYER.time = InstantiateHealth.GetComponentInChildren<TextMeshProUGUI>();
            PLAYER.DeathPanel = InstantiateDeath;

            PLAYER.HealthSlider.maxValue = PLAYER.HealthPlayer;
            PLAYER.HealthSlider.value = PLAYER.TempHealth;


        }


    }
}
