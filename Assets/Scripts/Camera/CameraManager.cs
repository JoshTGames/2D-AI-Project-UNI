using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class CameraManager : MonoBehaviour{
    [SerializeField] Transform target;
    [SerializeField] Vector2 targetPosition, offsetPosition;
    [SerializeField] Tilemap bounds;

    [SerializeField] float smoothingSpeed;
    Vector3 smoothVel;

    Camera cam;

    private void Awake() => cam = Camera.main;
    
    void FixedUpdate(){
        if(target){ targetPosition = target.position; }
        targetPosition += offsetPosition;

        Vector3 boundsSize = Vector3.Scale(((Vector3)bounds.size / 2), bounds.cellSize);

        float camSizeX = cam.orthographicSize * cam.aspect;
        float camSizeY = cam.orthographicSize;        
        Vector3 clampedPos = ClampedPosition.GetClampedPos(targetPosition, bounds.transform.position, boundsSize, camSizeX, camSizeY);
        clampedPos.z = -10;

        Vector3 newPos = Vector3.SmoothDamp(transform.position, clampedPos, ref smoothVel, smoothingSpeed * Time.fixedDeltaTime);
        transform.position = new Vector3(newPos.x,newPos.y, -10);
    }
}
