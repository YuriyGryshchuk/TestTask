using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileDecal _decalPrefab;
    [SerializeField] private ProjectileExsplotion _explotionPrefab;
    [SerializeField] private float _decalLifetime = 120f; 
    [SerializeField] private float _explotionLifetime = 10f; 
    [SerializeField] private int _maxColition = 2; 

    private float _speed = 1.0f;
    private PullObjectService _pullObjectService;
    private int _positionCount;
    private float _force;
    private List<Vector3> _points;
    private Vector3 _direction;
    private int _currentPointIndex = 0;
    private Transform _transform;
    private Vector3 _startPosition;
    private bool _isCollision;
    private int _colisionCount;
    private float _time;

    private void Awake()
    {
        _transform = transform;
    }

    private void FixedUpdate()
    {
        _time += Time.fixedDeltaTime * _speed;

        Vector3 position = _startPosition + _direction * _time * _force;
        position.y += Physics.gravity.y / 2f * _time * _time;

        _transform.LookAt(position);

        _transform.position = position;

        if (transform.position.y <= -2)
        {
            SpawnExplotion(_transform.position);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<Environment>(out Environment environment)) return;

        _colisionCount++;
        if(_colisionCount >= _maxColition)
        {
            SpawnExplotion(_transform.position);
            gameObject.SetActive(false);
            return;
        }

        if(environment.DecalTarget) SpawnDecal(collision.collider.ClosestPoint(transform.position), collision.contacts[0].normal);

        RefreshPoints(collision, environment);
    }

    private async void SpawnDecal(Vector3 position, Vector3 normal)
    {
        ProjectileDecal decal = _pullObjectService.SpawnObject<ProjectileDecal>(_decalPrefab, position, Quaternion.LookRotation(normal));

        await UniTask.Delay(TimeSpan.FromSeconds(_decalLifetime));

        decal.gameObject.SetActive(false);
        Destroy(decal, _decalLifetime);
    }

    private async void SpawnExplotion(Vector3 position)
    {
        ProjectileExsplotion exsplotion = _pullObjectService.SpawnObject<ProjectileExsplotion>(_explotionPrefab, position, Quaternion.identity);

        await UniTask.Delay(TimeSpan.FromSeconds(_explotionLifetime));

        exsplotion.gameObject.SetActive(false);
    }

    private void RefreshPoints(Collision collision, Environment environment)
    {
        Vector3 collisionNormal = collision.contacts[0].normal;

        _direction = Vector3.Reflect(_transform.forward, collisionNormal).normalized * environment.BounceForce;

        _startPosition = _transform.position;

        _force /= 2;

        _time = 0;
    }

    public void Initialize(Vector3 direction, float speed, float force, int positionCount, PullObjectService pullObjectService)
    {
        _pullObjectService = pullObjectService;
        _positionCount = positionCount;
        _force = force;
        _direction = direction;
        _speed = speed;

        _time = 0;
        _startPosition = _transform.position;
        _colisionCount = 0;
    }
}
