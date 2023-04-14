using AlexeyVlasyuk.MultiplayerTest.Interfaces;
using AlexeyVlasyuk.MultiplayerTest.Models;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class CoinBar : InfoBar, Interfaces.IObserver<GatheredCoins>
    {
        private Interfaces.IObservable<GatheredCoins> _model;

        private void OnDestroy()
        {
            _model?.Unsubscribe(this);
        }

        public IObserver<GatheredCoins> Subscribe(IObservable<GatheredCoins> observable)
        {
            _model = observable;

            return this;
        }

        public void Notify(GatheredCoins value)
        {
            OnValueChange(value.Rate);
        }
    }
}