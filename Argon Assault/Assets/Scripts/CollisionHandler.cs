using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] ParticleSystem[] crashParticles;
    [SerializeField] GameObject[] boxColliders;
    [SerializeField] GameObject[] capsuleColliders;
    [SerializeField] GameObject[] sphereColiders;

    float loadDelay = 2.5f;
    bool isTransitioning = false;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) { return; }

        LoadLevelSequence();
    }
    void LoadLevelSequence()
    {
        isTransitioning = true;
        GetComponent<PlayerControls>().enabled = false;
        CrashSequence();
        Invoke("ReloadLevel", loadDelay);
    }

    private void CrashSequence()
    {
        foreach (ParticleSystem particle in crashParticles)
        {
            particle.Play();
        }
        foreach (GameObject box in boxColliders)
        {
            box.GetComponent<BoxCollider>().enabled = false;
        }
        foreach (GameObject capsule in capsuleColliders)
        {
            capsule.GetComponent<CapsuleCollider>().enabled = false;
        }
        foreach (GameObject sphere in sphereColiders)
        {
            sphere.GetComponent<SphereCollider>().enabled = false;
        }
        rb.useGravity = true;
        var playerCrashSFX = crashParticles[1].GetComponent<AudioSource>();
        playerCrashSFX.enabled = isTransitioning;
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
