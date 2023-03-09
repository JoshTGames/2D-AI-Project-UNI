using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class UI_FollowTarget : MonoBehaviour{
    [SerializeField] Transform target;
    public Transform Target{
        get{ return target; }
        set{
            target = value;
            transform.position = value.position;
        }
    }
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothingLatency;
    Vector3 smoothingRef;

    private void Awake() => transform.position = (target)? target.position : transform.position;
    private void LateUpdate() {
        if(!target){ return; }
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref smoothingRef, smoothingLatency);
    }
}
