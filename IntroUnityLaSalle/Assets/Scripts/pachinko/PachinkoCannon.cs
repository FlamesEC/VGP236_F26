using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PachinkoCannon : MonoBehaviour
{
    public GameObject ball;
    public AnimatedObject animator;
    public Transform spawn;

    public float rotationBound = 45.0f;
    public float rotationSpeed = 2;

    public float positionBound = 7;
    public float positionSpeed = 2;

    public GameObject anchorMover;
    public GameObject ballRuntime;

    public float throwForce = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void CannonRotation()
    {
        float rotationAddition = Input.GetAxis("Vertical") * Time.deltaTime * rotationSpeed;
        float currentRot = Mathf.DeltaAngle(0f, transform.rotation.eulerAngles.z);
        float newRot = rotationAddition + currentRot;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Clamp(newRot, -rotationBound, rotationBound));
    }

    public void CannonPosition()
    {
        float positionAddition = Input.GetAxis("Horizontal") * Time.deltaTime * positionSpeed;
        float newPos = transform.position.x + positionAddition;

        float x = Mathf.Clamp(newPos, -positionBound, positionBound);

        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        anchorMover.transform.position = new Vector3(x, anchorMover.transform.position.y, anchorMover.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (ballRuntime != null || !Pachinko.instance.CanPlay())
            return;

        CannonRotation();
        CannonPosition();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.changeAnimation(0);
            animator.changeAnimation(1);

            StopAllCoroutines();
            StartCoroutine(LateAppear());
        }
    }

    IEnumerator LateAppear()
    {
        yield return new WaitForSeconds(0.15f);
        transform.localScale = Vector3.one * 1.1f;

        yield return new WaitForSeconds(0.05f);

        Vector2 alteration = Vector2.zero;
        if (transform.rotation.eulerAngles.z == 0)
            alteration = new Vector2(Random.Range(0.01f, 0.001f) * (Random.Range(1.0f, 100.0f) >= 50 ? 1 : -1), 0);

        Rigidbody2D rb = Instantiate(ball, spawn.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        rb.AddForce(GetThrowForce(Mathf.DeltaAngle(0f, transform.rotation.eulerAngles.z)) + alteration, ForceMode2D.Impulse);
        ballRuntime = rb.gameObject;
        AudioManager.instance.PlaySFX("sfx_tile");
        AudioManager.instance.PlaySFX("sfx_damage");

        yield return new WaitForSeconds(0.01f);
        transform.localScale = Vector3.one;
        Pachinko.instance.RemoveLife();
    }
    
    Vector2 GetThrowForce(float angle)
    {
        float angleRad = (angle - 90) * Mathf.Deg2Rad;

        // Crear la dirección en 2D según el ángulo
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        print(direction);

        return direction * throwForce;
    }
}
