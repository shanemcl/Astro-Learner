using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class ShipSmoothMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
            _smoothedMovementInput, 
            _movementInput, 
            ref _movementInputSmoothVelocity, 
            0.1f);
        
        _rigidbody.velocity = _smoothedMovementInput * _speed;
    }
    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }
    
}

