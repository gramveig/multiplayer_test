using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CInGame : PUN2ConnectionState
    {
        private bool _otherPlayersJoinedTriggered;
        
        public P2CInGame(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }
        
        public override void Start()
        {
            _otherPlayersJoinedTriggered = false;

            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("PUN2: not connected to server");
                OnDisconnect();
                return;
            }

            if (PhotonNetwork.CurrentRoom == null)
            {
                Debug.Log("PUN2: current room is null");
                OnDisconnect();
                return;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                OnOtherPlayersJoinedRoom();
            }
        }

        public override void Update()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("PUN2: not connected to server");
                OnDisconnect();
                return;
            }
            
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && !_otherPlayersJoinedTriggered)
            {
                OnOtherPlayersJoinedRoom();
            }
        }

        private void OnDisconnect()
        {
            p2c.CallOnDisconnectEvent();
            p2c.SetConnectionState(p2c.csDisconnected);
        }

        private void OnOtherPlayersJoinedRoom()
        {
            if (_otherPlayersJoinedTriggered)
            {
                return;
            }

            _otherPlayersJoinedTriggered = true;

            p2c.CallOnOtherPlayersJoinedRoom();
        }
    }
}