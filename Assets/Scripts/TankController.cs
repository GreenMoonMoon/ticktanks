using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TankController : MonoBehaviour, IPowerable, IDamagable
{
    Rigidbody rb;
    float movementInput, turnInput, aimInput;
    float nextBoost;
    float charge, chargeRatio;
    bool canLaunch;
    string verticalInputName, horizontalInputName, boostInputName, launchInputName, aimInputName;
    new Collider collider;
    int aLaunchTrigger;

    public int playerID;
    public float health;
    public float moveSpeed;
    public float aimSpeed;
    public float turnDamping;
    public float boostForce, boostCooldown;
    public float minLaunchForce, maxLaunchForce, launchCooldown;
    public GameObject shellPrefab;
    public Transform launchPoint;
    public Slider boostSlider;
    public Slider launchSlider;
    public Animator anim;
    public Transform aimTransform;

    public float driftCoefficient;
    public float upwardForce;
   
    void Awake()
    {
        verticalInputName = "Vertical" + playerID.ToString();
        horizontalInputName = "Horizontal" + playerID.ToString();
        boostInputName = "Boost" + playerID.ToString();
        launchInputName = "Launch" + playerID.ToString();
        aimInputName = "Aim" + playerID.ToString();
        movementInput = turnInput = 0;
        nextBoost = Time.time;
        rb = GetComponent<Rigidbody>();
        boostSlider.maxValue = boostSlider.value = boostCooldown;
        charge = minLaunchForce;
        chargeRatio = (maxLaunchForce - minLaunchForce) / launchCooldown;
        launchSlider.maxValue = maxLaunchForce - minLaunchForce;
        canLaunch = true;
        collider = GetComponent<Collider>();
        aLaunchTrigger = Animator.StringToHash("launch");
    }

    private void Update()
    {
        movementInput = Input.GetAxis(verticalInputName);
        turnInput = Input.GetAxis(horizontalInputName);
        aimInput = Input.GetAxis(aimInputName);

        aimTransform.Rotate(Vector3.right, aimInput * aimSpeed * Time.deltaTime);

        if (Input.GetButtonDown(boostInputName) && nextBoost < Time.time)
        {
            nextBoost = Time.time + boostCooldown;
            boostSlider.value = 0.0f;
            if (rb.velocity.magnitude > 0.5f)
                rb.AddForce(rb.velocity + (rb.velocity.normalized * boostForce), ForceMode.VelocityChange);
            else
                rb.AddForce(transform.forward * boostForce, ForceMode.VelocityChange);
        }
        boostSlider.value += Time.deltaTime;

        if (Input.GetButtonDown(launchInputName) && canLaunch)
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
        anim.SetTrigger(aLaunchTrigger);
        GameObject shell = Instantiate(shellPrefab, launchPoint.position, Quaternion.identity);
        Physics.IgnoreCollision(shell.GetComponent<Collider>(), collider);

        shell.GetComponent<Shell>().safeCollider = collider;
        shell.GetComponent<Rigidbody>().AddForce((Vector3.up * upwardForce) + (launchPoint.up * charge), ForceMode.VelocityChange);
    }

    IEnumerator Charge()
    {
        while (charge < maxLaunchForce)
        {
            if (!Input.GetButton(launchInputName))
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
                moveSpeed *= 2f;
                turnDamping *= 0.5f;
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
