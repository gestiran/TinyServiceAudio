// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyServices.Audio.Configs;
using UnityEngine;

namespace TinyServices.Audio.Extensions {
    public static class AudioGroupExtension {
        public static AudioSource Play<T>(this AudioGroup<T> group, T type) where T : Enum {
            return group.GetConfigs()[type].Play();
        }
        
        public static AudioSource Play<T>(this AudioGroup<T> group, T type, Vector3 position) where T : Enum {
            return group.GetConfigs()[type].Play(position);
        }
        
        public static AudioSource Play<T>(this AudioGroup<T> group, T type, int limit) where T : Enum {
            string key = $"{type.GetType().Name}_{type}";
            return group.GetConfigs()[type].Play(key, limit);
        }
        
        public static AudioSource Play<T>(this AudioGroup<T> group, T type, Vector3 position, int limit) where T : Enum {
            string key = $"{type.GetType().Name}_{type}";
            return group.GetConfigs()[type].Play(position, key, limit);
        }
        
        public static AudioSource PlayLoop<T>(this AudioGroup<T> group, T type, string key) where T : Enum {
            return group.GetConfigs()[type].PlayLoop(key);
        }
        
        public static AudioSource PlayLoop<T>(this AudioGroup<T> group, T type) where T : Enum {
            return group.GetConfigs()[type].PlayLoop();
        }
        
        public static AudioSource PlayLoop<T>(this AudioGroup<T> group, T type, string key, Transform root) where T : Enum {
            return group.GetConfigs()[type].PlayLoop(key, root);
        }
        
        public static AudioSource PlayLoop<T>(this AudioGroup<T> group, T type, Transform root) where T : Enum {
            return group.GetConfigs()[type].PlayLoop(root);
        }
        
        public static AudioSource PlayLoop<T>(this AudioGroup<T> group, T type, string key, Vector3 position) where T : Enum {
            return group.GetConfigs()[type].PlayLoop(key, position);
        }
        
        public static AudioSource PlayLoop<T>(this AudioGroup<T> group, T type, Vector3 position) where T : Enum {
            return group.GetConfigs()[type].PlayLoop(position);
        }
        
        public static AudioSource PlayLoop<T>(this AudioGroup<T> group, T type, string key, Transform root, Vector3 position) where T : Enum {
            return group.GetConfigs()[type].PlayLoop(key, root, position);
        }
        
        public static AudioSource PlayLoop<T>(this AudioGroup<T> group, T type, Transform root, Vector3 position) where T : Enum {
            return group.GetConfigs()[type].PlayLoop(root, position);
        }
        
        public static void StopLoop<T>(this AudioGroup<T> group, T type, string key) where T : Enum {
            group.GetConfigs()[type].StopLoop(key);
        }
        
        public static void StopLoop<T>(this AudioGroup<T> group, T type) where T : Enum {
            group.GetConfigs()[type].StopLoop();
        }
        
        public static void ChangeVolume<T>(this AudioGroup<T> group, T type, string key, float volume) where T : Enum {
            group.GetConfigs()[type].ChangeVolume(key, volume);
        }
        
        public static void ChangeVolume<T>(this AudioGroup<T> group, T type, float volume) where T : Enum {
            group.GetConfigs()[type].ChangeVolume(volume);
        }
    }
}