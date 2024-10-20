using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        Basic, Combat, Topdown
    }
    [Header("Player Rotation Settings")]
    [Tooltip("The Look direction the player is looking at")]
    public Transform Player;
    [Space(3)]
    public Transform Orientation;
    public Transform PlayerMeshObject;

    [Space(30)]
    public CameraMode currentCameraMode;
    [HideInInspector] public PlayerInput input;

    public void Start()
    {
        ToggleCursor(false);
    }
    public void Update()
    {
        Vector3 viewDir = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        Orientation.forward = viewDir.normalized;
    }
    public void ToggleCursor(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isVisible;
    }
}
