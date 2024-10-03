using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseState
{
    /// <summary>
    /// dont forget to add the main data to this
    /// </summary>
    public BaseState() { }
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnLogic();
}
