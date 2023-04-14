using System;
using TMPro;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class PlayerName : MonoBehaviour
    {
        private TMP_Text _nameText;
        private Transform _transform;
        private Player _player;
        private Camera _cam;
        private string _name;
        
        private void Awake()
        {
            _nameText = GetComponent<TMP_Text>();
            _transform = transform;
            _cam = Camera.main;
        }

        private void Start()
        {
            _nameText.text = _name;
        }
        
        private void Update()
        {
            _transform.position = _cam.WorldToScreenPoint(_player.CachedTransform.position);
        }

        private void OnDestroy()
        {
            _player.OnPlayerDestroyed -= OnPlayerDestroyed;
        }

        public void Init(string name, Player player)
        {
            _name = name;
            _player = player;

            _player.OnPlayerDestroyed += OnPlayerDestroyed;
        }

        private void OnPlayerDestroyed()
        {
            Destroy(gameObject);
        }
    }
}