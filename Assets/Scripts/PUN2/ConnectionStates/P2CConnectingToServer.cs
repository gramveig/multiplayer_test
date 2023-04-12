using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CConnectingToServer : PUN2ConnectionState
    {
        public P2CConnectingToServer(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }

        public override void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                OnSuccess();
                return;
            }

            Debug.Log("PUN2: Connecting to Photon server");

            ConnectToServer();
        }

        void ConnectToServer()
        {
            PhotonNetwork.NickName = p2c.GetRandomPlayerName();
            PhotonNetwork.ConnectUsingSettings();
        }

        //called when connected to the Internet, but before connected to the server
        public override void OnConnected()
        {
            Debug.Log("PUN2: Connected to Intenet");
        }

        //called when connected to the server
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN2: Connected to Photon server");
            OnSuccess();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("PUN2: Failed to connect to Photon server. Cause: " + cause);
            p2c.CallOnDisconnectEvent();
        }

        void OnSuccess()
        {
            //p2c.SetConnectionState(p2c.csWaitBeforeJoinLobby);
        }
    }
}