using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    private Vector3 lastInteractionDir;

    private void Update()
    {

        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleMovement()
    {

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 move = new Vector3(inputVector.x, 0f, inputVector.y);

        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, move, moveDistance);

        if (!canMove)
        {
            Vector3 moveX = new Vector3(move.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveX, moveDistance);

            if (canMove)
            {
                move = moveX;
            }
            else
            {
                Vector3 moveZ = new Vector3(0, 0, move.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveZ, moveDistance);

                if (canMove)
                {
                    move = moveZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += move * moveDistance;
        }

        isWalking = move != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * 10f);
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 move = new Vector3(inputVector.x, 0f, inputVector.y);

        if(move != Vector3.zero)
        {
            lastInteractionDir = move;
        }

        float interactDistance = 2f;

        if(Physics.Raycast(transform.position, lastInteractionDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if(raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                //Has ClearCounter
                clearCounter.Interact();
            }

        }
    }
}
