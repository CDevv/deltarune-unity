using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Luminosity.IO;

public class CustomInput : BaseInput
{
    public override bool GetButtonDown(string buttonName)
    {
        return InputManager.GetButtonDown(buttonName);
    }
    public override float GetAxisRaw(string axisName)
    {
        return InputManager.GetAxisRaw(axisName);
    }
}
