using System;
using AlexeyVlasyuk.MultiplayerTest.Models;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class HealthBar : InfoBar, Interfaces.IObserver<PlayerHealth>
    {
        private Interfaces.IObservable<PlayerHealth> _model;

        private void OnDestroy()
        {
            _model.Unsubscribe(this);
        }

        public Interfaces.IObserver<PlayerHealth> Subscribe(Interfaces.IObservable<PlayerHealth> observable)
        {
            _model = observable;

            return this;
        }

        public void Notify(PlayerHealth value)
        {
            OnValueChange(value.Rate);
        }
    }
}