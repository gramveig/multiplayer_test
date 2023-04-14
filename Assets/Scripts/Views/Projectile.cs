using UnityEngine;
using System;
using AlexeyVlasyuk.MultiplayerTest.Utilities;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float _damage = 1f;
        
        [SerializeField]
        private float _speed = 100f;

        private Transform _transform;
        private ObjectPool<Projectile> _pool;

        private void Awake()
        {
            _transform = transform;
        }

        public void Init(ObjectPool<Projectile> pool)
        {
            _pool = pool;
        }

        public Transform CachedTransform => _transform;
        
        private void Update() 
        {
            _transform.Translate(_transform.up * _speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                var player = col.GetComponent<Player>();
                if (player == null)
                {
                    Debug.LogError("Object tagged 'player' has no Player component on it");
                    return;
                }

                if (player.CachedPhotonView.IsMine)
                {
                    GameSceneManager.Instance.OnPlayerHit(_damage);
                }

                ReturnToPool();
            }
            else if (col.CompareTag("Border"))
            {
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            _pool.Return(this);
        }
    }
}