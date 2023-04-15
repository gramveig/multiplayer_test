using AlexeyVlasyuk.MultiplayerTest.Models;
using TMPro;
using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class EndGameScreen : UIScreen
    {
        [SerializeField]
        private TMP_Text _playerNameField;

        [SerializeField]
        private TMP_Text _coinsField;
        
        public void SetContent(string playerName, GatheredCoins coins)
        {
            _playerNameField.text = "Winning player: " + playerName;
            _coinsField.text = $"You gathered: {coins.gathered} of {coins.total}";
        }
    }
}