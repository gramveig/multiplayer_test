using System;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 1f;

        private Rigidbody2D _rigidBody;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public void UpdateCoord(float x, float y)
        {
            var direction = new Vector2(x, y);
            Vector2 position = _transform.position;
            _transform.position = position + direction * _speed * Time.deltaTime;
        }
    }
}