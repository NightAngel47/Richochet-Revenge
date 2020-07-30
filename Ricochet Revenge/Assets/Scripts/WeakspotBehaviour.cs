using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakspotBehaviour : MonoBehaviour
{
    public float damageMultiplier;

    private float detectedDamage;

    public float getDamage()
    {
        float tempDam = detectedDamage;
        detectedDamage = 0;
        return tempDam;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "PlayerBullet")
        {
            detectedDamage += damageMultiplier*col.gameObject.GetComponent<BulletBehaviour>().damage;
            Destroy(col.gameObject);
        }
    }
}
