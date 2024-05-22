using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

public class PlayerControls : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] InputAction movement;
    [SerializeField] InputAction hardRotateRight;
    [SerializeField] InputAction hardRotateLeft;
    [SerializeField] InputAction fireInner;
    [SerializeField] InputAction fireOuterRight;
    [SerializeField] InputAction fireOuterLeft;

    [Header("Particle Systems")]
    [SerializeField] ParticleSystem outerRight;
    [SerializeField] ParticleSystem outerLeft;

    [Header("Game Objects")]
    [SerializeField] GameObject[] innerLasers;
    [SerializeField] GameObject[] outerLasers;
    [SerializeField] GameObject[] laserObject;
    GameObject parentGameObject;


    // Reload scene time
    float loadDelay = 2f;

    // Player movement boundries
    float yMax = 23f;
    float yMin = -15f;
    float xRange = 30f;

    // Player movement speed from controller input
    float xThrow, yThrow;
    float movementScalar = 50f;

    // Player rotation based on movement from controller input
    float positionPitchScalar = 2f;
    float controlPitchScalar = 10f;
    float positionYawScalar = 2f;
    float controlRollScalar = 20;

    // Right/Left hard-rotation toggle
    float rollSpeed = .35f;
    float timeCount;

    private void OnEnable()
    {
        movement.Enable();
        hardRotateRight.Enable();
        hardRotateLeft.Enable();
        fireInner.Enable();
        fireOuterRight.Enable();
        fireOuterLeft.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        hardRotateRight.Disable();
        hardRotateLeft.Disable();
        fireInner.Disable();
        fireOuterRight.Disable();
        fireOuterLeft.Disable();
    }

    void Start()
    {
        parentGameObject = GameObject.FindWithTag("SpawnAtRuntime");

        foreach (GameObject laser in innerLasers)
        {
            laser.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
            Invoke("ProcessAll", loadDelay);
    }

    private void ProcessAll()
    {
        foreach (GameObject laser in innerLasers)
        {
            laser.SetActive(true);
        }

        ProcessTranslation();
        ProcessRotation();
        ProcessInnerFiring();
        ProcessOuterFiring();
    }

    void ProcessInnerFiring()
    {
        if (fireInner.IsPressed())
        {
            
            SetInnerLasersActive(true);
        }
        else
        {
            SetInnerLasersActive(false);
        }
    }
    void ProcessOuterFiring()
    {
        if (fireOuterRight.IsPressed())
        {
            ParticleSystem rightClone = Instantiate(outerRight, laserObject[0].transform.position,laserObject[0].transform.rotation);
            if (Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                rightClone.Simulate(0f, true, true);
                rightClone.Play();
            }
            rightClone.transform.parent = parentGameObject.transform;
        }
        if (fireOuterLeft.IsPressed())
        {
            ParticleSystem leftClone = Instantiate(outerLeft, laserObject[1].transform.position, laserObject[1].transform.rotation);
            if (Input.GetKeyDown(KeyCode.Joystick1Button6))
            {
                leftClone.Simulate(0.0f, true, true);
                leftClone.Play();
            }
            leftClone.transform.parent = parentGameObject.transform;
        }
    }

    void SetInnerLasersActive(bool firing)
    {
        foreach (GameObject laser in innerLasers)
        {
            var emissionModule = laser.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = firing;
        }

        var blaserSound = innerLasers[0].GetComponent<AudioSource>();
        blaserSound.enabled = firing;
    }

    void ProcessTranslation()
    {
        xThrow = movement.ReadValue<Vector2>().x;
        float xOffset = xThrow * Time.deltaTime * movementScalar;
        float rawXPos = (transform.localPosition.x + xOffset);
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        yThrow = movement.ReadValue<Vector2>().y;
        float yOffset = yThrow * Time.deltaTime * movementScalar;
        float rawYPos = (transform.localPosition.y + yOffset);
        float clampedYPos = Mathf.Clamp(rawYPos, yMin, yMax);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    void ProcessRotation()
    {
        // Pitch
        float pitchFromPosition = transform.localPosition.y * -positionPitchScalar;
        float pitchFromControl = yThrow * -controlPitchScalar;

        // Yaw
        float yawFromPosition = transform.localPosition.x * positionYawScalar;

        // Roll
        float rollFromControl = xThrow * -controlRollScalar;

        float pitch = pitchFromPosition + pitchFromControl; 
        float yaw = yawFromPosition;
        float roll;


        if (hardRotateRight.IsPressed())
        {
            HardRotateRightActivate(pitch, yaw);
            roll = transform.localRotation.z * 125f;

        }
        else if (hardRotateLeft.IsPressed())
        {
            NewMethod(pitch, yaw);
            roll = transform.localRotation.z * 125f;

        }
        else
        {
            timeCount = 0;
            roll = rollFromControl;
        }

        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
    }

    private void HardRotateRightActivate(float pitch, float yaw)
    {
        Quaternion initialRotation = transform.localRotation;
        Quaternion wantedRotation = Quaternion.Euler(pitch, yaw, -90 - (transform.rotation.z * 125f));
        transform.localRotation = Quaternion.Lerp(initialRotation, wantedRotation, timeCount * rollSpeed);
        timeCount = timeCount + Time.deltaTime;
    }

    private void NewMethod(float pitch, float yaw)
    {
        Quaternion initialRotation = transform.localRotation;
        Quaternion wantedRotation = Quaternion.Euler(pitch, yaw, 90 - (transform.rotation.z * 125f));
        transform.localRotation = Quaternion.Lerp(initialRotation, wantedRotation, timeCount * rollSpeed);
        timeCount = timeCount + Time.deltaTime;
    }

}

