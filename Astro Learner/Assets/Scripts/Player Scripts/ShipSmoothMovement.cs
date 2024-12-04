using UnityEngine;
using UnityEngine.InputSystem;

public class ShipSmoothMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f; // Base movement speed
    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private PlayerControls _controls;
    private Player player; // Reference to Player script

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _controls = new PlayerControls();
        player = FindObjectOfType<Player>(); // Link to Player script
        if (player != null)
        {
            UpdateSpeed(player.GetMovementSpeed()); // Sync movement speed with Player's speed
        }
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Player.Move.performed += OnMove;
        _controls.Player.Move.canceled += OnMove; // Reset movement when input stops
    }

    private void OnDisable()
    {
        _controls.Player.Move.performed -= OnMove;
        _controls.Player.Move.canceled -= OnMove;
        _controls.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _movementInput * _speed;
    }

    public void UpdateSpeed(float newSpeed)
    {
        _speed = Mathf.Max(0.1f, newSpeed);
        Debug.Log($"Movement speed updated: {_speed}");
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public void ResetSpeed()
    {
        _speed = 5.0f; // Reset to default speed
        Debug.Log("Movement speed reset to default.");
    }
}
