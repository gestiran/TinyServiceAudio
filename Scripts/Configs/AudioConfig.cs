using System;
using Sirenix.OdinInspector;
using TinyUtilities.CustomTypes;
using UnityEngine;
using UnityEngine.Audio;
using UnityObject = UnityEngine.Object;

namespace TinyServices.Audio.Configs {
    [Serializable]
    public abstract class AudioConfig : ISelfValidator {
        [field: CustomContextMenu("Enable Range", "EnableRange")]
        [field: CustomContextMenu("Disable Range", "DisableRange")]
        [field: SerializeField, HorizontalGroup, HideLabel]
        public AudioClip clip { get; private set; }
        
        [field: CustomContextMenu("Enable Range", "EnableRange")]
        [field: CustomContextMenu("Disable Range", "DisableRange")]
        [field: SerializeField, HorizontalGroup, HideLabel]
        public AudioMixerGroup mixer { get; private set; }
        
        [field: SerializeField, HorizontalGroup, HideLabel, Range(0, 1f)]
        public float volume { get; private set; } = 1f;
        
        [field: SerializeField, HideLabel, Range(0.1f, 30f), ShowIf("isHaveRange")]
        public float range { get; private set; } = 7.5f;
        
        [field: SerializeField, HideInInspector]
        public bool isHaveRange { get; private set; }
        
        public void Validate(SelfValidationResult result) {
        #if UNITY_EDITOR
            if (_isDuplicate) {
                result.AddError("Is duplicate type!");
            } else if (clip == null) {
                result.AddError($"{nameof(AudioClip)} is null!");
            } else if (mixer == null) {
                result.AddError($"{nameof(AudioMixerGroup)} is null!");
            } else if (mixer.name == "Master") {
                result.AddError("Cant apply master mixer group!");
            }
        #endif
        }
        
    #if UNITY_EDITOR
        
        [NonSerialized] private bool _isDuplicate;
        [NonSerialized] private UnityObject _root;
        
        internal void ApplyEditorRoot(UnityObject root) => _root = root;
        
        internal void UpdateDuplicate(bool isDuplicate) => _isDuplicate = isDuplicate;
        
        private void EnableRange() {
            isHaveRange = true;
            SetDirty();
        }
        
        private void DisableRange() {
            isHaveRange = false;
            SetDirty();
        }
        
        private void SetDirty() {
            if (_root != null) {
                UnityEditor.EditorUtility.SetDirty(_root);   
            }
        }
        
    #endif
        
        public override string ToString() => $"{clip}_{mixer}";
    }
    
    [Serializable]
    public sealed class AudioConfig<T> : AudioConfig, IEquatable<AudioConfig<T>> where T : Enum {
        [field: SerializeField, PropertyOrder(-10), HorizontalGroup, HideLabel]
        public EnumName<T> type { get; private set; } = EnumName.New(default(T));
        
        public bool Equals(AudioConfig<T> other) => other != null && type.Equals(other.type);
        
        public override string ToString() => $"{type}_{base.ToString()}";
    }
}