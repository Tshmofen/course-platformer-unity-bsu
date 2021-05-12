using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Fading
{
    [RequireComponent(typeof(Animator), typeof(Image))]
    public class SceneFader : MonoBehaviour
    {
        #region Fields & properties
        
        private Animator _animator;
        private int _startHash; 
        private int _fromBlackHash; 
        private int _waitHash; 
        private int _fadeHash;
        
        [Header("Animator")] 
        public string startTriggerName = "startFade";
        public string fromBlackBoolName = "fromBlack";
        public string waitTimeFloatName = "waitTime";
        public string fadeTimeFloatName = "fadeTime";
        [Header("Visuals")]
        public float fadeTime = 1;
        public float waitTime = 0.5f;

        #endregion

        private void Start()
        {
            _startHash = Animator.StringToHash(startTriggerName);
            _fromBlackHash = Animator.StringToHash(fromBlackBoolName);
            _waitHash = Animator.StringToHash(waitTimeFloatName);
            _fadeHash = Animator.StringToHash(fadeTimeFloatName);
            _animator = GetComponent<Animator>();
            
            StartFade(true);
        }

        public void StartFade(bool fromBlack)
        {
            _animator.SetBool(_fromBlackHash, fromBlack);
            _animator.SetFloat(_waitHash, 1 / waitTime);
            _animator.SetFloat(_fadeHash, 1 / fadeTime);
            _animator.SetTrigger(_startHash);
        }

        #region Callbacks
        
        public event Action OnFadeAddingEnd;
        public event Action OnFadeRemovingEnd;

        // called as events of animation
        public void CallFadeAddingEnd() => OnFadeAddingEnd?.Invoke();
        public void CallFadeRemovingEnd() => OnFadeRemovingEnd?.Invoke();

        #endregion
        
    }
}