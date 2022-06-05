using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Mirror;

namespace FPS.Controls
{
    public class PlayerCameraController : NetworkBehaviour
    {   
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private Transform playerCamera;

        private Controls controls;
        private Controls Controls
        {
            get
            {
                if (controls != null)
                {
                    return controls;
                }
                return controls = new Controls();
            }
        }

        private Vector2 lastMouseInput;
        private float xRotation = 0;

        public override void OnStartAuthority()
        {
            enabled = true;

            playerCamera.GetComponent<Camera>().enabled = true;
            playerCamera.GetComponent<AudioListener>().enabled = true;

            Controls.Player.Look.performed += ctx => SetLookRotation(ctx.ReadValue<Vector2>());
            Controls.Player.Look.canceled += ctx => ResetLookRotation();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable() => Controls.Enable();

        private void OnDisable() => Controls.Disable();
        
        private void Update() => HandleMouseInput();

        private void SetLookRotation(Vector2 mouseInput)
        {
            lastMouseInput = mouseInput;
        }

        private void ResetLookRotation()
        {
            lastMouseInput = Vector2.zero;
        }
        
        private void HandleMouseInput()
        {
            float xInput = lastMouseInput.x * mouseSensitivity * Time.deltaTime;
            float yInput = lastMouseInput.y * mouseSensitivity * Time.deltaTime;

            xRotation -= yInput;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(0f, xInput, 0f);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerCamera.position, playerCamera.position + playerCamera.forward * 2f);
        }
    }
}

