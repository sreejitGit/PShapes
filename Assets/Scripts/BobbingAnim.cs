using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnim : MonoBehaviour {
    float originalY;

    public float floatStrength = 1; 

    bool initDone = false;
    public void Init() {
        this.originalY = this.transform.position.y;
        initDone = true;
    }

    void Update() {
        if (!initDone) { return; }

        transform.position = new Vector3(transform.position.x,
            originalY + ((float)Math.Sin(Time.time) * floatStrength),
            transform.position.z);
    }

}
