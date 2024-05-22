using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject[] deathVFX;
    [SerializeField] GameObject hitVFX;
    [SerializeField] int amountToIncrease;
    [SerializeField] int enemyHitpoints;


    ScoreBoard scoreBoard;
    GameObject parentGameObject;


    bool exploding = false;
    bool hit = false;

    private void Start()
    {
        scoreBoard = FindObjectOfType<ScoreBoard>();
        parentGameObject = GameObject.FindWithTag("SpawnAtRuntime");
        AddRigidBody();
    }

    private void AddRigidBody()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
    }


    void OnParticleCollision(GameObject other) 
    {
        enemyHitpoints -= 1;

        if (enemyHitpoints > 0)
        {
            ProcessHit();

        }
        else
        {
            EnemyDeathSequence();
        }
    }

    private void ProcessHit()
    {
        hit = true;

        GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
        vfx.transform.parent = parentGameObject.transform;

        var hitSound = hitVFX.GetComponent<AudioSource>();
        hitSound.enabled = hit;

    }

    private void EnemyDeathSequence()
    {
        scoreBoard.IncreaseScore(amountToIncrease);

        if (exploding) { return; }
        exploding = true;

        foreach (GameObject fx in deathVFX)
        {
            GameObject vfx = Instantiate(fx, transform.position, Quaternion.identity);
            vfx.transform.parent = parentGameObject.transform;
        }

        Destroy(gameObject);
    }

}
