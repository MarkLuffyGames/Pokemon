using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private InputAction move;
    private Animator animator;

    public float moveSpeed  = 5f;
    public bool isMoving;
    public Vector3 moveDirection;
    public Vector3 lastDirection;
    public Vector3 newPosition;

    public LayerMask collisionLayer;
    public LayerMask pokemonLayer;

    public event Action OnPokemonEncountered;

    private void Start()
    {
        animator = GetComponent<Animator>();
        move = InputSystem.actions.FindAction("Move");
    }

    public void HandleUpdate()
    {
        GetInput();
        Move();
    }

    private void GetInput()
    {
        if (isMoving) return;

        Vector2 moveInput = move.ReadValue<Vector2>().normalized;

        moveDirection = OneDirectionMove.OneDirection(moveInput, lastDirection);

        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);
    }

    private void Move()
    {
        if(moveDirection != Vector3.zero && !isMoving)
        {
            StartCoroutine(MovePlayer());
        }
    }

    IEnumerator MovePlayer()
    {
        if(lastDirection == moveDirection)
        {
            newPosition = transform.position + moveDirection;
            if (PositionAvailable(newPosition))
            {
                newPosition = transform.position;
            }
            else
            {
                isMoving = true;
                animator.SetBool("IsMoving", isMoving);
                newPosition = transform.position + moveDirection;
                while (Vector2.Distance(transform.position, newPosition) > Mathf.Epsilon)
                {
                    transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }
                transform.position = newPosition;
                isMoving = false;
                animator.SetBool("IsMoving", isMoving);
                animator.SetFloat("LastH", lastDirection.x);
                animator.SetFloat("LastV", lastDirection.y);
                CheckPokemonEncounter();
            }
        }
        else
        {
            isMoving = true;
            lastDirection = moveDirection;
            animator.SetFloat("LastH", lastDirection.x);
            animator.SetFloat("LastV", lastDirection.y);
            yield return new WaitForSeconds(0.1f);
            isMoving = false;
        }

        

        
    }

    private bool PositionAvailable(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, 0.3f, collisionLayer);
    }

    private void CheckPokemonEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.3f, pokemonLayer))
        {
            if(Random.Range(0,100) < 15)
            {
                OnPokemonEncountered();
            }
        }
    }
}

public static class OneDirectionMove
{
    public static Vector2 OneDirection(Vector2 moveInput, Vector3 lastDirection)
    {
        if (moveInput == Vector2.zero)
        {
            return Vector2.zero;
        }
        else if (Mathf.Abs(moveInput.x) == Mathf.Abs(moveInput.y))
        {
            return new Vector2(lastDirection.x, lastDirection.y).normalized;
        }
        else
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                return new Vector2(moveInput.x, 0).normalized;
            }
            else
            {
                return new Vector2(0, moveInput.y).normalized;
            }
        }
    }
}
