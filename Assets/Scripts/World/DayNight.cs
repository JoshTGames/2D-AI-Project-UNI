using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class DayNight : MonoBehaviour{
    public static DayNight instance;

    [Serializable] public class Songs{
        public AudioClip song;
        [Range(0, 23)] public int hourToPlayAt;
        [HideInInspector] public bool isPlaying;
    }

    [SerializeField] Songs daySong, nightSong;

    [HideInInspector] public float daylight;
   
    float secondsInDay = 86400, secondsInHour = 3600, secondsInMinute = 60;

    [Serializable] public class ColourProfile{
        public Color colourToBe;               
    }

    [SerializeField] ColourProfile[] colourForTime;
    [Serializable] public class GameTime{
        [Range(0, 24)] public int hour;
        [Range(0, 60)] public int minute, second;        
    }
    [Range(0, 86400)] public int timeOfDayInSeconds;
    [SerializeField] GameTime time;
    public float timeSpeed = 1f;

    Color previousColour;
    Color TargetColour{ get{ return colourForTime[ColourIndex].colourToBe; } }
    int colourIndex = 1;
    int ColourIndex{
        get{ return colourIndex; }
        set{
            if(value == colourIndex){ return; }
            previousColour = colourForTime[Mathf.Clamp(colourIndex, 0, colourForTime.Length-1)].colourToBe;
            colourIndex = value;
        }
    }
    

    public float Hour{
        get{ return time.hour; }
        set{ time.hour = Mathf.FloorToInt(Mathf.Clamp((value / secondsInHour) % secondsInHour, 0, 24)); }
    }
    public float Minute{
        get{ return time.minute; }
        set{ time.minute = Mathf.FloorToInt(Mathf.Clamp((value / secondsInMinute) % secondsInMinute, 0, secondsInMinute)); }
    }
    public float Second{
        get{ return time.hour; }
        set{ time.second = Mathf.FloorToInt(Mathf.Clamp(value % secondsInMinute, 0, secondsInMinute)); }
    }

    public float TimeOfDayInSeconds{
        get{ return timeOfDayInSeconds; }
        set{ timeOfDayInSeconds = Mathf.FloorToInt(Mathf.Clamp(value % secondsInDay, 0, secondsInDay)); }
    }


    float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax){
        float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
        i = Mathf.Clamp(i, toMin, toMax);
        return i;
    }

    private void Awake() {
        if(instance){
            Destroy(this);
            return;
        }
        instance = this;        
    }

    private void Update() {
        TimeOfDayInSeconds += Time.deltaTime * timeSpeed;
        Hour = TimeOfDayInSeconds;
        Minute = TimeOfDayInSeconds;
        Second = TimeOfDayInSeconds;        

        daylight = (secondsInDay/2) - Mathf.Abs((secondsInDay/2) - timeOfDayInSeconds);
        daylight = Remap(daylight, 0, (secondsInDay/2), 0.1f, 1f);        
        ScenelightSingleton.instance.SceneLight.intensity = daylight;


        float timeOfDayColour = Remap(timeOfDayInSeconds % secondsInDay/colourForTime.Length, 0, secondsInDay/colourForTime.Length, 0, colourForTime.Length);
        float rawIndex = (timeOfDayColour + 1) % colourForTime.Length;
        ColourIndex = Mathf.FloorToInt(rawIndex) % colourForTime.Length;             
        
        ScenelightSingleton.instance.SceneLight.color = Color.Lerp(previousColour, TargetColour, rawIndex % 1);

        if(!daySong.isPlaying && Hour >= daySong.hourToPlayAt && Hour < nightSong.hourToPlayAt){ MusicHandler.instance.NextSong(daySong.song); }
        else if(!nightSong.isPlaying && Hour >= nightSong.hourToPlayAt || Hour < daySong.hourToPlayAt){ MusicHandler.instance.NextSong(nightSong.song); }                

        daySong.isPlaying = (MusicHandler.instance.PlayingSource.clip == daySong.song)? true: false;
        nightSong.isPlaying = (MusicHandler.instance.PlayingSource.clip == nightSong.song)? true: false;
    }
}
