using System.Collections;
using UnityEngine;

namespace Environment.Interactive
{
    public class CollisionSound : MonoBehaviour
    {
        private int _currentSoundsAmount;
        
        [Header("Audio")] 
        public int maxSounds = 2;
        public AudioSource collisionAudio;
        
        private void OnCollisionEnter2D()
        {
            if (_currentSoundsAmount < maxSounds)
            {
                collisionAudio.PlayOneShot(collisionAudio.clip);
                StartCoroutine(AddOneShot());
            }
        }

        private IEnumerator AddOneShot()
        {
            _currentSoundsAmount++;
            yield return new WaitForSeconds(collisionAudio.clip.length);
            _currentSoundsAmount--;
        }
    }
}