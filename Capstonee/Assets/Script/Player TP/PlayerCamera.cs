using Cinemachine;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] private CameraStyle _CameraStyle;
    [SerializeField] private float rotationSpeed;

    [Header("Combat Settings")]
    [SerializeField] float radius;
    [SerializeField] Vector2 targetLockOffset;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float panning_speed;
    //public Action CameraLogic;

/*    [Header("Equip")]
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject swordSheath;*/

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform FacingDirection;
    [HideInInspector] public Transform PlayerMeshObject;
    [HideInInspector] public PlayerControls input;
    private Camera cam;

    private Player player;
    float mouseX, mouseY;
    private Transform Target;
    private bool isEquipped;
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
        }
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        cam = Camera.main;
        cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
        cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
    }
    private void Update()
    {
        Vector3 Forward = cam.transform.forward;
        FacingDirection.forward = new Vector3(Forward.x, 0, Forward.z).normalized;

        //CameraLogic?.Invoke();
        if (target)
        {
            CombatCamera();
        }
        else
        {
            BasicCamera();
        }
        cinemachineFreeLook.m_XAxis.m_InputAxisValue = mouseX;
        cinemachineFreeLook.m_YAxis.m_InputAxisValue = mouseY;
    }
    void BasicCamera()
    {
        var Input = input.Movement.Move.ReadValue<Vector2>();
        Vector3 inputDir = FacingDirection.forward * Input.y + FacingDirection.right * Input.x;
        if (inputDir != Vector3.zero) PlayerMeshObject.forward = Vector3.Slerp(PlayerMeshObject.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        var MouseInput = input.Controls.Mouse.ReadValue<Vector2>();
        mouseX = MouseInput.x;
        mouseY = MouseInput.y;

        player._CameraStyle = CameraStyle.Basic;
        //_CameraStyle = CameraStyle.Basic;
    }
    void CombatCamera()
    {
        Vector2 viewPos = cam.WorldToViewportPoint(target.position);
        //float diff = Vector2.Distance(viewPos, Vector2.one * 0.5f);
        //float pan_speed = Mathf.Pow(panning_speed, 1 - diff);
        //if (aimIcon) aimIcon.transform.position = mainCamera.WorldToScreenPoint(target.position);
        if (Vector3.Distance(target.position, transform.position) < minDistance) return;
        mouseX = (viewPos.x - 0.5f + targetLockOffset.x) * panning_speed;
        mouseY = (viewPos.y - 0.5f + targetLockOffset.y) * panning_speed;
        Vector3 inputDir = new(cam.transform.forward.x, 0, cam.transform.forward.z);
        PlayerMeshObject.forward = Vector3.Slerp(PlayerMeshObject.forward, inputDir, Time.deltaTime * rotationSpeed);

        player._CameraStyle = CameraStyle.Combat;
        //_CameraStyle = CameraStyle.Combat;
    }

/*    public void Equip()
    {
        if (!isEquipped)
        {
            sword.SetActive(true);
            swordSheath.SetActive(false);
            isEquipped = !isEquipped;
        } else
        {
            sword.SetActive(false);
            swordSheath.SetActive(true);
            isEquipped = !isEquipped;
        }
    }*/


    //public void SetCameraStyle(CameraStyle cameraStyle)
    //{
    //    _CameraStyle = cameraStyle;
    //    CameraLogic = GetCameraLogic(cameraStyle);
    //    if (cameraStyle == CameraStyle.Combat) input.Controls.Target.performed += (val) => GetTarget();
    //    else input.Controls.Target.performed -= (val) => GetTarget();
    //}
    //public Action GetCameraLogic(CameraStyle cameraStyle)
    //{
    //    return cameraStyle switch
    //    {
    //        CameraStyle.Basic => BasicCamera,
    //        CameraStyle.Combat => CombatCamera,
    //        _ => throw new NotImplementedException()
    //    };
    //}
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(cam.transform.position, radius);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(cam.transform.position, cam.transform.position + Vector3.forward * maxDistance);
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(cam.transform.position, cam.transform.position + Vector3.right * minDistance);
    //}
    public void GetTarget() => target = target ? null : FindClosest().transform;
    private GameObject FindClosest()
    {
        var hits = Physics.SphereCastAll(cam.transform.position, radius, cam.transform.forward, maxDistance, layerMask).ToList();
        GameObject hit = hits.Find(x => x.collider.CompareTag("Enemy")).collider.gameObject;
        return hit.GetComponent<Collider>().gameObject;
    }

}
