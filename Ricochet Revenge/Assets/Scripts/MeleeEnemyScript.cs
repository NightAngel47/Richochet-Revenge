using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeEnemyScript : MonoBehaviour
{
    public const int DANGER = 1;

    public float speed;
    public static Vector3 aimOffset;

    public float health;
    public GameObject weakspot;

    public EnemySword sword;

    public GameObject feet;

    private bool isAlive = false;

    public WaveGenerator wg;

    public LevelAnalytics analytics;
    private bool reported = false;

    public AudioSource audio;

    public Animator anim;

    public GameObject dmgTextGO;

    [Range(0f, 1f)]
    public float shakeAmount = 0.1f;
    [Range(0f, 1f)]
    public float shakeDeathAmount = 0.25f;
    [Range(0f, 1f)]
    public float shakeDuration = 0.1f;

    void Awake()
    {
        analytics = GameObject.FindGameObjectWithTag("Analytics").GetComponent<LevelAnalytics>();
        wg = GameObject.FindGameObjectWithTag("WaveGenerator").GetComponent<WaveGenerator>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        analytics.IncreaseMeleeSpawnCount();
        Invoke("Spawn", 0.3f);
    }

    private void Spawn()
    {
        isAlive = true;
        anim.SetTrigger("Spawned");
    }

    void Update()
    {
        if(isAlive)
            Aim();

        health -= weakspot.GetComponent<WeakspotBehaviour>().getDamage();

        if(health <= 0)
        {
            GetComponent<Animator>().SetTrigger("Death");
            isAlive = false;
            GetComponent<Rigidbody2D>().isKinematic = true;
            Destroy(GetComponent<CapsuleCollider2D>());
            Destroy(GetComponentInChildren<CapsuleCollider2D>());
            ScreenShake.shakeDuration = shakeDuration;
            ScreenShake.shakeAmount = shakeDeathAmount;
            Invoke("destroy", 0.5f);
        }
    }

    void FixedUpdate()
    {
        if(isAlive)
            Movement();
    }

    void Movement()
    {
        Vector2 move;
        if (sword.collidingRecency > 0)
        {
            move = transform.right;
            sword.collidingRecency -= Time.deltaTime;
        }
        else
        {
            move = transform.up;
        }
        feet.GetComponent<Animator>().SetFloat("Move", move.magnitude);
        move *= speed * Time.deltaTime;
        GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + move);
    }

    void Aim()
    {
        aimOffset = GameObject.FindWithTag("Player").transform.position - transform.position;
        GetComponent<Rigidbody2D>().MoveRotation(-Mathf.Rad2Deg * Mathf.Atan2(aimOffset.x, aimOffset.y));
        feet.transform.rotation = transform.rotation;
        //Debug.DrawLine(transform.position, aimOffset);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "PlayerBullet")
        {
            ScreenShake.shakeDuration = shakeDuration;
            ScreenShake.shakeAmount = shakeAmount;

            Time.timeScale = 0.2f;
            Invoke("ResumeTime", 0.05f);

            if (!audio.isPlaying && isAlive)
            {
                audio.Play();
            }

            GameObject DT = Instantiate(dmgTextGO, transform.position, Quaternion.identity);

            float tempDam = weakspot.GetComponent<WeakspotBehaviour>().getDamage();

            if (tempDam > 0)
            {
                health -= tempDam;
                dmgTextGO.GetComponentInChildren<Text>().text = (tempDam * 100).ToString();
            }
            else
            {
                float damage = col.gameObject.GetComponent<BulletBehaviour>().damage;
                health -= damage;
                dmgTextGO.GetComponentInChildren<Text>().text = (damage * 100).ToString();
            }

            Destroy(DT, 0.25f);

            Destroy(col.gameObject);
        }
    }

    private void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    private void destroy()
    {

        if (!reported)
        {
            wg.enemyCount--;
            if(wg.enemyCount == 0)
            {
                wg.NextWave();
            }

            analytics.IncreaseMeleeKillCount();
            reported = true;
        }
        Destroy(gameObject);
    }
}