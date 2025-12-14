// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyServices.Audio.Configs;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyServices.Audio.Players {
    public sealed class AudioPlayerSingle {
        private readonly Transform _pool;
        private readonly AudioParameters _parameters;
        private readonly Dictionary<string, List<AudioSource>> _activePool;
        
        public AudioPlayerSingle(Transform pool, AudioParameters parameters) {
            _pool = pool;
            _parameters = parameters;
            _activePool = new Dictionary<string, List<AudioSource>>(128);
        }
        
        public AudioSource PlayLimit<T>(T config, Vector3 position, string key, int count) where T : AudioConfig {
            AudioSource source;
            
            if (_activePool.TryGetValue(key, out List<AudioSource> active)) {
                if (active.Count > count) {
                    source = active[count - 1];
                } else {
                    source = Play(config, position);
                    active.Add(source);   
                }
            } else {
                active = new List<AudioSource>(count);
                source = Play(config, position);
                active.Add(source);
                _activePool.Add(key, active);
            }
            
            return source;
        }
        
        public AudioSource Play<T>(T config, Vector3 position) where T : AudioConfig {
            AudioSource source = UnityObject.Instantiate(_parameters.sources.single, position, Quaternion.identity, _pool);
            source.name = config.ToString();
            
            source.Stop();
            
            source.loop = false;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = config.mixer;
            source.clip = config.clip;
            
            source.volume = config.volume;
            
            source.spatialBlend = position.Equals(Vector3.zero) ? 0 : 1;
            source.minDistance = _parameters.distanceMin;
            source.rolloffMode = _parameters.sources.single.rolloffMode;
            
            if (config.isHaveRange) {
                source.maxDistance = config.range;
            } else {
                source.maxDistance = _parameters.distanceMax;
            }
            
            source.Play();
            
            UnityObject.Destroy(source.gameObject, config.clip.length * 2f);
            
            return source;
        }
        
        internal void ClearActive() => _activePool.Clear();
    }
}