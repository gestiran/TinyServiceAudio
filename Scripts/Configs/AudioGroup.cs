// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyUtilities.Extensions.Unity;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyServices.Audio.Configs {
    public abstract class AudioGroup {
        [NonSerialized] internal UnityObject root;
        
        internal virtual void ApplyEditorRoot(UnityObject value) => root = value;
    }
    
    [Serializable, InlineProperty, HideLabel]
    public sealed class AudioGroup<T> : AudioGroup where T : Enum {
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowItemCount = false)]
        [SerializeField, LabelText("@GetLabel()"), OnValueChanged("UpdateDuplicateChecking", true)]
        private AudioConfig<T>[] _configs;
        
        [NonSerialized]
        private Dictionary<T, AudioConfig<T>> _cache;
        
        internal Dictionary<T, AudioConfig<T>> GetConfigs() {
            if (_cache != null) {
                return _cache;
            }
            
            _cache = new Dictionary<T, AudioConfig<T>>(_configs.Length);
            
            for (int configId = 0; configId < _configs.Length; configId++) {
                _cache.Add(_configs[configId].type.value, _configs[configId]);
            }
            
            return _cache;
        }
        
    #if UNITY_EDITOR
        
        [OnInspectorInit]
        private void InspectorInit() {
            UpdateDuplicateChecking();
            UpdateConfigs();
        }
        
        private void UpdateDuplicateChecking() {
            if (_configs != null) {
                Dictionary<T, AudioConfig<T>> checkList = new Dictionary<T, AudioConfig<T>>(_configs.Length);
                
                for (int configId = 0; configId < _configs.Length; configId++) {
                    _configs[configId].UpdateDuplicate(checkList.TryAdd(_configs[configId].type.value, _configs[configId]) == false);
                }
            }
        }
        
        private void UpdateConfigs() {
            if (_configs != null) {
                string[] names = Enum.GetNames(typeof(T));
                
                if (_configs.Length != names.Length) {
                    List<AudioConfig<T>> result = new List<AudioConfig<T>>(names.Length);
                    
                    for (int nameId = 0; nameId < names.Length; nameId++) {
                        if (TryGetConfig(names[nameId], out AudioConfig<T> config)) {
                            result.Add(config);
                        } else {
                            result.Add(AudioConfig<T>.New(names[nameId]));
                        }
                    }
                    
                    _configs = result.ToArray();
                    root.TrySetDirty();
                }
            }
        }
        
        private bool TryGetConfig(string enumName, out AudioConfig<T> config) {
            foreach (AudioConfig<T> other in _configs) {
                if (other != null && other.type.ToString().Equals(enumName)) {
                    config = other;
                    return true;
                }
            }
            
            config = null;
            return false;
        }
        
        private string GetLabel() => typeof(T).Name;
        
        internal override void ApplyEditorRoot(UnityObject value) {
            base.ApplyEditorRoot(value);
            
            if (_configs != null) {
                for (int configId = 0; configId < _configs.Length; configId++) {
                    _configs[configId].ApplyEditorRoot(value);
                }
            }
        }
        
    #endif
    }
}