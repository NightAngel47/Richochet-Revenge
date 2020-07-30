using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // movement and aim
    public Rigidbody2D rb;
    public float speed;
    private static Vector2 aimOffset;
    public bool isAlive = true;

    // health and shields
    [Range(1, 500f)]
    public float healthMAX = 100f;
    [Range(1, 500f)]
    public float shieldsMAX = 100f;
    public Slider healthSlider;
    public Slider shieldsSlider;
    [Range(1f, 25f)]
    public float shieldPowerUpAmount;

    public SceneSwitcher sceneSwitcher;

    public Animator anim;
    public GameObject feet;

    public Texture2D crosshairTexture;

    public LevelAnalytics analytics;

    public AudioSource audio;
    public AudioClip[] audioClips;

    private void Awake()
    {
        rb.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sceneSwitcher = GameObject.FindGameObjectWithTag("SceneSwitcher").GetComponent<SceneSwitcher>();

        Time.timeScale = 1;
        isAlive = true;
    }

    void Start()
    {
        // setup health and shields sliders
        healthSlider.maxValue = healthMAX;
        shieldsSlider.maxValue = shieldsMAX;
        healthSlider.value = healthMAX;
        shieldsSlider.value = 50;

        Cursor.SetCursor(crosshairTexture, new Vector2(16, 16), CursorMode.Auto);
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (isAlive)
            Aim();
    }

    void FixedUpdate()
    {
        if (isAlive)
            Movement();
    }

    // handles wasd and arrow key movement
    void Movement()
    {
        float xMove = Input.GetAxis("Horizontal");
        float yMove = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(xMove, yMove);
        feet.GetComponent<Animator>().SetFloat("Move", move.magnitude);
        move *= speed * Time.deltaTime;
        rb.MovePosition(rb.position + move);
    }

    // makes player look at mouse pos.
    void Aim()
    {
        aimOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.rotation = Quaternion.Euler(0, 0, -Mathf.Rad2Deg * Mathf.Atan2(aimOffset.x, aimOffset.y));
        feet.transform.rotation = transform.rotation;
        //Debug.DrawLine(transform.position, aimOffset);
    }

    // returns aimoffset
    public static Vector2 getAimOffset()
    {
        return aimOffset;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ShieldPowerUp"))
        {
            shieldPowerUp();
            Destroy(collision.gameObject);
        }
    }

    // take damage from melee enemies
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemySword"))
        {
            TakeDamage(collision.GetComponent<EnemySword>().damage);
            collision.GetComponent<EnemySword>().damage = 0;
        }
    }

    // take damage from enemy bullets
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("EnemyBullet"))
        {
            TakeDamage(collision.collider.GetComponent<BulletBehaviour>().damage);
        }
    }

    // subtract damage taken from shields then health
    private void TakeDamage(float damage)
    {
        CancelInvoke("resetHit");
        GetComponent<Animator>().SetBool("Hit", true);

        audio.clip = audioClips[1];
        if(!audio.isPlaying && isAlive)
        {
            audio.Play();
        }
        if (shieldsSlider.value > 0)
        {
            shieldsSlider.value -= damage;
            if(shieldsSlider.value <= 0)
            {
                audio.clip = audioClips[0];
                audio.Play();
            }
        }
        else if (healthSlider.value > 0)
        {
            healthSlider.value -= damage;
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Death");
            isAlive = false;
            Invoke("gameOver", 1f);
        }
        Invoke("resetHit", 0.25f);
    }

    private void shieldPowerUp()
    {
        shieldsSlider.value += shieldPowerUpAmount;
    }

    private void gameOver()
    {
        analytics.SetLevelPlayerState(LevelAnalytics.LevelPlayState.Over);
        sceneSwitcher.switchScenes("GameOver");
    }

    private void resetHit()
    {
        anim.SetBool("Hit", false);
    }

    void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
