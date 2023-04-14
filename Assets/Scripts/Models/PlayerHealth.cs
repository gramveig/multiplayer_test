using AlexeyVlasyuk.MultiplayerTest.Interfaces;

namespace AlexeyVlasyuk.MultiplayerTest.Models
{
    public struct PlayerHealth
    {
        private readonly float _total;
        private float _current;

        public PlayerHealth(float total)
        {
            _total = total;
            _current = total;
        }

        public void AddDamage(float damage)
        {
            _current -= damage;
        }
        
        public float Rate
        {
            get
            {
                var h = _current >= 0 ? _current : 0;
                return h / _total;
            }
        }
    }
}