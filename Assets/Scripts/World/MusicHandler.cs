using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour{
    public static MusicHandler instance;
    
    [Header("Will interpolate between 1 to the other")]
    public AudioSource[] sources;

    public float maxVolume;
    
    [SerializeField] float smoothing;
    float smoothVelSelected, smoothVelOther;

    
    int selectedSource = 0;
    int SelectedSource{
        get{ return selectedSource; }
        set{
            if(value == selectedSource){ return; }            
            selectedSource = value % sources.Length;            
        }
    }
    public AudioSource PlayingSource{
        get{ return sources[SelectedSource]; }
    }   

    public void NextSong(AudioClip clip){
        int nextSource = (SelectedSource + 1) % sources.Length;
        sources[nextSource].clip = clip;
        SelectedSource++;
    }
    
    
    private void Awake() {
        if(instance){
            Destroy(this);
            return;
        }
        instance = this;  
    }    
    
    private void Update() {
        for(int i = 0; i < sources.Length; i++){
            AudioSource thisSource  = sources[i];
            if(thisSource.volume <=0){ thisSource.Stop(); }
            else if(!thisSource.isPlaying){ thisSource.Play(); }

            if(i == SelectedSource){                
                PlayingSource.volume = Mathf.SmoothDamp(PlayingSource.volume, maxVolume, ref smoothVelSelected, smoothing);
                continue;
            }
            thisSource.volume = Mathf.SmoothDamp(thisSource.volume, 0, ref smoothVelOther, smoothing);
        }
    }
}
