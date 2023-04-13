using AlexeyVlasyuk.MultiplayerTest.PUN2;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class DisconnectSceneManager : MonoBehaviour
    {
        protected void Start()
        {
            if (PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized)
            {
                PUN2Controller.Instance.AssureDisconnection();
            }
        }
        
        public void LoadFirstScene()
        {
            SceneManager.LoadScene("Loading");
        }
    }
}