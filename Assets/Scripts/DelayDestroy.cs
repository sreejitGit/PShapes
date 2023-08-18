using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour
{
    public void Init(float seconds) {
        StartCoroutine(DestroyAfter(seconds));
    }

    IEnumerator DestroyAfter(float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
