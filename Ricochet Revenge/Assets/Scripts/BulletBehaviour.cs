using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletBehaviour : MonoBehaviour
{
    public int bounceCount = 5;
    public int bounces = 0;
    public float damage;
    public float damageIncrease;

    [Range(0f, 1f)]
    public float shakeAmount = 0.1f;
    [Range(0f, 1f)]
    public float shakeDuration = 0.1f;

    public TrailRenderer tr;
    public Gradient[] colors;

    void Start()
    {
        Physics2D.IgnoreLayerCollision(8, 8, true);
        if(GameObject.FindWithTag("Level").GetComponent<Tilemap>().GetColliderType(GameObject.FindWithTag("Level").GetComponent<Tilemap>().WorldToCell(transform.position)) != Tile.ColliderType.None)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(bounceCount <= 0)
        {
            Destroy(gameObject);
        }
        else
        {

            ScreenShake.shakeDuration = shakeDuration;
            ScreenShake.shakeAmount = shakeAmount;

            bounceCount--;
            bounces++;
            if(bounces < colors.Length)
                tr.colorGradient = colors[bounces];

            if(collision.gameObject.tag != "Enemy")
            {
                damage += damageIncrease;
            }
        }
    }
}
