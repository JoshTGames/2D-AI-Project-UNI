using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class UI_MainMenu : MonoBehaviour{
    GameObject mainFrame = null;

    [SerializeField] List<GameObject> extraWindows = new List<GameObject>();
    private void Awake() {
        mainFrame = (mainFrame)? mainFrame : transform.GetChild(0).gameObject;
        Time.timeScale = (mainFrame.activeSelf)? 0 : 1;
    }
    
    void Update(){
        Time.timeScale = (mainFrame.activeSelf)? 0 : 1;


        if(Input.GetKeyDown(KeyCode.Escape)){ 
            mainFrame.SetActive(!mainFrame.activeSelf); 
            mainFrame.transform.GetChild(1).gameObject.SetActive(mainFrame.activeSelf);
            
            extraWindows.ForEach((window) => {
                window.SetActive((!mainFrame.activeSelf) ? false : window.activeSelf);
            });
        }
    }


    public void ExitApplication() => Application.Quit();
}
