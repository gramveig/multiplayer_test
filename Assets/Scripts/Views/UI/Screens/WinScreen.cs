using AlexeyVlasyuk.MultiplayerTest.Models;
using AlexeyVlasyuk.MultiplayerTest.Utilities;
using TMPro;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class WinScreen : UIScreen
    {
        [SerializeField]
        private TMP_Text _playerNameField;

        [SerializeField]
        private TMP_Text _coinsField;
        
        public void SetContent(string playerName, Color playerColor, int coinsGathered, int totalCoins)
        {
            _playerNameField.text = "Player: " + playerName;
            _playerNameField.color = playerColor;
            _coinsField.text = $"Gathered {coinsGathered} of {totalCoins} coins";
        }
    }
}