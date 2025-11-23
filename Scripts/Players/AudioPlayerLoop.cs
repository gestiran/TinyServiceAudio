using System.Collections.Generic;
using TinyServices.Audio.Configs;
using UnityEngine;

namespace TinyServices.Audio.Players {
    public sealed class AudioPlayerLoop {
        private readonly Transform _pool;
        private readonly Dictionary<string, AudioSource> _active;
        
        public AudioPlayerLoop(Transform pool) {
            _pool = pool;
            _active = new Dictionary<string, AudioSource>();
        }
        
        public void PlayLoop<T>(string key, Vector3 position, T config) where T : AudioConfig {
            PlayLoop(key, _pool, position, config);
        }
        
        public void PlayLoop<T>(string key, Transform root, Vector3 position, T config) where T : AudioConfig {
            if (_active.ContainsKey(key)) {
                return;
            }
            
            GameObject soundEffect = new GameObject($"Loop Effect {key}");
            soundEffect.transform.SetParent(root, false);
            soundEffect.transform.position = position;
            
            AudioSource source = soundEffect.AddComponent<AudioSource>();
            
            source.Stop();
            
            source.loop = true;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = config.mixer;
            source.clip = config.clip;
            
            source.volume = config.volume;
            
            source.Play();
            
            _active.Add(key, source);
        }
        
        public void StopLoop(string key) {
            if (_active.TryGetValue(key, out AudioSource source) == false) {
                return;
            }
            
            source.Stop();
            Object.Destroy(source.gameObject, 0.5f);
            _active.Remove(key);
        }
        
        public void ChangeVolume(string key, float volume) {
            if (_active.TryGetValue(key, out AudioSource source) == false) {
                return;
            }
            
            source.volume = volume;
        }
        
        public void ClearAllLoops() {
            foreach (AudioSource source in _active.Values) {
                source.Stop();
                Object.Destroy(source.gameObject, 0.5f);
            }
            
            _active.Clear();
        }
    }
}