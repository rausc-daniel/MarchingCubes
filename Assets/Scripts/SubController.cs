using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubController : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float angularAcceleration;
    [SerializeField] private float maxTorque;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _rigidbody.AddForce(acceleration * Time.deltaTime * transform.forward);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _rigidbody.AddForce(acceleration * Time.deltaTime * -transform.forward);
        }

        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.AddTorque(transform.up * angularAcceleration);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.AddTorque(-transform.up * angularAcceleration);
        }

        if (Input.GetKey(KeyCode.E))
        {
            _rigidbody.AddForce(acceleration * Time.deltaTime * transform.up);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            _rigidbody.AddForce(acceleration * Time.deltaTime * -transform.up);
        }

        var currentVelocity = _rigidbody.velocity;
        _rigidbody.velocity = new Vector3(Mathf.Clamp(currentVelocity.x, -maxSpeed, maxSpeed),
            Mathf.Clamp(currentVelocity.y, -maxSpeed, maxSpeed),
            Mathf.Clamp(currentVelocity.z, -maxSpeed, maxSpeed));

        var currentAngularVelocity = _rigidbody.angularVelocity;
        _rigidbody.angularVelocity = new Vector3(Mathf.Clamp(currentAngularVelocity.x, -maxTorque, maxTorque),
            Mathf.Clamp(currentAngularVelocity.y, -maxTorque, maxTorque),
            Mathf.Clamp(currentAngularVelocity.z, -maxTorque, maxTorque));
    }
}