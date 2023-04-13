using Photon.Pun;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CInGame : PUN2ConnectionState
    {
        public P2CInGame(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }
        
        public override void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("PUN2: not connected to server");
                OnDisconnect();
                return;
            }
        }

        public override void Update()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("PUN2: not connected to server");
                OnDisconnect();
            }
        }

        private void OnDisconnect()
        {
            p2c.CallOnDisconnectEvent();
        }
    }
}