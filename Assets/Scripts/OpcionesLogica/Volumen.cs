using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volumen : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Image imagenMute;
    
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        slider.value = sliderValue;
        AudioListener.volume = slider.value;
        RevisarSiEstoyMute();
    }

    public void Cambiarlider(float valor)
    {
        sliderValue = valor;
        PlayerPrefs.SetFloat("volumenAudio", sliderValue);
        AudioListener.volume = slider.value;
        RevisarSiEstoyMute();
    }

    private void RevisarSiEstoyMute()
    {
        if (sliderValue == 0)
        {
            imagenMute.enabled = true;
        }
        else
        {
            imagenMute.enabled = false;
        }

    }
}