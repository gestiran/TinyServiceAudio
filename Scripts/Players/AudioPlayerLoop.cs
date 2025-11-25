// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyServices.Audio.Configs;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyServices.Audio.Players {
    public sealed class AudioPlayerLoop {
        private readonly Transform _pool;
        private readonly Dictionary<string, AudioSource> _active;
        private readonly AudioParameters _parameters;
        
        public AudioPlayerLoop(Transform pool, AudioParameters parameters) {
            _pool = pool;
            _active = new Dictionary<string, AudioSource>();
            _parameters = parameters;
        }
        
        public void PlayLoop<T>(string key, Vector3 position, T config) where T : AudioConfig {
            PlayLoop(key, _pool, position, config);
        }
        
        public void PlayLoop<T>(string key, Transform root, Vector3 position, T config) where T : AudioConfig {
            if (_active.ContainsKey(key)) {
                return;
            }
            
            AudioSource source = UnityObject.Instantiate(_parameters.sources.loop, position, Quaternion.identity, root);
            source.name = $"Loop Effect {key}";
            
            source.Stop();
            
            source.loop = true;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = config.mixer;
            source.clip = config.clip;
            
            source.volume = config.volume;
            source.rolloffMode = _parameters.sources.loop.rolloffMode;
            
            source.Play();
            
            _active.Add(key, source);
        }
        
        public void StopLoop(string key) {
            if (_active.TryGetValue(key, out AudioSource source) == false) {
                return;
            }
            
            source.Stop();
            UnityObject.Destroy(source.gameObject, 0.5f);
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
                UnityObject.Destroy(source.gameObject, 0.5f);
            }
            
            _active.Clear();
        }
    }
}