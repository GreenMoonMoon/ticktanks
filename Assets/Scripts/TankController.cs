using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TankController : MonoBehaviour, IPowerable, IDamagable
{
    Rigidbody rb;
    float movementInput, turnInput, turnDamping;
    float nextBoost;
    float charge, chargeRatio;
    bool canLaunch;

    public float health;
    public float moveSpeed;
    public float boostForce, boostCooldown;
    public float minLaunchForce, maxLaunchForce, launchCooldown;
    public GameObject shellPrefab;
    public Transform launchPoint;
    public Slider boostSlider;
    public Slider launchSlider;

    public float driftCoefficient;
    public float upwardForce;
   
    void Awake()
    {
        movementInput = turnInput = 0;
        turnDamping = 0.1f;
        nextBoost = Time.time;
        rb = GetComponent<Rigidbody>();
        boostSlider.maxValue = boostSlider.value = boostCooldown;
        charge = minLaunchForce;
        chargeRatio = (maxLaunchForce - minLaunchForce) / launchCooldown;
        launchSlider.maxValue = maxLaunchForce - minLaunchForce;
        canLaunch = true;
    }

    private void Update()
    {
        movementInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Boost") && nextBoost < Time.time)
        {
            nextBoost = Time.time + boostCooldown;
            boostSlider.value = 0.0f;
            if (rb.velocity.magnitude > 0.5f)
                rb.AddForce(rb.velocity + (rb.velocity.normalized * boostForce), ForceMode.VelocityChange);
            else
                rb.AddForce(transform.forward * boostForce, ForceMode.VelocityChange);
        }
        boostSlider.value += Time.deltaTime;

        if (Input.GetButtonDown("Launch") && canLaunch)
        {
            canLaunch = false;
            StartCoroutine(Charge());
        }
    }

    void FixedUpdate()
    {
        rb.angularDrag = 3 + (rb.velocity.magnitude * -driftCoefficient);
        rb.AddTorque(transform.up * (turnInput * moveSpeed * turnDamping), ForceMode.Acceleration);

        rb.AddForce(transform.forward * movementInput * moveSpeed, ForceMode.Acceleration);
    } 

    void Launch()
    {
        GameObject shell = Instantiate(shellPrefab, launchPoint.position, Quaternion.identity);
        shell.GetComponent<Rigidbody>().AddForce((Vector3.up * upwardForce) + (transform.forward * charge), ForceMode.VelocityChange);
    }

    IEnumerator Charge()
    {
        while (charge < maxLaunchForce)
        {
            if (!Input.GetButton("Launch"))
                break;
            charge += chargeRatio * Time.deltaTime;
            launchSlider.value = charge - minLaunchForce;
            yield return null;
        }

        Launch();
        charge = minLaunchForce;
        launchSlider.value = 0.0f;
        canLaunch = true;
    }

    void Destruct()
    {
        gameObject.SetActive(false);
    }

    public void Powerup(string powerUpType)
    {
        switch (powerUpType)
        {
            case "speed":
                moveSpeed *= 2;
                break;
            case "launch":
                launchCooldown /= 2;
                break;
        }
    }

    public void Damage()
    {
        health -= 1;
        if (health <= 0) {
            Destruct();
        }
    }
}
