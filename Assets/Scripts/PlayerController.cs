using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShapeMovement))]
public class PlayerController : MonoBehaviour
{
    private InputActions inputActions;
    private ShapeMovement movement;
    private ShapeAttack attack;

    private InputAction attackAction;
    private InputAction jumpAction;
    private InputAction dashAction;


    void Awake()
    {
        inputActions = InputManager.Instance.InputActions;
        jumpAction = inputActions.Player.Jump;
        dashAction = inputActions.Player.Dash;
        attackAction = inputActions.Player.Attack;

        movement = GetComponent<ShapeMovement>();
        attack = GetComponent<ShapeAttack>();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        attackAction.performed += OnAttack;
        jumpAction.performed += OnJump;
        dashAction.performed += OnDash;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
        attackAction.performed -= OnAttack;
        jumpAction.performed -= OnJump;
        dashAction.performed -= OnDash;
    }

    void Update()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        movement.SetMoveInput(moveInput);
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        attack.Attack();
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        movement.Jump();
    }

    void OnDash(InputAction.CallbackContext ctx)
    {
        movement.Dash();
    }
}
