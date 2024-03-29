using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressRotation : MonoBehaviour
{
    public float rotationSpeed = 20.0f;
    public float maxRotationAngle = 90f; // 최대 회전 각도
    public float minRotationAngle = 0f; // 최소 회전 각도
    bool inputAttack = false;
    bool isShot = false;
    public GameObject playerBullet;
    public GameObject shotPosition;
    Transform m_transform;
    public float shotPower = 5f;
    public float shotTime = 0.5f;

    private Quaternion defaultRotation;

    bool m_arrowUp = false;
    bool m_arrowDown = false;
    bool m_pressShield = false;
    bool m_pressAttack = false;

    private void Start()
    {
        m_transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) || m_arrowUp)
        {
            RotateObject(rotationSpeed);
        }

        if (Input.GetKey(KeyCode.DownArrow) || m_arrowDown)
        {
            RotateObject(-rotationSpeed);
        }

        // 공격 구현
        if ((Input.GetKeyDown(KeyCode.Z) || m_pressAttack) && !inputAttack)
        {
            inputAttack = true;
            ShotBullet();
        }
    }

    void RotateObject(float speed)
    {
        float currentRotation = transform.rotation.eulerAngles.z;
        currentRotation = (currentRotation + 360) % 360;
        float newRotation = Mathf.Clamp(currentRotation + speed * Time.deltaTime, minRotationAngle, maxRotationAngle);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, newRotation);
    }

    void ShotBullet()
    {
        inputAttack = false;
        Quaternion rotation = new Quaternion();
        //rotation = Quaternion.Euler(-transform.rotation.eulerAngles.z, 90f, 0f);
        rotation = Quaternion.Euler(-transform.rotation.eulerAngles.z, 90f, 0f);

        if (!isShot)
        {
            isShot = true;
            GameObject objArrow = Instantiate(playerBullet, shotPosition.transform.position, rotation);
            //objArrow.GetComponent<Rigidbody>().velocity = new Vector3(m_transform.localScale.z * shotPower, 0, 0);
            Vector3 direction = (shotPosition.transform.position - m_transform.position).normalized;
            objArrow.GetComponent<Rigidbody>().velocity = direction * shotPower;

            Invoke("ShotCoolTime", shotTime);
        }
    }

    void ShotCoolTime()
    {
        isShot = false;
    }

    public void PressUpArrow()
    {
        m_arrowUp = true;
    }

    public void UpUpArrow()
    {
        m_arrowUp = false;
    }

    public void PressDownArrow()
    {
        m_arrowDown = true;
    }
    public void UpDownArrow()
    {
        m_arrowDown = false;
    }
    public void PressAttack()
    {
        m_pressAttack = true;
    }
    public void UpAttack()
    {
        m_pressAttack = false;
    }    
}
