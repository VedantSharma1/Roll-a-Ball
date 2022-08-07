using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PowerUpType currentPowerUp = PowerUpType.None;

    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    public float speed = 10.0f;
    private Rigidbody playerRb;
    private float powerUpStrength = 15;

    private GameObject focalPoint;
    public bool hasPowerUp;

    public GameObject powerUpIndicator;
    public GameObject powerUpIndicatorRockets;
    public GameObject powerUpIndicatorSmash;

    //smash powerup variables, test with different values
    public float hangTime = 1;
    public float smashSpeed = 10;
    public float explosionForce = 30;
    public float explosionRadius = 30;

    bool smashing = false;
    float floorY;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward.normalized * forwardInput * speed);
        //pushback indicator
        powerUpIndicator.transform.position = transform.position + new Vector3(0 , -0.69f , 0);
        //Rocket indicator
        powerUpIndicatorRockets.transform.position = transform.position + new Vector3(0, -0.69f, 0);
        //Smash Indicator
        powerUpIndicatorSmash.transform.position = transform.position + new Vector3(0, -0.69f, 0);

        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
        }

        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerUp = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            //setting the inndicators
            if(currentPowerUp == PowerUpType.Pushback)
            {
                powerUpIndicator.gameObject.SetActive(true);
                powerUpIndicatorRockets.gameObject.SetActive(false);
                powerUpIndicatorSmash.gameObject.SetActive(false);
            }
            else if(currentPowerUp == PowerUpType.Rockets)
            {
                powerUpIndicator.gameObject.SetActive(false);
                powerUpIndicatorRockets.gameObject.SetActive(true);
                powerUpIndicatorSmash.gameObject.SetActive(false);
            }
            else if (currentPowerUp == PowerUpType.Smash)
            {
                powerUpIndicator.gameObject.SetActive(false);
                powerUpIndicatorRockets.gameObject.SetActive(false);
                powerUpIndicatorSmash.gameObject.SetActive(true);
            }

            Destroy(other.gameObject);

            if(powerupCountdown != null)
            {
                StartCoroutine(PowerUpCountDownRoutine());
            }
            powerupCountdown = StartCoroutine(PowerUpCountDownRoutine());

        }
    }

    IEnumerator PowerUpCountDownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasPowerUp = false;
        currentPowerUp = PowerUpType.None;
        powerUpIndicator.gameObject.SetActive(false);
        powerUpIndicatorRockets.gameObject.SetActive(false);
        powerUpIndicatorSmash.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy") &&currentPowerUp == PowerUpType.Pushback)
        {   //get the rigidbody of the enemy
            Rigidbody enemyRigidBody = collision.gameObject.GetComponent<Rigidbody>();
            //calculate vector where the enemy will go
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            //Apply the force to the rigidBody

            enemyRigidBody.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
            Debug.Log("Collided with " + collision.gameObject.name +
                " with powerup set to " + currentPowerUp.ToString());
        }
    }

    void LaunchRockets()
    {
        foreach(var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up,
                Quaternion.identity);
            tmpRocket.GetComponent<RocketBehavoir>().Fire(enemy.transform);
        }
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();
        //store y position before taking off
        floorY = transform.position.y;

        // Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;

        while(Time.time < jumpTime)
        {   
            //move the player ip while keeping the up velocity
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        for(int i =0; i< enemies.Length; i++)
        {
            //Apply an explosion force that originates from our position
            if(enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f
                    , ForceMode.Impulse);
            }
            smashing = false;
        }
    }
}
