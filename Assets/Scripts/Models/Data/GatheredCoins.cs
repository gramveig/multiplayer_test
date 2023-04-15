using UnityEngine;

namespace AlexeyVlasyuk.MultiplayerTest.Models
{
    public struct GatheredCoins
    {
        public readonly int total;
        public int gathered;

        public GatheredCoins(int total)
        {
            this.total = total;
            gathered = 0;
        }

        public void Add()
        {
            gathered++;
        }

        public float Rate => Mathf.Clamp((float) gathered / total, 0, 1f);
    }
}