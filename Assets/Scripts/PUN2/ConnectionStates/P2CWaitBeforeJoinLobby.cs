using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    //have to wait some time before joining the lobby,
    //otherwise often getting "Trying to connect to Lobby while Authenticating" error
    public class P2CWaitBeforeJoinLobby : PUN2ConnectionState
    {
        private float _timer;
        private bool _successTriggered;

        private const float JoinLobbyTimeout = 1f;

        public P2CWaitBeforeJoinLobby(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }

        public override void Start()
        {
            Debug.Log("PUN2: Timing out before connecting to Lobby");

            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("PUN2: not connected to server");
                OnFailure();
                return;
            }

            if (PhotonNetwork.InLobby)
            {
                OnSuccess();
                return;
            }

            _timer = 0;
            _successTriggered = false;
        }

        public override void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= JoinLobbyTimeout && !_successTriggered)
            {
                OnSuccess();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("PUN2: Unexpectedly disconnected from Photon server. Cause: " + cause);
            OnFailure();
        }

        void OnSuccess()
        {
            _successTriggered = true;

            //p2c.SetConnectionState(p2c.csJoiningLobby);
        }

        void OnFailure()
        {
            p2c.CallOnDisconnectEvent();
        }
    }
}