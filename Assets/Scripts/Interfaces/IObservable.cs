namespace AlexeyVlasyuk.MultiplayerTest.Interfaces
{
    public interface IObservable<T>
    {
        void Unsubscribe(IObserver<T> observer);
    }
}