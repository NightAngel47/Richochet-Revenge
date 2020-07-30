using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankEnemyBehaviour : MonoBehaviour
{
    public const int DANGER = 3;

    public float speed;
    public float minRange;
    public float maxRange;

    public GameObject bullet;
    public GameObject[] gunPos;
    public float bulletForce = 2000f;

    [Range(0f, 2f)]
    public float fireRate;

    private Vector3 aimOffset;
    private float behaviourTimer;
    private bool behaviourType;

    private float fireTimer;

    public float health;
    public GameObject weakspot;

    public GameObject feet;

    private bool isAlive = false;

    public LevelAnalytics analytics;
    private bool reported = false;

    public Animator anim;

    public GameObject dmgTextGO;

    public WaveGenerator wg;

    public AudioSource audio;
    public AudioClip[] audioClips;

    [Range(0f, 1f)]
    public float shakeAmount = 0.1f;
    [Range(0f, 1f)]
    public float shakeDeathAmount = 0.25f;
    [Range(0f, 1f)]
    public float shakeDuration = 0.1f;

    void Awake()
    {
        analytics = GameObject.FindGameObjectWithTag("Analytics").GetComponent<LevelAnalytics>();
        audio = GetComponent<AudioSource>();
        wg = GameObject.FindGameObjectWithTag("WaveGenerator").GetComponent<WaveGenerator>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        behaviourTimer = 0;
        behaviourType = true;

        fireTimer = 0;

        analytics.IncreaseTankSpawnCount();

        Invoke("Spawn", 0.3f);
    }

    private void Spawn()
    {
        isAlive = true;
        anim.SetTrigger("Spawned");
    }

    void Update()
    {
        if (isAlive)
            Aim();

        health -= weakspot.GetComponent<WeakspotBehaviour>().getDamage();

        if (health <= 0)
        {
            GetComponent<Animator>().SetTrigger("Death");
            isAlive = false;
            GetComponent<Rigidbody2D>().isKinematic = true;
            Destroy(GetComponent<CapsuleCollider2D>());
            ScreenShake.shakeDuration = shakeDuration;
            ScreenShake.shakeAmount = shakeDeathAmount;
            Invoke("destroy", 0.5f);
        }
    }

    void FixedUpdate()
    {
        if (isAlive)
            Action();
    }

    void Action()
    {
        if(behaviourTimer <= 0)
        {
            behaviourTimer = Random.Range(1f, 1.5f);
            behaviourType = !behaviourType;
        }

        if (behaviourType)
        {
            if (fireTimer <= 0)
            {
                Vector2 move = new Vector2(0f, 0f);

                if (aimOffset.magnitude < minRange)
                {
                    move = -1 * transform.up;
                }
                else if (aimOffset.magnitude > maxRange)
                {
                    move = transform.up;
                }

                move *= speed * Time.deltaTime;
                GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + move);

                Shoot();

                fireTimer = fireRate;
            }
            else
            {
                fireTimer -= Time.deltaTime;
            }
        }
        else
        {
            Vector2 move = new Vector2(0f,0f);

            if (aimOffset.magnitude < minRange)
            {
                move = -1 * transform.up;
            }
            else if (aimOffset.magnitude > maxRange)
            {
                move = transform.up;
            }

            Vector2 moveRight = transform.right;
            move += moveRight;
            feet.GetComponent<Animator>().SetFloat("Move", move.magnitude);
            move *= speed * Time.deltaTime;
            GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + move);

            fireTimer = 0;
        }

        behaviourTimer -= Time.deltaTime;
    }

    void Aim()
    {
        aimOffset = GameObject.FindWithTag("Player").transform.position - transform.position;
        GetComponent<Rigidbody2D>().MoveRotation(-Mathf.Rad2Deg * Mathf.Atan2(aimOffset.x, aimOffset.y));
        feet.transform.rotation = transform.rotation;
        //Debug.DrawLine(transform.position, aimOffset);
    }

    void Shoot()
    {
        GameObject fired = Instantiate(bullet, gunPos[Mathf.RoundToInt(Random.value)].transform.position, Quaternion.identity);
        fired.GetComponent<Rigidbody2D>().AddForce(transform.up * bulletForce);
        audio.clip = audioClips[0];
        audio.pitch = Random.Range(.9f, .95f);
        //audio.volume = Random.Range(.3f, .35f);
        audio.Play();

        ScreenShake.shakeDuration = shakeDuration;
        ScreenShake.shakeAmount = shakeAmount;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "PlayerBullet")
        {
            ScreenShake.shakeDuration = shakeDuration;
            ScreenShake.shakeAmount = shakeAmount;

            Time.timeScale = 0.2f;
            Invoke("ResumeTime", 0.05f);

            audio.clip = audioClips[1];
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
        if(!reported)
        {
            wg.enemyCount--;
            if (wg.enemyCount == 0)
            {
                wg.NextWave();
            }

            analytics.IncreaseTankKillCount();
            reported = true;
        }
        Destroy(gameObject);
    }
}
