using System.Collections.Generic;
using UnityEngine;
using AlexeyVlasyuk.MultiplayerTest.Interfaces;

namespace AlexeyVlasyuk.MultiplayerTest.Models
{
    public class GameModel : IObservable<PlayerHealth>, IObservable<GatheredCoins>
    {
        private PlayerHealth _playerHealth;
        private GatheredCoins _gatheredCoins;
        private List<IObserver<PlayerHealth>> _healthObservers = new();
        private List<IObserver<GatheredCoins>> _coinObservers = new();

        public GameModel(float totalPlayerHealth, int totalCoins)
        {
            _playerHealth = new PlayerHealth(totalPlayerHealth);
            _gatheredCoins = new GatheredCoins(totalCoins);
        }
        
        public void AddDamageToPlayer(float damage)
        {
            if (damage == 0)
            {
                return;
            }

            if (damage < 0)
            {
                Debug.LogError("Trying to add negative damage to player");
                return;
            }

            _playerHealth.AddDamage(damage);
            foreach (var observer in _healthObservers)
            {
                observer.Notify(_playerHealth);
            }
        }

        public void AddCoin()
        {
            _gatheredCoins.Add();
            foreach (var observer in _coinObservers)
            {
                observer.Notify(_gatheredCoins);
            }
        }

        public void AddHealthObserver(IObserver<PlayerHealth> observer)
        {
            _healthObservers.Add(observer.Subscribe(this));
        }

        public void AddCoinObserver(IObserver<GatheredCoins> observer)
        {
            _coinObservers.Add(observer.Subscribe(this));
        }
        
        public void Unsubscribe(IObserver<PlayerHealth> observer)
        {
            _healthObservers.Remove(observer);
        }
        
        public void Unsubscribe(IObserver<GatheredCoins> observer)
        {
            _coinObservers.Remove(observer);
        }
    }
}