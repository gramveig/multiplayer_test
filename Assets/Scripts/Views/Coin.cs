using Photon.Pun;
using UnityEngine;
using Action = System.Action;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class Coin : MonoBehaviour
    {
        private Action _onPicked;
        
        public void Init(Action onPicked)
        {
            _onPicked = onPicked;
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                _onPicked?.Invoke();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}