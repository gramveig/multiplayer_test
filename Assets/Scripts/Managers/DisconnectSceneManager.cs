using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class DisconnectSceneManager : MonoBehaviour
    {
        public void LoadFirstScene()
        {
            SceneManager.LoadScene("Loading");
        }
    }
}