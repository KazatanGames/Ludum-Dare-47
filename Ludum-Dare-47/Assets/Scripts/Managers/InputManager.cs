using UnityEngine;
using System.Collections;

public class InputManager
{
    protected InputStruct totalInputs;
    protected int inputsThisFrame;

    public InputManager()
    {
        ResetInputs();
    }

    public void UpdateInputs()
    {
        totalInputs =  new InputStruct()
        {
            hasVertical = totalInputs.hasVertical || Input.GetAxis("Jump") >= 0.25f,
            hasHorizontal = totalInputs.hasHorizontal || (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f),
            horizontal = totalInputs.horizontal + Input.GetAxis("Horizontal"),
            vertical = totalInputs.vertical + Input.GetAxis("Jump")
        };
        inputsThisFrame++;
    }

    public InputStruct GetInputs()
    {
        return new InputStruct()
        {
            hasVertical = totalInputs.hasVertical,
            hasHorizontal = totalInputs.hasHorizontal,
            horizontal = totalInputs.horizontal / inputsThisFrame,
            vertical = totalInputs.vertical / inputsThisFrame
        };
    }

    public void ResetInputs()
    {
        inputsThisFrame = 0;
        totalInputs = new InputStruct()
        {
            hasVertical = false,
            hasHorizontal = false,
            horizontal = 0,
            vertical = 0
        };
    }
}
