using AlexeyVlasyuk.MultiplayerTest.PUN2;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest
{
    public class LoadingSceneManager : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.WaitUntil(() => PUN2Controller.Instance != null && PUN2Controller.Instance.IsInitialized);

            PUN2Controller.Instance.ConectToServer();
        }
    }
}