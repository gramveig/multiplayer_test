using Photon.Pun;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CWaitForPlayers : PUN2ConnectionState
    {
        private bool _successTriggered;
        
        public P2CWaitForPlayers(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }

        public override void Start()
        {
            _successTriggered = false;

            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("PUN2: not connected to server");
                p2c.CallOnDisconnectEvent();
                return;
            }

            if (PhotonNetwork.CurrentRoom == null)
            {
                Debug.Log("PUN2: current room not found");
                p2c.CallOnDisconnectEvent();
                return;
            }

            Debug.Log("PUN2: Waiting for players...");
            
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                OnSuccess();
            }
        }

        public override void Update()
        {
            if (_successTriggered)
            {
                return;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                OnSuccess();
            }
        }

        private void OnSuccess()
        {
            if (_successTriggered)
            {
                return;
            }

            Debug.Log($"PUN2: {PhotonNetwork.CurrentRoom.PlayerCount} players are in room. Proceeding to game");

            _successTriggered = true;
            p2c.CallOnOtherPlayersJoinedRoom();
        }
    }
}