using UnityEngine;
using Random = UnityEngine.Random;

namespace VR_Group_Project.Scripts
{
    public class AudioController : MonoBehaviour
    {
        private AudioSource _audioSource;
        public AudioClip[] ambientMusics;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            InvokeRepeating(nameof(CheckForMusicTerminationAndReplay), 0, 10);
        }

        private void CheckForMusicTerminationAndReplay()
        {
            if (_audioSource.isPlaying)
            {
                return;
            }
            
            _audioSource.clip = ambientMusics[Random.Range(0, ambientMusics.Length)];
            _audioSource.Play();
        }
    }
}