using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private float decalLifetime = 120f; 

    private float _speed = 1.0f;
    private int _positionCount;
    private float _force;
    private List<Vector3> _points;
    private int _currentPointIndex = 0;
    private Transform _transform;
    private bool _isCollision;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_points == null || _points.Count == 0)
            return;

        if (MoveToNextPoint())
        {
            _currentPointIndex++;
            if (_currentPointIndex >= _points.Count)
            {
                //currentPointIndex = 0; 
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isCollision || !collision.gameObject.TryGetComponent<Environment>(out Environment environment)) return;
        _isCollision = true;

        if(environment.DecalTarget) SpawnDecal(collision.contacts[0].point, collision.contacts[0].normal);
        RefreshPoints(collision, environment);
    }

    private void SpawnDecal(Vector3 position, Vector3 normal)
    {
        GameObject decal = Instantiate(decalPrefab, position, Quaternion.LookRotation(normal));

        Destroy(decal, decalLifetime);
    }

    private void RefreshPoints(Collision collision, Environment environment)
    {
        Vector3 collisionNormal = collision.contacts[0].normal;

        Vector3 bounceDirection = Vector3.Reflect(_points[_currentPointIndex] - _transform.position, collisionNormal).normalized * environment.BounceForce;

        _points.Clear();

        float timeStep = 0.1f;

        float forse = _force /= 2;

        for (int i = 0; i < _positionCount; i++)
        {
            float time = i * timeStep;
            Vector3 position = transform.position + bounceDirection * time * forse;
            position.y += Physics.gravity.y / 2f * time * time;
            _points.Add(position);
        }

        _currentPointIndex = 0;
    }

    private bool MoveToNextPoint()
    {
        Vector3 direction = (_points[_currentPointIndex] - _transform.position).normalized;

        _transform.position += direction * _speed * Time.deltaTime;

        if (Vector3.Distance(_transform.position, _points[_currentPointIndex]) < 0.5f)
        {
            return true;
        }
        return false;
    }

    public void Initialize(List<Vector3> points, float speed, float force, int positionCount)
    {
        _positionCount = positionCount;
        _force = force;
        _points = points;
        _speed = speed;
        _transform.position = _points[0];
    }
}
