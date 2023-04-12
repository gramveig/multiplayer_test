using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CWaitBeforeRejoinLobby : PUN2ConnectionState
    {
        private float _timer;
        private bool _successTriggered;

        private const float RejoinLobbyTimeout = 2f;

        public P2CWaitBeforeRejoinLobby(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }

        public override void Start()
        {
            Debug.Log("PUN2: Waiting before trying to rejoin lobby");

            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("PUN2: not connected to server");
                OnServerDisconnect();
            }

            if (PhotonNetwork.InLobby)
            {
                Debug.LogError("PUN2: already in Lobby");
                OnSuccess();
            }

            _timer = 0;
            _successTriggered = false;
        }

        public override void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= RejoinLobbyTimeout && !_successTriggered)
            {
                OnSuccess();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("PUN2: Unexpectedly disconnected from Photon server. Cause: " + cause);
            OnServerDisconnect();
        }

        void OnSuccess()
        {
            _successTriggered = true;

            p2c.SetConnectionState(p2c.csJoiningLobby);
        }

        void OnServerDisconnect()
        {
            p2c.CallOnDisconnectEvent();
        }
    }
}