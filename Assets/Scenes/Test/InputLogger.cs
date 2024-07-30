using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLogger : MonoBehaviour
{
    [SerializeField] private Vector2 movementLog;
    [SerializeField] private Vector2 rotationLog;

    private void Update()
    {
        movementLog = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rotationLog = new Vector2(Input.GetAxis("RotateHorizontal"), Input.GetAxis("RotateVertical"));
    }
}