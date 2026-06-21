using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoverObject : MonoBehaviour
{
    [Header("_thrusters")]
    [SerializeField] private List<Transform> _thruster = new();

    [Header("Hover Settings")]
    [SerializeField] private float _hoverHeight = 2.5f;
    [SerializeField] private float _floatingPower = 50.0f;
    [SerializeField] private float _dampingFactor = 10.0f;
    [SerializeField] private LayerMask _groundLayer; 

    private Rigidbody _rigidBody;
    private bool _enableHover = false;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (_enableHover)
        {
            int count = _thruster.Count;

            for (int i = 0; i < count; i++)
            {
                if (Physics.Raycast(_thruster[i].position, Vector3.down, out RaycastHit hit, _hoverHeight, _groundLayer))
                {
                    float difference = _hoverHeight - hit.distance;
                    Vector3 hoverForce = Vector3.up * _floatingPower * difference;
                    Vector3 dampingForce = -_rigidBody.GetPointVelocity(_thruster[i].position) * _dampingFactor;
                    _rigidBody.AddForceAtPosition(hoverForce + dampingForce, _thruster[i].position, ForceMode.Force);
                }
            }
        }
    }

    // public void Initialize(List<Transform> newFloaters) => floaters = newFloaters;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (Transform _thruster in _thruster)
        {
            if (_thruster != null) Gizmos.DrawLine(_thruster.position, _thruster.position + (Vector3.down * _hoverHeight));
        }
    }

    public void EnableHover(bool tf)
    {
        _enableHover = tf;
    }
}