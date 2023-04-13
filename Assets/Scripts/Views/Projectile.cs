using UnityEngine;
using System;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float _damage;

        private Action<float> _onHit;

        public void Init(Action<float> onHit)
        {
            _onHit = onHit;
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _onHit?.Invoke(_damage);
                Destroy(gameObject);
            }
        }
    }
}