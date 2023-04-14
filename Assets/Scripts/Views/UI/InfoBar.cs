using UnityEngine;
using System;
using UnityEngine.UI;

namespace AlexeyVlasyuk.MultiplayerTest.Views
{
    public class InfoBar : MonoBehaviour
    {
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }
        
        protected void OnValueChange(float value)
        {
            _image.fillAmount = Mathf.Clamp(value, 0, 1f);
        }
    }
}