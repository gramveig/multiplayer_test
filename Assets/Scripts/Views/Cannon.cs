using UnityEngine;
using AlexeyVlasyuk.MultiplayerTest.Utilities;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField]
        private Projectile _projectilePrefab;

        [SerializeField]
        private float _fireSpeed = 10;
        
        private Transform _transform;
        private ObjectPool<Projectile> _projectilePool;
        private float _fireTimer;
        private bool _isFiring;

        private void Awake()
        {
            _transform = transform;
            _projectilePool = new ObjectPool<Projectile>(InstantiateProjectile);
            _projectilePool.Prefetch();
        }

        private void Update()
        {
            _fireTimer += Time.deltaTime;
            if (_isFiring && _fireTimer >= 1f / _fireSpeed)
            {
                FireOnce();
                _fireTimer = 0;
            }
        }

        private Projectile InstantiateProjectile()
        {
            var projectile = Instantiate(_projectilePrefab, IniPos, _transform.rotation);
            projectile.Init(_projectilePool);

            return projectile;
        }

        public void StartFire()
        {
            _isFiring = true;
        }

        public void StopFire()
        {
            _isFiring = false;
        }

        private void FireOnce()
        {
            var projectile = _projectilePool.Draw();
            projectile.CachedTransform.position = IniPos;
            projectile.CachedTransform.localRotation = _transform.rotation;
        }

        private Vector3 IniPos => _transform.position
                                  + (_transform.rotation * Vector3.up) * 0.75f; //shift from the barrel's center, in order not to touch parent player's collider
    }
}