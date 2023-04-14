using System;
using Photon.Pun;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float _speedMovement = 10f;
        
        [SerializeField]
        private float _speedRotation = 200f;

        [SerializeField]
        private Cannon _cannon;

        private Transform _transform;
        private PhotonView _photonView;
        
        private void Awake()
        {
            _transform = transform;
            _photonView = GetComponent<PhotonView>();
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

        public void StartFire()
        {
            _photonView.RPC("OnNetworkStartFire", RpcTarget.All);
        }
        
        public void StopFire()
        {
            _photonView.RPC("OnNetworkStopFire", RpcTarget.All);
        }

        [PunRPC]
        private void OnNetworkStartFire()
        {
            _cannon.StartFire();
        }
        
        [PunRPC]
        private void OnNetworkStopFire()
        {
            _cannon.StopFire();
        }
    }
}