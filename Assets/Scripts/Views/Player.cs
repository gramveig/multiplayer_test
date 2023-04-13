using System;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float _speedMovement = 10f;
        
        [SerializeField]
        private float _speedRotation = 10f;

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
            _transform.position = position + direction * _speedMovement * Time.deltaTime;
        }

        public void UpdateRotation(float x, float y)
        {
            float magn = new Vector2(x, y).magnitude;
            float angle = Mathf.Atan2(x, y ) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, -angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _speedRotation * Time.deltaTime * magn);
        }
    }
}