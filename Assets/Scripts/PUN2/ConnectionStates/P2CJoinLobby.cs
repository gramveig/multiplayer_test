using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CJoinLobby : PUN2ConnectionState
    {
        public P2CJoinLobby(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }

        public override void Start()
        {
            Debug.Log("PUN2: Joining Lobby");

            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("PUN2: not connected to server");
                OnServerDisconnect();
                return;
            }

            if (PhotonNetwork.InLobby)
            {
                Debug.LogError("PUN2: already in Lobby");
                OnSuccess();
                return;
            }

            PhotonNetwork.JoinLobby(p2c.customLobby);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("PUN2: Unexpectedly disconnected from Photon server. Cause: " + cause);
            OnServerDisconnect();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("PUN2: Successfully joined Lobby");
            OnSuccess();
        }

        public override void OnLeftLobby()
        {
            Debug.Log("PUN2: Failed to join Lobby");
            OnFailure();
        }

        void OnSuccess()
        {
            p2c.SetConnectionState(p2c.csInLobby);
        }

        void OnFailure()
        {
            p2c.SetConnectionState(p2c.csWaitBeforeRejoinLobby);
        }

        void OnServerDisconnect()
        {
            p2c.CallOnDisconnectEvent();
            p2c.SetConnectionState(p2c.csDisconnected);
        }
    }
}