using Cinemachine;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera2 : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Camera cam;

    [Header("Combat Settings")]
    [SerializeField] float offset;
    [SerializeField] Vector3 size;
    [SerializeField] Vector2 targetLockOffset;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float X_panning_speed;
    [SerializeField] float Y_panning_speed;
    public float multiplier = 1;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform FacingDirection;
    [HideInInspector] public Transform PlayerMeshObject;
    [HideInInspector] public InputActionMap action;

    private Player2 player;
    private CombatHandler combat;

    float mouseX, mouseY;
    private Transform Target;
    public Transform target
    {
        get
        {
            return Target;
        }
        set
        {
            Target = value;
            animator.SetBool("isCombat", Target != null);
            if (Target)
            {
                player._CameraStyle = CameraStyle.Combat;
                player.modifierMultiplier = 0.5f;
            }
            else
            {
                player._CameraStyle = CameraStyle.Basic;
                player.modifierMultiplier = 1;
            }
        }
    }

    private void Awake()
    {
        combat = GetComponent<CombatHandler>();
        player = GetComponent<Player2>();
        cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
        cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
    }
    private void Update()
    {
        Vector3 Forward = cam.transform.forward;
        FacingDirection.forward = new Vector3(Forward.x, 0, Forward.z).normalized;

        if (Target)
        {
            CombatCamera();
        }
        else
        {
            BasicCamera();
        }

        cinemachineFreeLook.m_XAxis.m_InputAxisValue = mouseX * multiplier;
        cinemachineFreeLook.m_YAxis.m_InputAxisValue = mouseY * multiplier;
    }
    void BasicCamera()
    {
        if (combat.isBlocking && combat.isFall && combat.isDodging)
        {
            var Input = action.FindAction("Move").ReadValue<Vector2>();
            Vector3 inputDir = FacingDirection.forward * Input.y + FacingDirection.right * Input.x;
            if (inputDir != Vector3.zero) PlayerMeshObject.forward = Vector3.Slerp(PlayerMeshObject.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        var MouseInput = action.FindAction("Mouse").ReadValue<Vector2>();
        mouseX = MouseInput.x;
        mouseY = MouseInput.y;
    }
    void CombatCamera()
    {
        Vector2 viewPos = cam.WorldToViewportPoint(target.position);

        if (Vector3.Distance(target.position, transform.position) < minDistance) return;
        mouseX = (viewPos.x - 0.5f + targetLockOffset.x) * X_panning_speed;
        mouseY = (viewPos.y - 0.5f + targetLockOffset.y) * Y_panning_speed;
        Vector3 inputDir = new(cam.transform.forward.x, 0, cam.transform.forward.z);
        if(!player.Dodging)PlayerMeshObject.forward = Vector3.Slerp(PlayerMeshObject.forward, inputDir, Time.deltaTime * rotationSpeed);
    }

    public void GetTarget() => target = target ? null : FindClosest();
    private Transform FindClosest()
    {
        var hits = Physics.BoxCastAll(PlayerMeshObject.position, size, cam.transform.forward, PlayerMeshObject.rotation, maxDistance, layerMask).ToList();
        RaycastHit hit = hits.Find(x => x.collider.CompareTag("Enemy"));
        if (hit.collider)
        {
            return hit.collider.gameObject.transform;
        }
        return null;
    }
#if UNITY_EDITOR
    [SerializeField] Transform mesh;
    private void OnDrawGizmos()
    {
        if(mesh == null) return;
        Gizmos.DrawWireCube(mesh.position + mesh.forward * offset, size);
    }
#endif
}