// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyServices.Audio.Configs;
using UnityEngine;

namespace TinyServices.Audio.Extensions {
    public static class AudioConfigExtension {
        private static readonly Vector3 _zeroPosition = Vector3.zero;
        
        public static AudioSource Play<T>(this T config) where T : AudioConfig {
            return AudioService.instance.player.Play(config, _zeroPosition);
        }
        
        public static AudioSource Play<T>(this T config, Vector3 position) where T : AudioConfig {
            return AudioService.instance.player.Play(config, position);
        }
        
        public static AudioSource Play<T>(this T config, string key, float limit) where T : AudioConfig {
            return AudioService.instance.player.PlayLimit(config, _zeroPosition, key, limit);
        }
        
        public static AudioSource Play<T>(this T config, Vector3 position, string key, float limit) where T : AudioConfig {
            return AudioService.instance.player.PlayLimit(config, position, key, limit);
        }
        
        public static AudioSource PlayLoop<T>(this T config, string key) where T : AudioConfig {
            return AudioService.instance.playerLoop.PlayLoop(key, _zeroPosition, config);
        }
        
        public static AudioSource PlayLoop<T>(this T config) where T : AudioConfig {
            return AudioService.instance.playerLoop.PlayLoop(config.ToString(), _zeroPosition, config);
        }
        
        public static AudioSource PlayLoop<T>(this T config, string key, Transform root) where T : AudioConfig {
            return AudioService.instance.playerLoop.PlayLoop(key, root, _zeroPosition, config);
        }
        
        public static AudioSource PlayLoop<T>(this T config, Transform root) where T : AudioConfig {
            return AudioService.instance.playerLoop.PlayLoop(config.ToString(), root, _zeroPosition, config);
        }
        
        public static AudioSource PlayLoop<T>(this T config, string key, Vector3 position) where T : AudioConfig {
            return AudioService.instance.playerLoop.PlayLoop(key, position, config);
        }
        
        public static AudioSource PlayLoop<T>(this T config, Vector3 position) where T : AudioConfig {
            return AudioService.instance.playerLoop.PlayLoop(config.ToString(), position, config);
        }
        
        public static AudioSource PlayLoop<T>(this T config, string key, Transform root, Vector3 position) where T : AudioConfig {
            return AudioService.instance.playerLoop.PlayLoop(key, root, position, config);
        }
        
        public static AudioSource PlayLoop<T>(this T config, Transform root, Vector3 position) where T : AudioConfig {
            return AudioService.instance.playerLoop.PlayLoop(config.ToString(), root, position, config);
        }
        
        public static void StopLoop<T>(this T _, string key) where T : AudioConfig {
            AudioService.instance.playerLoop.StopLoop(key);
        }
        
        public static void StopLoop<T>(this T config) where T : AudioConfig {
            AudioService.instance.playerLoop.StopLoop(config.ToString());
        }
        
        public static void ChangeVolume<T>(this T _, string key, float volume) where T : AudioConfig {
            AudioService.instance.playerLoop.ChangeVolume(key, volume);
        }
        
        public static void ChangeVolume<T>(this T config, float volume) where T : AudioConfig {
            AudioService.instance.playerLoop.ChangeVolume(config.ToString(), volume);
        }
    }
}