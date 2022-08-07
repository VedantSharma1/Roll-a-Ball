using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehavoir : MonoBehaviour
{   
    
    private Transform target;
    private float speed = 15.0f;
    private bool homing;

    private float rocketStrength = 15.0f;
    private float aliveTimer = 4.0f;
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        if (homing && target != null)
        {
            Vector3 moveDirection = (target.transform.position - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.LookAt(target);
        }
    }

    public void Fire(Transform newTarget)
    {
        target = newTarget;
        homing = true;
        Destroy(gameObject, aliveTimer);
    }

    //When thers a collision
    private void OnCollisionEnter(Collision col)
    {
      if(target != null)
        {   //Apply force in the oppposite directions and then delte the missiles
            if (col.gameObject.CompareTag(target.tag))
            {
                Rigidbody targetRigidbody = col.gameObject.GetComponent<Rigidbody>();
                Vector3 away = -col.contacts[0].normal;
                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse);
                Destroy(gameObject);
            }
        }  
    }
}
