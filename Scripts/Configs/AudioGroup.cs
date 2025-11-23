using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyServices.Audio.Configs {
    public abstract class AudioGroup { }
    
    [Serializable, InlineProperty, HideLabel]
    public sealed class AudioGroup<T> : AudioGroup where T : Enum {
        [Searchable(FilterOptions = SearchFilterOptions.ValueToString)]
        [SerializeField, ListDrawerSettings(ShowFoldout = false, HideRemoveButton = true), OnValueChanged("UpdateDuplicateChecking", true)]
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
        
        [NonSerialized] private UnityObject _root;
        
        [OnInspectorInit]
        private void InspectorInit() => UpdateDuplicateChecking();
        
        private void UpdateDuplicateChecking() {
            if (_configs != null) {
                Dictionary<T, AudioConfig<T>> checkList = new Dictionary<T, AudioConfig<T>>(_configs.Length);
                
                for (int configId = 0; configId < _configs.Length; configId++) {
                    _configs[configId].UpdateDuplicate(checkList.TryAdd(_configs[configId].type.value, _configs[configId]));
                }
            }
        }
        
        public void ApplyEditorRoot(UnityObject root) {
            _root = root;
            
            if (_configs != null) {
                for (int configId = 0; configId < _configs.Length; configId++) {
                    _configs[configId].ApplyEditorRoot(_root);
                }
            }
        }
        
    #endif
    }
}