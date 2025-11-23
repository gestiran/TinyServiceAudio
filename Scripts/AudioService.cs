using TinyServices.Audio.Players;
using UnityEngine;
using UnityEngine.Audio;

namespace TinyServices.Audio {
    public abstract class AudioService {
        public static AudioService instance { get; internal set; }
        
        public AudioPlayerSingle player { get; private set; }
        public AudioPlayerLoop playerLoop { get; private set; }
        public bool isEnableMusic { get; private set; }
        public bool isEnableSound { get; private set; }
        
        internal AudioParameters parametersInternal;
        
        private const string _SNAPSHOT_ACTIVE = "Active";
        private const string _SNAPSHOT_MUTE = "Mute";
        private const float _DEFAULT_TRANSITION_TIME = 0.5f;
        
        protected virtual void Init() {
            Transform pool = new GameObject("Audio").transform;
            
            player = new AudioPlayerSingle(pool, parametersInternal);
            playerLoop = new AudioPlayerLoop(pool);
            
            Object.DontDestroyOnLoad(pool);
            
            isEnableMusic = true;
            isEnableSound = true;
        }
        
        public void UpdateAmbient(bool isEnable, float timeToReach = _DEFAULT_TRANSITION_TIME) {
            if (isEnable == isEnableMusic) {
                return;
            }
            
            if (isEnable) {
                TransitionToSnapshots(parametersInternal.mixers.music, _SNAPSHOT_ACTIVE, timeToReach);
            } else {
                TransitionToSnapshots(parametersInternal.mixers.music, _SNAPSHOT_MUTE, timeToReach);
            }
            
            isEnableMusic = isEnable;
        }
        
        public void UpdateSound(bool isEnable, bool isPause, float timeToReach = _DEFAULT_TRANSITION_TIME) {
            if (isEnable == isEnableSound) {
                return;
            }
            
            if (isEnable) {
                TransitionToSnapshots(parametersInternal.mixers.soundUnscaled, _SNAPSHOT_ACTIVE, timeToReach);
                
                if (isPause == false) {
                    TransitionToSnapshots(parametersInternal.mixers.soundScaled, _SNAPSHOT_ACTIVE, timeToReach);
                }
            } else {
                TransitionToSnapshots(parametersInternal.mixers.soundUnscaled, _SNAPSHOT_MUTE, timeToReach);
                
                if (isPause == false) {
                    TransitionToSnapshots(parametersInternal.mixers.soundScaled, _SNAPSHOT_MUTE, timeToReach);
                }
            }
            
            isEnableSound = isEnable;
        }
        
        private void UpdatePause(bool isPause, float timeToReach = _DEFAULT_TRANSITION_TIME) {
            if (isEnableSound) {
                if (isPause) {
                    TransitionToSnapshots(parametersInternal.mixers.soundScaled, _SNAPSHOT_MUTE, timeToReach);
                } else {
                    TransitionToSnapshots(parametersInternal.mixers.soundScaled, _SNAPSHOT_ACTIVE, timeToReach);
                }
            }
        }
        
        public void ClearAllLoops() => playerLoop.ClearAllLoops();
        
        private void TransitionToSnapshots(AudioMixer mixer, string snapshotName, float timeToReach) {
            mixer.TransitionToSnapshots(new[] { mixer.FindSnapshot(snapshotName) }, new float[] { 1f }, timeToReach);
        }
    }
    
    public abstract class AudioService<T1, T2> : AudioService where T1 : AudioService<T1, T2>, new() where T2 : AudioParameters {
        public T2 parameters { get; private set; }
        
        public new static T1 instance { get; private set; }
        
        static AudioService() {
            instance = new T1();
            instance.Init();
            AudioService.instance = instance;
        }
        
        protected override void Init() {
            parameters = LoadParameters();
            parametersInternal = parameters;
            base.Init();
        }
        
        protected abstract T2 LoadParameters();
    }
}