using UnityEngine;
using UnityEngine.InputSystem;

public class ShipSmoothMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private PlayerControls _controls;

    private Camera _mainCamera;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _controls = new PlayerControls();
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _controls.Player.Move.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _movementInput = Vector2.zero;
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void FixedUpdate()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
            _smoothedMovementInput, 
            _movementInput, 
            ref _movementInputSmoothVelocity, 
            0.1f);

        Vector2 newPosition = _rigidbody.position + _smoothedMovementInput * _speed * Time.fixedDeltaTime;
        
        // Clamp the new position within the camera bounds
        Vector2 clampedPosition = ClampPositionToCameraBounds(newPosition);
        
        _rigidbody.MovePosition(clampedPosition);
    }

    private Vector2 ClampPositionToCameraBounds(Vector2 position)
    {
        Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(position);
        viewportPosition.x = Mathf.Clamp01(viewportPosition.x);
        viewportPosition.y = Mathf.Clamp01(viewportPosition.y);

        return _mainCamera.ViewportToWorldPoint(viewportPosition);
    }
}