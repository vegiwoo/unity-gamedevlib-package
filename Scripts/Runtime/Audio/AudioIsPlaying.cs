using System;
using GameDevLib.Enums;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Audio
{
    /// <summary>
    /// Represents ability to play audio.
    /// </summary>
    public class AudioIsPlaying : MonoBehaviour
    {
        #region Links
        [field: SerializeField] 
        private AudioClip NegativeClipToPlay { get; set; }
        
        [field: SerializeField] 
        private AudioClip PositiveClipToPlay { get; set; }
        #endregion
        
        #region Properties
        private AudioSource NegativeSourceToPlay { get; set; }
        private AudioSource PositiveSourceToPlay { get; set; }
        #endregion
        
        #region Constants and variables 
        private Coroutine _audioPlayCoroutine;
        #endregion
        
        #region Events
        public delegate void AudioTriggerHandler(bool isSoundPlayed);  
        [CanBeNull] 
        public event AudioTriggerHandler AudioTriggerNotify;
        #endregion
        
        #region MonoBehaviour methods

        private void Start()
        {
            if (NegativeClipToPlay != null)
            {
                var sourceToPlay = gameObject.AddComponent<AudioSource>();
                MakeAudio(NegativeClipToPlay, ref sourceToPlay);
                NegativeSourceToPlay = sourceToPlay;
            }
            
            if (PositiveClipToPlay != null)
            {
                var sourceToPlay = gameObject.AddComponent<AudioSource>();
                MakeAudio(PositiveClipToPlay, ref sourceToPlay);
                PositiveSourceToPlay = sourceToPlay;
            }
        }

        #endregion
        
        #region Functionality
        /// <summary>
        /// Creates an AudioSource from provided AudioClip.
        /// </summary>
        /// <param name="clip">AudioClip to create an AudioSource.</param>
        /// <param name="audioSource">AudioSource to play audio</param>
        private static void MakeAudio(in AudioClip clip, ref AudioSource audioSource)
        {
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
            audioSource.pitch = 1;
            audioSource.panStereo = audioSource.spatialBlend = audioSource.spread = 0;
            audioSource.volume = audioSource.reverbZoneMix = audioSource.dopplerLevel = 1;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.minDistance = 1;
            audioSource.maxDistance = 500;
        }
        
        /// <summary>
        /// Starts audio playback.
        /// </summary>
        public void PlaySound(SoundType type, float volume = 1.0f)
        {
            _audioPlayCoroutine = StartCoroutine(PlayCoroutine(type, volume));
        }
        
        /// <summary>
        /// Coroutine that plays audio.
        /// </summary>
        /// <param name="type">Type of audio being played.</param>
        /// <param name="volume">Volume of audio being played.</param>
        /// <remarks>
        /// Dispatches an event about end of audio playback.
        /// </remarks>
        private IEnumerator PlayCoroutine(SoundType type,  float volume)
        {
            var pinch =  Random.Range(0.55f, 1.0f);
            
            switch (type)
            {
                case SoundType.Negative:
                    if (NegativeSourceToPlay != null)
                    {
                        NegativeSourceToPlay.volume = volume;
                        NegativeSourceToPlay.pitch = pinch;
                        NegativeSourceToPlay.Play();
                        yield return new WaitWhile (()=> NegativeSourceToPlay.isPlaying);
                    }
                    break;
                case SoundType.Positive:
                    if (PositiveSourceToPlay != null)
                    {
                        PositiveSourceToPlay.volume = volume;
                        PositiveSourceToPlay.pitch = pinch;
                        PositiveSourceToPlay.Play();
                        yield return new WaitWhile (()=> PositiveSourceToPlay.isPlaying);
                    }
                    break;
            }
            AudioTriggerNotify?.Invoke(true);
        }
        #endregion
    }
}