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
        
        public bool isEnableMusicSettings { get; private set; }
        public bool isEnableSoundSettings { get; private set; }
        
        public bool isPause { get; private set; }
        
        internal AudioParameters parametersInternal;
        
        private const string _SNAPSHOT_ACTIVE = "Active";
        private const string _SNAPSHOT_MUTE = "Mute";
        private const float _DEFAULT_TRANSITION_TIME = 0.5f;
        
        protected virtual void Init() {
            Transform pool = new GameObject("Audio").transform;
            
            player = new AudioPlayerSingle(pool, parametersInternal);
            playerLoop = new AudioPlayerLoop(pool);
            
            Object.DontDestroyOnLoad(pool);
            
            isEnableMusic = LoadMusicState();
            isEnableSound = LoadSoundState();
            
            isEnableMusicSettings = isEnableMusic;
            isEnableSoundSettings = isEnableSound;
            
            isPause = LoadPauseState();
            
            ChangeMusicForce(isEnableMusic, _DEFAULT_TRANSITION_TIME);
            ChangeSoundForce(isEnableSound, _DEFAULT_TRANSITION_TIME);
        }
        
        public void ChangeMusicSettings(bool isEnable, float timeToReach = _DEFAULT_TRANSITION_TIME) {
            if (isEnableMusicSettings == isEnable) {
                return;
            }
            
            isEnableMusicSettings = isEnable;
            
            if (isEnable) {
                if (isEnableMusic) {
                    ChangeMusicForce(true, timeToReach);
                }
            } else if (isEnableMusic) {
                ChangeMusicForce(false, timeToReach);
            }
        }
        
        public void ChangeMusicState(bool isEnable, float timeToReach = _DEFAULT_TRANSITION_TIME) {
            if (isEnable == isEnableMusic) {
                return;
            }
            
            if (isEnableMusicSettings) {
                ChangeMusicForce(isEnable, timeToReach);
            }
            
            isEnableMusic = isEnable;
        }
        
        public void ChangeSoundSettings(bool isEnable, float timeToReach = _DEFAULT_TRANSITION_TIME) {
            if (isEnableSoundSettings == isEnable) {
                return;
            }
            
            isEnableSoundSettings = isEnable;
            
            if (isEnable) {
                if (isEnableSound) {
                    ChangeSoundForce(true, timeToReach);
                }
            } else if (isEnableSound) {
                ChangeSoundForce(false, timeToReach);
            }
        }
        
        public void ChangeSoundState(bool isEnable, float timeToReach = _DEFAULT_TRANSITION_TIME) {
            if (isEnable == isEnableSound) {
                return;
            }
            
            if (isEnableSoundSettings) {
                ChangeSoundForce(isEnable, timeToReach);
            }
            
            isEnableSound = isEnable;
        }
        
        private void ChangePause(bool isEnable, float timeToReach = _DEFAULT_TRANSITION_TIME) {
            if (isEnable == isPause) {
                return;
            }
            
            isPause = isEnable;
            
            if (isEnable) {
                if (isEnableSound && isEnableSoundSettings) {
                    ChangeSoundScaledForce(false, timeToReach);
                }
            } else if (isEnableSound && isEnableSoundSettings) {
                ChangeSoundScaledForce(true, timeToReach);
            }
        }
        
        public void ClearAllLoops() => playerLoop.ClearAllLoops();
        
        protected abstract bool LoadMusicState();
        
        protected abstract bool LoadSoundState();
        
        protected abstract bool LoadPauseState();
        
        private void ChangeMusicForce(bool isEnable, float timeToReach) {
            if (isEnable) {
                TransitionToSnapshots(parametersInternal.mixers.music, _SNAPSHOT_ACTIVE, timeToReach);
            } else {
                TransitionToSnapshots(parametersInternal.mixers.music, _SNAPSHOT_MUTE, timeToReach);
            }
        }
        
        private void ChangeSoundForce(bool isEnable, float timeToReach) {
            if (isEnable) {
                TransitionToSnapshots(parametersInternal.mixers.soundUnscaled, _SNAPSHOT_ACTIVE, timeToReach);
            } else {
                TransitionToSnapshots(parametersInternal.mixers.soundUnscaled, _SNAPSHOT_MUTE, timeToReach);
            }
            
            if (isPause == false) {
                ChangeSoundScaledForce(isEnable, timeToReach);
            }
        }
        
        private void ChangeSoundScaledForce(bool isEnable, float timeToReach) {
            if (isEnable) {
                TransitionToSnapshots(parametersInternal.mixers.soundScaled, _SNAPSHOT_ACTIVE, timeToReach);
            } else {
                TransitionToSnapshots(parametersInternal.mixers.soundScaled, _SNAPSHOT_MUTE, timeToReach);
            }
        }
        
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