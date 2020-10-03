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
            startJump = totalInputs.startJump || Input.GetAxis("Jump") >= 0.25f,
            hasHorizontal = totalInputs.hasHorizontal || (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f),
            horizontal = totalInputs.horizontal + Input.GetAxis("Horizontal")
        };
        inputsThisFrame++;
    }

    public InputStruct GetInputs()
    {
        return new InputStruct()
        {
            startJump = totalInputs.startJump,
            hasHorizontal = totalInputs.hasHorizontal,
            horizontal = totalInputs.horizontal / inputsThisFrame
        };
    }

    public void ResetInputs()
    {
        inputsThisFrame = 0;
        totalInputs = new InputStruct()
        {
            startJump = false,
            hasHorizontal = false,
            horizontal = 0
        };
    }
}
