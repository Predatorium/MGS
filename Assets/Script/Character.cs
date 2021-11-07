using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] private Text AmmoText = null;
    [SerializeField] private Text LifeText = null;
    [SerializeField] private Image LifeBarre = null;
    [SerializeField] private Rigidbody Body = null;
    [SerializeField] private float MoveSpeed = 10f;
    [SerializeField] private Ammo ammo = null;
    [SerializeField] private int damage = 1;
    [SerializeField] private int MaxAmmoInWeapon = 10;
    [SerializeField] private int TotalAmmo = 0;
    [SerializeField] private BoxCollider colliderWin = null;
    public Camera ActivCam = null;
    private int AmmoInWeapon = 0;

    [SerializeField] private int MaxLife = 10;
    public int Life = 0;

    [SerializeField] private float FireRate = 2f;
    private float timeFire = 0f;

    [SerializeField] private float TimeForReload = 2f;
    private float timeReload = 0f;
    private bool Reload = false;

    public bool Win = false;

    // Start is called before the first frame update
    void Start()
    {
        Life = MaxLife;
        AmmoInWeapon = MaxAmmoInWeapon;
        AmmoText.text = "Munition\n" + AmmoInWeapon.ToString() + "/" + TotalAmmo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0f)
        {
            if (Mathf.Abs(ActivCam.transform.rotation.z) == 0)
            {
                Vector3 Velo = new Vector3(Input.GetAxis("Horizontal"), Body.velocity.y, Input.GetAxis("Vertical")).normalized * MoveSpeed;
                Body.velocity = Velo;
                transform.LookAt(transform.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")));
            }
            else if (ActivCam.transform.rotation.z == -0.5)
            {
                Vector3 Velo = new Vector3(Input.GetAxis("Vertical"), Body.velocity.y, -Input.GetAxis("Horizontal")).normalized * MoveSpeed;
                Body.velocity = Velo;
                transform.LookAt(transform.position + new Vector3(Input.GetAxisRaw("Vertical"), 0f, -Input.GetAxisRaw("Horizontal")));
            }
            else if (ActivCam.transform.rotation.z == 0.5)
            {
                Vector3 Velo = new Vector3(-Input.GetAxis("Vertical"), Body.velocity.y, Input.GetAxis("Horizontal")).normalized * MoveSpeed;
                Body.velocity = Velo;
                transform.LookAt(transform.position + new Vector3(-Input.GetAxisRaw("Vertical"), 0f, Input.GetAxisRaw("Horizontal")));
            }

            timeFire += Time.deltaTime;

            if (Reload) 
            {
                timeReload += Time.deltaTime;

                if (timeReload > TimeForReload)
                {
                    Reload = false;
                    timeReload = 0f;

                    if (TotalAmmo > MaxAmmoInWeapon)
                    {
                        TotalAmmo -= MaxAmmoInWeapon - AmmoInWeapon;
                        AmmoInWeapon += MaxAmmoInWeapon - AmmoInWeapon;
                    }
                    else
                    {
                        TotalAmmo -= TotalAmmo - AmmoInWeapon;
                        AmmoInWeapon += TotalAmmo - AmmoInWeapon;
                    }

                    AmmoText.text = "Munition\n" + AmmoInWeapon.ToString() + "/" + TotalAmmo.ToString();
                }
            }

            if (Input.GetButtonDown("Fire2") && timeFire > 1f / FireRate && AmmoInWeapon > 0)
            {
                Shoot();
                AmmoInWeapon--;
                timeFire = 0f;

                AmmoText.text = "Munition\n" + AmmoInWeapon.ToString() + "/" + TotalAmmo.ToString();
            }
            if (Input.GetButtonDown("Fire3") && !Reload && TotalAmmo > 0)
            {
                Reload = true;
            }

            LifeBarre.fillAmount = Life / MaxLife;
            LifeText.text = Life.ToString() + "/" + MaxLife.ToString();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AmmoInWorld tmp = collision.gameObject.GetComponent<AmmoInWorld>();
        if (tmp != null)
        {
            TotalAmmo += tmp.Ammo;
            AmmoText.text = "Munition\n" + AmmoInWeapon.ToString() + "/" + TotalAmmo.ToString();
            Destroy(tmp.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == colliderWin.gameObject)
        {
            Win = true;
        }
    }

    public void TakeDamages(int damages)
    {
        Life -= damages;

        if (Life <= 0)
        {
            Life = 0;
        }

        if (Life > MaxLife)
        {
            Life = MaxLife;
        }
    }

    private void Shoot()
    {
        Ammo tmpAmmo = Instantiate(ammo);
        tmpAmmo.Damage = damage;
        tmpAmmo.transform.position = transform.position + transform.forward;
        tmpAmmo.transform.LookAt(tmpAmmo.transform.position + transform.forward);
        tmpAmmo.transform.Rotate(new Vector3(90f, 0f, 0f));
        tmpAmmo.Body.AddForce(transform.forward * 10f, ForceMode.Impulse);
    }
}
