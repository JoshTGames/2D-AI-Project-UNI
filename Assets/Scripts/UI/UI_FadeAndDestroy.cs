using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class UI_FadeAndDestroy : MonoBehaviour{
    [SerializeField] bool doDestroy;
    public bool DoDestroy{
        get{ return doDestroy; }
        set{
            doDestroy = value;
            curTime = 0;
        }
    }
    [SerializeField] Vector3 offset;
    [SerializeField] float timeTillDestroy;
    [SerializeField] TextMeshProUGUI label;
    public Inventory_ItemStats targetObj;
    Camera cam;
    List<Image> images;

    float curTime;

    Vector3 targetPos = Vector3.zero;

    private void Awake() {
        images = GetComponentsInChildren<Image>().ToList();
        cam = Camera.main;
        targetPos = (targetObj)? targetObj.transform.position : targetPos;
    }
    void Update(){
        curTime += Time.deltaTime;

        if(targetObj){ targetPos = targetObj.transform.position; }

        transform.position = Vector3.Lerp(transform.position, (!DoDestroy)? cam.WorldToScreenPoint(targetPos + offset) : cam.WorldToScreenPoint(targetPos), curTime / timeTillDestroy);
        images.ForEach((image) => {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, (!DoDestroy)? 1 : 0, curTime / timeTillDestroy));
        });
        label.color = new Color(label.color.r, label.color.g, label.color.b, Mathf.Lerp(label.color.a, (!DoDestroy)? 1 : 0, curTime / timeTillDestroy));
        label.text = $"x{targetObj.quantityOfItem}";
        if(DoDestroy & curTime >= timeTillDestroy){ Destroy(gameObject); }
    }
}
