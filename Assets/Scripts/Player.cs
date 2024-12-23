using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    bool _isGrounded;

    Animator _animator;
    Rigidbody _rigidbody;
    Collider _collider;
    // PlayerControls _input;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        // _playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        _isGrounded = Physics.OverlapBox(transform.position, new Vector2(_collider.bounds.size.x * 0.82f, 0.125f), Quaternion.identity, 1 << 0).Length > 0;
    }
}