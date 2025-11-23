using TinyServices.Audio.Configs;
using UnityEngine;

namespace TinyServices.Audio.Players {
    public sealed class AudioPlayerSingle {
        private readonly Transform _pool;
        private readonly AudioParameters _parameters;
        
        public AudioPlayerSingle(Transform pool, AudioParameters parameters) {
            _pool = pool;
            _parameters = parameters;
        }
        
        public void Play<T>(T config, Vector3 position) where T : AudioConfig {
            AudioSource source = Object.Instantiate(_parameters.sources.single, position, Quaternion.identity, _pool);
            
            source.Stop();
            
            source.loop = false;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = config.mixer;
            source.clip = config.clip;
            
            source.volume = config.volume;
            
            source.spatialBlend = position.Equals(Vector3.zero) ? 0 : 1;
            source.minDistance = _parameters.distanceMin;
            
            if (config.isHaveRange) {
                source.maxDistance = config.range;
            } else {
                source.maxDistance = _parameters.distanceMax;
            }
            
            source.Play();
            
            Object.Destroy(source.gameObject, config.clip.length * 2f);
        }
    }
}