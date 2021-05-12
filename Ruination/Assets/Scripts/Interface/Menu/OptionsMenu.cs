using UnityEngine;
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
        
        private void Start()
        {
            if (PlayerPrefs.HasKey(PrefsProperties.MasterVolume))
                masterSlider.value = PlayerPrefs.GetFloat(PrefsProperties.MasterVolume);
            
            if (PlayerPrefs.HasKey(PrefsProperties.MusicVolume))
                musicSlider.value = PlayerPrefs.GetFloat(PrefsProperties.MusicVolume);
            
            if (PlayerPrefs.HasKey(PrefsProperties.EffectVolume))
                effectSlider.value = PlayerPrefs.GetFloat(PrefsProperties.EffectVolume);
            
            if (PlayerPrefs.HasKey(PrefsProperties.AmbientVolume))
                ambientSlider.value = PlayerPrefs.GetFloat(PrefsProperties.AmbientVolume);
            
            EnableMenu(false);
        }

        public void HandleMasterSlider()
        {
            AudioListener.volume = masterSlider.value;
            SaveState();
        }
        
        public void HandleMusicSlider()
        {
            
        }
        
        public void HandleEffectSlider()
        {
            
        }
        
        public void HandleAmbientSlider()
        {
            
        }
        
        public override void EnableMenu(bool enable)
        {
            gameObject.SetActive(enable);
        }
        
        public override bool GetMenuControls() => false;

        private void SaveState()
        {
            PlayerPrefs.SetFloat(PrefsProperties.MasterVolume, masterSlider.value);
            PlayerPrefs.SetFloat(PrefsProperties.MusicVolume, musicSlider.value);
            PlayerPrefs.SetFloat(PrefsProperties.EffectVolume, effectSlider.value);
            PlayerPrefs.SetFloat(PrefsProperties.AmbientVolume, ambientSlider.value);
            PlayerPrefs.Save();
        }
    }
}