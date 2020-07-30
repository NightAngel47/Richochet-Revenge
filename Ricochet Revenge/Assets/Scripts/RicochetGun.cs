using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RicochetGun : MonoBehaviour
{
    public GameObject bullet;
    public float bulletForce = 2000f;
    [Range(0f, 2f)]
    public float fireRate;
    public bool canFire = true;
    public Image coolDownImg1;
    public Image coolDownImg2;
    public float coolDown;
    [Range(0f, 10f)]
    public float coolDownMax = 6f;
    [Range(0f, 2f)]
    public float coolDownRate = 1f;
    [Range(0f, 2f)]
    public float coolDownAmount = 1f;
    [Range(0f, 2f)]
    public float fastCoolDownAmount = 1f;
    public bool mustCool = false;
    public AudioSource audio;
    [Range(0f, 1f)]
    public float shakeAmount = 0.1f;
    [Range(0f, 1f)]
    public float shakeDuration = 0.1f;

    // Update is called once per frame
    void Update()
    {

        // starts firing
        if (Input.GetButtonDown("Fire1") && canFire && GetComponentInParent<PlayerController>().isAlive)
        {
            //print("start");
            StartCoroutine(Shoot());
        }
        else if(Input.GetButtonDown("Jump") && mustCool && GetComponentInParent<PlayerController>().isAlive)
        {
            coolDown -= fastCoolDownAmount;
        }

        // updates cool down image
        if(mustCool)
        {
            coolDownImg1.fillAmount = 0;
            coolDownImg2.fillAmount = coolDown / coolDownMax;
        }
        else
        {
            coolDownImg2.fillAmount = 0;
            coolDownImg1.fillAmount = coolDown / coolDownMax;
        }
    }

    // shoots bullets
    private IEnumerator Shoot()
    {
        // spawns bullet
        SpawnBullet();
        canFire = false;

        // increase gun cool down
        coolDown += fireRate;

        yield return new WaitForSeconds(fireRate);
        canFire = true;

        // if cool down exceeds max then player can't fire
        if (coolDown >= coolDownMax)
        {
            canFire = false;
            mustCool = true;
        }

        // checks if the player is holding down fire and can fire based on cool down
        if (Input.GetButton("Fire1") && canFire && GetComponentInParent<PlayerController>().isAlive)
        {
            StartCoroutine(Shoot());
        }
        else 
        {
            StartCoroutine(CoolDown());
        }
    }

    // spawns bullet with force in a direction
    private void SpawnBullet()
    {
        GameObject fired = Instantiate(bullet, transform.position, Quaternion.identity);
        var aimDis = PlayerController.getAimOffset().magnitude;
        var aimDir = PlayerController.getAimOffset() / aimDis;
        fired.GetComponent<Rigidbody2D>().AddForce(aimDir * bulletForce);
        audio.pitch = Random.Range(.96f, 1.03f);
        //audio.volume = Random.Range(.96f, 1.03f);
        audio.Play();
        ScreenShake.shakeDuration = shakeDuration;
        ScreenShake.shakeAmount = shakeAmount;
    }

    // handles cool down
    private IEnumerator CoolDown()
    {
        // cools down if greater than 0
        if (coolDown > 0)
            coolDown -= coolDownAmount;

        if(coolDown <= 0)
        {
            mustCool = false;
            canFire = true;
        }

        // stops from going negative and resets must cool
        if (coolDown < 0)
        {
            coolDown = 0;
        }

        yield return new WaitForSeconds(coolDownRate);

        if (!Input.GetButton("Fire1"))
            StartCoroutine(CoolDown());
        else if (mustCool)
            StartCoroutine(CoolDown());
    }
}
