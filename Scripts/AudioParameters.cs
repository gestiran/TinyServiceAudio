// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Reflection;
using Sirenix.OdinInspector;
using TinyServices.Audio.Configs;
using TinyUtilities;
using UnityEngine;
using UnityEngine.Audio;

namespace TinyServices.Audio {
    [Searchable(FilterOptions = SearchFilterOptions.ValueToString, Recursive = true, FuzzySearch = false)]
    public abstract class AudioParameters : ScriptableObject, ISelfValidator {
        [field: FoldoutGroup(InspectorNames.PARAMETERS)]
        [field: SerializeField, BoxGroup(InspectorNames.PARAMETERS  + "/Sources")]
        public Sources sources { get; private set; }
        
        [field: SerializeField, BoxGroup(InspectorNames.PARAMETERS  + "/Mixers")]
        public Mixers mixers { get; private set; }
        
        [field: SerializeField, BoxGroup(_DISTANCE), LabelText("Min"), Required]
        public float distanceMin { get; private set; } = 0.05f;
        
        [field: SerializeField, BoxGroup(_DISTANCE), LabelText("Max"), Required]
        public float distanceMax { get; private set; } = 7.5f;
        
        private const string _DISTANCE = InspectorNames.PARAMETERS + "/Distance";
        private const float _DISTANCE_MIN = 0.01f;
        private const float _DISTANCE_MAX = 300f;
        
        [Serializable, InlineProperty, HideLabel]
        public sealed class Sources {
            [field: SerializeField, Required]
            public AudioSource single { get; private set; }
            
            [field: SerializeField, Required]
            public AudioSource loop { get; private set; }
            
        #if UNITY_EDITOR
            
            private const string _PATH_TO_SOURCE_SINGLE = "Packages/com.ges.services.audio/Prefabs/AudioSourceSingle.prefab";
            private const string _PATH_TO_SOURCE_LOOP = "Packages/com.ges.services.audio/Prefabs/AudioSourceLoop.prefab";
            
            internal void Reset() {
                if (single == null) {
                    single = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioSource>(_PATH_TO_SOURCE_SINGLE);
                }
                
                if (loop == null) {
                    loop = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioSource>(_PATH_TO_SOURCE_LOOP);
                }
            }
            
        #endif
        }
        
        [Serializable, InlineProperty, HideLabel]
        public sealed class Mixers {
            [field: SerializeField, Required]
            public AudioMixer music { get; private set; }
            
            [field: SerializeField, Required]
            public AudioMixer soundScaled { get; private set; }
            
            [field: SerializeField, Required]
            public AudioMixer soundUnscaled { get; private set; }
            
        #if UNITY_EDITOR
            
            private const string _PATH_TO_MIXER_MUSIC = "Packages/com.ges.services.audio/Mixers/Music.mixer";
            private const string _PATH_TO_MIXER_SOUND_SCALED = "Packages/com.ges.services.audio/Mixers/SoundScaled.mixer";
            private const string _PATH_TO_MIXER_SOUND_UNSCALED = "Packages/com.ges.services.audio/Mixers/SoundUnscaled.mixer";
            
            internal void Reset() {
                if (music == null) {
                    music = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioMixer>(_PATH_TO_MIXER_MUSIC);
                }
                
                if (soundScaled == null) {
                    soundScaled = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioMixer>(_PATH_TO_MIXER_SOUND_SCALED);
                }
                
                if (soundUnscaled == null) {
                    soundUnscaled = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioMixer>(_PATH_TO_MIXER_SOUND_UNSCALED);
                }
            }
            
        #endif
        }
        
        public virtual void Validate(SelfValidationResult result) {
        #if UNITY_EDITOR
            distanceMin = Mathf.Clamp(distanceMin, _DISTANCE_MIN, distanceMax);
            distanceMax = Mathf.Clamp(distanceMax, distanceMin, _DISTANCE_MAX);
        #endif
        }
        
    #if UNITY_EDITOR
        
        [OnInspectorInit]
        private void InitInspector() => ApplyEditorRoot();
        
        [ContextMenu(InspectorNames.SOFT_RESET)]
        protected virtual void Reset() {
            sources.Reset();
            mixers.Reset();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        private void ApplyEditorRoot() {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Type subType = typeof(AudioGroup);
            
            foreach (FieldInfo fieldInfo in fields) {
                if (fieldInfo.FieldType.IsSubclassOf(subType) && fieldInfo.GetValue(this) is AudioGroup other) {
                    other.ApplyEditorRoot(this);
                }
            }
        }
        
    #endif
    }
}