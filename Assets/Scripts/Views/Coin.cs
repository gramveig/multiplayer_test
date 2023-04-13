using System;
using Photon.Pun;
using UnityEngine;
using Action = System.Action;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Coin : MonoBehaviour
    {
        private Action _onPicked;
        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }
        
        public void Init(Action onPicked)
        {
            _onPicked = onPicked;
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _onPicked?.Invoke();
                NetworkDestroy();
                PhotonNetwork.Destroy(gameObject);
            }
        }

        void NetworkDestroy()
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

        [PunRPC]
        private void OnNetworkDestroy()
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}