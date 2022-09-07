using System;
using GameDevLib.Enums;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Audio
{
    /// <summary>
    /// Represents ability to play audio.
    /// </summary>
    /// <remarks>
    /// 1. NegativeClipToPlay and PositiveClipToPlay are treated as primary sources, while AudioClips array is treated as a secondary source of audio clips.
    /// 2. At end of each play, an 'AudioTriggerNotify' event of type 'AudioTriggerHandler' is posted to which you can subscribe. 
    /// </remarks>>
    public class AudioIsPlaying : MonoBehaviour
    {
        #region Links
        [field: SerializeField] 
        private AudioClip NegativeClipToPlay { get; set; }
        
        [field: SerializeField] 
        private AudioClip PositiveClipToPlay { get; set; }
        
        [field: SerializeField] 
        public AudioClip[] AudioClipsToPlay { get; set; }
        #endregion
        
        #region Properties
        [field: SerializeField, Range(0, 1)] 
        public float AudioVolume { get; set; } = 0.5f;
        private AudioSource NegativeSourceToPlay { get; set; }
        private AudioSource PositiveSourceToPlay { get; set; }

        private List<AudioSource> RandomSourcesToPlay { get; set; } = new ();
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

            if (AudioClipsToPlay.Length > 0)
            {
                for (var index = 0; index < AudioClipsToPlay.Length; index++)
                {
                    var sourceToPlay = gameObject.AddComponent<AudioSource>();
                    var clip = AudioClipsToPlay[index];
                    MakeAudio(clip, ref sourceToPlay);
                    RandomSourcesToPlay.Add(sourceToPlay);
                }
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
        public void PlaySound(SoundType type)
        {
            if (type == SoundType.Positive)
            {
                StopAllCoroutines();
            }
            
            StartCoroutine(PlayCoroutine(type, AudioVolume));
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
                        PositiveSourceToPlay.volume = volume; 
                        PositiveSourceToPlay.pitch = pinch; 
                        NegativeSourceToPlay.Play();
                        yield return new WaitWhile(() => NegativeSourceToPlay.isPlaying);
                    }
                    else
                    {
                        yield break;
                    }

                    break;
                case SoundType.Positive:
                    if (PositiveSourceToPlay != null)
                    {
                        PositiveSourceToPlay.volume = volume; 
                        PositiveSourceToPlay.pitch = pinch; 
                        PositiveSourceToPlay.Play();
                        yield return new WaitWhile(() => PositiveSourceToPlay.isPlaying);
                    }
                    else
                    {
                        yield break;
                    }
                    break;
                case SoundType.RandomFromArray:
                    if (RandomSourcesToPlay.Count > 0)
                    {
                        var index = Random.Range(0, RandomSourcesToPlay.Count - 1);
                        var sourceToPlay = RandomSourcesToPlay[index];
                        sourceToPlay.volume = volume;
                        sourceToPlay.Play();
                        yield return new WaitWhile(() => sourceToPlay.isPlaying);
                    }
                    else
                    {
                        yield break;
                    }
                    break;
            }

            // Invoke event
            AudioTriggerNotify?.Invoke(true);
        }
        #endregion
    }
}