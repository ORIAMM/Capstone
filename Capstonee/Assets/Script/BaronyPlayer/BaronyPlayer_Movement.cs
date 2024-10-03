using UnityEngine;


public class Enemy : MonoBehaviour
{
    //ada movementnya sendiri
    //rolenya disini tuh cmn buat attack sama statnya
    //ada data statsnya
    //public DATA statsnya;
    public ThingMove role;

    void Awake()
    {
        //role.Set(statsnya);
    }
}
public class ThingMove : Entity
{
    
}
public class BaronyPlayer_Movement : MonoBehaviour
{
    public Player input;

    private void Awake()
    {
        input = new();
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }
    void Start()
    {
    }
}