using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class FireButton : Selectable
    {
        [SerializeField]
        private UnityEvent _onStartFire;

        [SerializeField]
        private UnityEvent _onStopFire;
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            _onStartFire?.Invoke(); 
        }
 
        public override void OnPointerUp(PointerEventData eventData)
        {
            _onStopFire?.Invoke();
        }
    }
}