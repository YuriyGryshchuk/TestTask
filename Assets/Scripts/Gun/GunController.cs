using System.Collections.Generic;
using UnityEngine;

namespace Implementations.Game
{
    public class GunController : MonoBehaviour
    {
        [SerializeField] private LineRenderer _currentLineRenderer;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private int _positionCount = 20;

        [SerializeField] private Vector2 _axisSpeeds = Vector2.one;
        [SerializeField] private Vector2 _gunLimits = new Vector2(-20, 10f);
        [SerializeField] private Vector2 _gunBarrelLimits = new Vector2(0, 10f);
        [SerializeField] private float _maxPower = 25f;
        [SerializeField] private float _maxSpeed = 40f;
        [SerializeField] private float _minSpeed = 10f;
        [SerializeField] private Transform _gunTransform;
        [SerializeField] private Transform _gunBarrelTransform;

        [SerializeField] private Joystick _joystick;
        [SerializeField] private PowerSlider _powerSlider;
        [SerializeField] private CameraController _cameraController;

        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Animation _shootAnimation;

        private Vector2 _smoothedRotationDelta;
        private Vector2 _rotationOffset;

        private void Start()
        {
            _currentLineRenderer.positionCount = _positionCount;
        }

        private void OnEnable()
        {
            _joystick.OnPointerUpEvent += Fire;

            _rotationOffset.x = (_gunLimits.x + _gunLimits.y) / 2;
            _gunTransform.localRotation = Quaternion.AngleAxis(_rotationOffset.x, Vector3.up);

            _rotationOffset.y = (_gunBarrelLimits.x + _gunBarrelLimits.y) / 2;
            _gunBarrelTransform.localRotation = Quaternion.AngleAxis(_rotationOffset.y, Vector3.right);
        }

        private void OnDisable()
        {
            _joystick.OnPointerUpEvent -= Fire;
        }

        private void Update()
        {
            UpdateRotations(_joystick.Direction);
        }

        protected void LateUpdate()
        {
            float timeStep = 0.1f;

            for (int i = 0; i < _currentLineRenderer.positionCount; i++)
            {
                float time = i * timeStep;
                Vector3 position = _shootPoint.position + _shootPoint.forward * time * _maxPower * _powerSlider.PowerSliderValue;
                position.y += Physics.gravity.y / 2f * time * time;
                _currentLineRenderer.SetPosition(i, position);
            }
        }

        private void Fire()
        {
            Projectile projectile = Instantiate<Projectile>(_projectilePrefab, _shootPoint.position, Quaternion.identity);
            projectile.gameObject.SetActive(true);

            List<Vector3> points = new();

            float timeStep = 0.1f;

            for (int i = 0; i < _currentLineRenderer.positionCount; i++)
            {
                float time = i * timeStep;
                Vector3 position = _shootPoint.position + _shootPoint.forward * time * _maxPower * _powerSlider.PowerSliderValue;
                position.y += Physics.gravity.y / 2f * time * time;
                points.Add(position);
            }

            float speed = Mathf.Clamp(_maxSpeed * _powerSlider.PowerSliderValue, _minSpeed, _maxSpeed);
            float force = _maxPower * _powerSlider.PowerSliderValue;

            projectile.Initialize(points, speed, force, _positionCount);
            _cameraController.ShakeCamera(0.5f, 1, 3);
            _shootAnimation.Play();
        }

        public void UpdateRotations(Vector2 rotationDelta)
        {
            if(rotationDelta == Vector2.zero) return;
            _smoothedRotationDelta = Vector2.Lerp(_smoothedRotationDelta, rotationDelta, Time.deltaTime * 5f);
            _rotationOffset.x -= _smoothedRotationDelta.x * _axisSpeeds.x;
            _rotationOffset.y += _smoothedRotationDelta.y * _axisSpeeds.y;

            Vector3 horizonProjection = Vector3.ProjectOnPlane(transform.forward, Vector3.right);
            float horizonAngle = Vector3.SignedAngle(transform.forward, horizonProjection, transform.up) + _rotationOffset.x;
            float horizonClampAngle = Mathf.Clamp(horizonAngle, _gunLimits.x, _gunLimits.y);

            _rotationOffset.x -= horizonAngle - horizonClampAngle;
            _gunTransform.localRotation = Quaternion.AngleAxis(horizonClampAngle, Vector3.up);

            Vector3 verticalProjection = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            float verticalAngle = Vector3.SignedAngle(transform.forward, verticalProjection, transform.right) + _rotationOffset.y;
            float verticalClampAngle = Mathf.Clamp(verticalAngle, _gunBarrelLimits.x, _gunBarrelLimits.y);

            _rotationOffset.y -= verticalAngle - verticalClampAngle;
            _gunBarrelTransform.localRotation = Quaternion.AngleAxis(verticalAngle, Vector3.right);

        }
    }
}
