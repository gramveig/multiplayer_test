using Photon.Pun;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.PUN2.ConnectionStates
{
    public class P2CDisconnected : PUN2ConnectionState
    {
        public P2CDisconnected(PUN2Controller p2c)
        {
            this.p2c = p2c;
        }

        public override void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("PUN2: disconnecting from Photon network");
                PhotonNetwork.Disconnect();
            }

            p2c.ClearCachedRoomList();
        }
    }
}