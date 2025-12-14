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
        private readonly Dictionary<string, ActiveLink> _activePool;
        
        private sealed class ActiveLink {
            public float remainingTime { get; private set; }
            
            public readonly AudioSource source;
            
            public ActiveLink(AudioSource source, float remainingTime) {
                this.source = source;
                this.remainingTime = remainingTime;
            }
            
            public bool Next(float time) {
                remainingTime -= time;
                return remainingTime > 0;
            }
        }
        
        public AudioPlayerSingle(Transform pool, AudioParameters parameters) {
            _pool = pool;
            _parameters = parameters;
            _activePool = new Dictionary<string, ActiveLink>(128);
        }
        
        public AudioSource PlayLimit<T>(T config, Vector3 position, string key, float limit) where T : AudioConfig {
            if (_activePool.TryGetValue(key, out ActiveLink link)) {
                return link.source;
            }
            
            AudioSource source = Play(config, position);
            _activePool.Add(key, new ActiveLink(source, limit));
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
        
        internal void UpdateActive(float time) {
            List<string> keys = new List<string>(32);
            
            foreach (KeyValuePair<string, ActiveLink> pair in _activePool) {
                if (pair.Value.Next(time)) {
                    continue;
                }
                
                keys.Add(pair.Key);
            }
            
            if (keys.Count > 0) {
                foreach (string key in keys) {
                    _activePool.Remove(key);
                }
            }
        }
    }
}