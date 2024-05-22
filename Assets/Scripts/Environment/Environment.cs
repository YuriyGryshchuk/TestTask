using UnityEngine;

public class Environment : MonoBehaviour
{
    [SerializeField] private float _bounceForce = 2.0f;

    [SerializeField] private bool _decalTarget;

    public float BounceForce => _bounceForce;
    public bool DecalTarget => _decalTarget;
}
