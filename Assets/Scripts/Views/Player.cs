using System;
using Photon.Pun;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Player : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField]
        private float _healthPoints = 100;
        
        [SerializeField]
        private float _speedMovement = 10f;
        
        [SerializeField]
        private float _speedRotation = 200f;

        [SerializeField]
        private Cannon _cannon;

        private Transform _transform;
        private PhotonView _photonView;

        public event Action OnPlayerDestroyed;

        private void Awake()
        {
            _transform = transform;
            _photonView = GetComponent<PhotonView>();
        }

        private void OnDestroy()
        {
            OnPlayerDestroyed?.Invoke();
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

        public Transform CachedTransform => _transform;
        public PhotonView CachedPhotonView => _photonView;
        public float TotalHealth => _healthPoints;
        
        public void StartFire()
        {
            _photonView.RPC("OnNetworkStartFire", RpcTarget.All);
        }
        
        public void StopFire()
        {
            _photonView.RPC("OnNetworkStopFire", RpcTarget.All);
        }

        public void OnCoinPicked()
        {
            //have to call GameSceneManager by instance here, as we cannot pass
            //the reference to event from it to the network instantiated object
            GameSceneManager.Instance.OnCoinPicked();
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            var color = GameSceneManager.Instance.GetPlayerColor(info.Sender.ActorNumber);
            GameSceneManager.Instance.OnPlayerInstantiated(this, info.Sender.NickName, color);
            ChangeColor(color);
        }

        public void NetworkDestroy()
        {
            if (_photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                _photonView.RPC("OnNetworkDestroy", RpcTarget.MasterClient);
            }
        }

        private void ChangeColor(Color color)
        {
            SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in srs)
            {
                sr.color = color;
            }
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

        [PunRPC]
        private void OnNetworkDestroy()
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}