using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Util;

namespace Interface.Menu
{
    public class OptionsMenu : BaseMenu
    {
        [Header("Sliders")] 
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider effectSlider;
        public Slider ambientSlider;
        [Header("Audio Groups")] 
        public AudioMixer masterMixer;
        
        
        private void Start()
        {
            if (PlayerPrefs.HasKey(AudioProperties.MasterVolume))
                masterSlider.value = PlayerPrefs.GetFloat(AudioProperties.MasterVolume);
            
            if (PlayerPrefs.HasKey(AudioProperties.MusicVolume))
                musicSlider.value = PlayerPrefs.GetFloat(AudioProperties.MusicVolume);
            
            if (PlayerPrefs.HasKey(AudioProperties.EffectVolume))
                effectSlider.value = PlayerPrefs.GetFloat(AudioProperties.EffectVolume);
            
            if (PlayerPrefs.HasKey(AudioProperties.AmbientVolume))
                ambientSlider.value = PlayerPrefs.GetFloat(AudioProperties.AmbientVolume);
            
            UpdateMixerState();
            EnableMenu(false);
        }

        // called by a slider change
        public void UpdateMixerState()
        {
            masterMixer.SetFloat(AudioProperties.MasterVolume, Mathf.Log(masterSlider.value) * 20);
            masterMixer.SetFloat(AudioProperties.MusicVolume, Mathf.Log(musicSlider.value) * 20);
            masterMixer.SetFloat(AudioProperties.EffectVolume, Mathf.Log(effectSlider.value) * 20);
            masterMixer.SetFloat(AudioProperties.AmbientVolume, Mathf.Log(ambientSlider.value) * 20);
            SaveStateInPrefs();
        }
        
        public override void EnableMenu(bool enable)
        {
            gameObject.SetActive(enable);
        }
        
        public override bool GetMenuControls() => false;

        private void SaveStateInPrefs()
        {
            PlayerPrefs.SetFloat(AudioProperties.MasterVolume, masterSlider.value);
            PlayerPrefs.SetFloat(AudioProperties.MusicVolume, musicSlider.value);
            PlayerPrefs.SetFloat(AudioProperties.EffectVolume, effectSlider.value);
            PlayerPrefs.SetFloat(AudioProperties.AmbientVolume, ambientSlider.value);
            PlayerPrefs.Save();
        }
    }
}