using AlexeyVlasyuk.MultiplayerTest.Models;
using AlexeyVlasyuk.MultiplayerTest.Utilities;
using TMPro;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class LoseScreen : UIScreen
    {
        [SerializeField]
        private TMP_Text _coinsField;
        
        public void SetContent(int coinsGathered, int totalCoins)
        {
            _coinsField.text = $"Gathered {coinsGathered} of {totalCoins} coins";
        }
    }
}