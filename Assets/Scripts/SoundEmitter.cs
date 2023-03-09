using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.Character;
using JoshGames.CharacterController;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.AI.Sensors{
    public class SoundEmitter : MonoBehaviour{
        public static SoundEmitter instance;
        [SerializeField] bool showGizmos;
        public class Sound{
            public AudioSource audio;
            public CharacterBase soundEmitter;
            public Transform soundEmitterTransform;
            public Vector3 soundPosition;
            public float soundRadius;
            float soundDuration; // Holds a time until this sound needs to be removed...
            public Color? debugColour;
            public Sound(AudioSource _audio, CharacterBase _soundEmitter, Vector3 _soundPosition, Color? _debugColour = null, Transform _soundEmitterTransform = null){
                this.audio = _audio;
                this.soundEmitter = _soundEmitter;
                this.soundPosition = _soundPosition;
                this.soundRadius = _audio.maxDistance / 10;                
                this.soundDuration = Time.time + _audio.clip.length;
                this.debugColour = _debugColour;
                
                if(soundEmitter){
                    this.soundEmitterTransform = (_soundEmitterTransform)? _soundEmitterTransform : _soundEmitter.transform;
                }                
            }

            public bool Execute(){
                if(Time.time >= soundDuration){ return false; }
                return true;
            }
        }
        
        [HideInInspector] public List<Sound> worldSounds = new List<Sound>();

        /// <summary>
        /// This function will compare all sounds and see if the calling character is within the radius of the sounds
        /// </summary>
        /// <param name="origin">The centre position to be compared to all the sound positions</param>
        /// <param name="soundEmitter">(Optional) this parameter is used to check if this agent is the same type which has caused the sound... If so then the sound will be ignored</param>
        /// <returns>A sound class where a sound was generated</returns>
        public int? GetClosestSound(Vector3 origin, CharacterBase callingChar = null){
            AIBrain _callingChar = (AIBrain) callingChar;
            
            int? closestSound = null;            
            float closestDistance = Mathf.Infinity;

            for(int i = 0; i < worldSounds.Count; i++){
                Sound sound = worldSounds[i];
                AIBrain sEmitter = (sound.soundEmitter?.GetType() != typeof(PlayerController) && (AIBrain)sound.soundEmitter)? sound.soundEmitter?.transform.GetComponent<AIBrain>() : null;
                // If of AI and is of the same agent profile... Then ignore sound 
                if(sEmitter?.agentProfile.name == _callingChar?.agentProfile.name){ continue; }
                
                if(_callingChar?.transform.GetInstanceID() == sEmitter?.transform.GetInstanceID()){ continue; } // Ensures its not targetting itself

                float thisDistance = (sound.soundPosition - origin).magnitude;
                // If sound is not as close as the 'closestSound' variable value...                 
                if(closestSound != null && closestDistance > thisDistance){ continue; }
                closestSound = i;
            }            
           
            // Ensure that this character is inside the sound boundaries
            bool positionInRadius = (closestSound != null && (worldSounds[(int)closestSound].soundPosition - origin).magnitude <= worldSounds[(int)closestSound].soundRadius)? true : false;
            if(positionInRadius){ return closestSound; }
            
            return null;
        }

        private void Awake() {
            if(instance){
                Destroy(this);
                return;
            }
            instance = this;        
        }

        private void Update() {
            for(int i = 0; i< worldSounds.Count; i++){
                if(!(worldSounds.Count >= i)){ break; }
                if(!worldSounds[i].Execute()){ worldSounds.Remove(worldSounds[i]); }
            }            
        }        


        private void OnDrawGizmos() {
            if(!showGizmos){ return; }
            foreach(Sound sound in worldSounds){
                if(sound.debugColour != null){
                    Gizmos.color = (Color)sound?.debugColour;
                    Gizmos.DrawWireSphere(sound.soundPosition, sound.soundRadius);
                }
            }
        }
    }
}