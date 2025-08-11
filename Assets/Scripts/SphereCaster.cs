using UnityEngine;

public class SphereCaster {
    private Vector3 _origin;
    private Vector3 _direction;
    private float _radius;
    private float _distance;
    private readonly int _layerMask;
    private RaycastHit _hitInfo;
    
    public SphereCaster(Vector3 origin, Vector3 direction, float radius, float distance, int layerMask) {
        _origin = origin;
        _direction = direction;
        _radius = radius;
        _distance = distance;
        _layerMask = layerMask;
    }
    
    public void Cast() => Physics.SphereCast(_origin, _radius, _direction, out _hitInfo, _distance, _layerMask);

    public void GizmosDebug() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_origin, _radius);
        Gizmos.DrawWireSphere(_origin + _direction.normalized * _distance, _radius);
        Gizmos.DrawLine(_origin, _origin + _direction.normalized * _distance);
    }

    public void SetOrigin(Vector3 origin) => _origin = origin;
    public void SetDirection(Vector3 direction) => _direction = direction;
    public void SetRadius(float radius) => _radius = radius;
    public void SetDistance(float distance) => _distance = distance;
    public bool HasHitSomething() => _hitInfo.transform != null;
    public Transform GetHitTransform() => _hitInfo.transform;
    public Vector3 GetHitPoint() => _hitInfo.point;
    public Vector3 GetHitNormal() => _hitInfo.normal;
}
