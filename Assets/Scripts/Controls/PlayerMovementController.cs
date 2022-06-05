using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace FPS.Controls
{
    public class PlayerMovementController : NetworkBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private float movementSpeed;

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


        private Vector2 movementInput;

        public override void OnStartAuthority()
        {
            enabled = true;

            Controls.Player.Move.performed += ctx => SetMovementInput(ctx.ReadValue<Vector2>());
            Controls.Player.Move.canceled += ctx => ResetMovementInput();
        }

        private void OnEnable() => Controls.Enable();

        private void OnDisable() => Controls.Disable();

        private void Update() => HandleMovementInput();

        private void SetMovementInput(Vector2 movementInput)
        {
            this.movementInput = movementInput;
        }

        private void ResetMovementInput()
        {
            movementInput = Vector2.zero;
        }

        private void HandleMovementInput()
        {
            Vector3 movementVector = transform.forward * movementInput.y + transform.right * movementInput.x;
            movementVector.y = 0;

            controller.Move(movementVector * movementSpeed * Time.deltaTime);
        }
    }
}

