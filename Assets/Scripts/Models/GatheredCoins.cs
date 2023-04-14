using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Models
{
    public struct GatheredCoins
    {
        private readonly int _total;
        private int _gathered;

        public GatheredCoins(int total)
        {
            _total = total;
            _gathered = total;
        }

        public void Add()
        {
            _gathered++;
        }

        public float Rate => Mathf.Clamp((float) _gathered / _total, 0, 1f);
    }
}