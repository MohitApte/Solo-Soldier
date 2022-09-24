using UnityEngine;
using System.Collections;
using CnControls;
using UnityEngine.UI;
using System;

public class WeaponManager : MonoBehaviour
{

    Animator animator;

    public Text rifleAmmoText;

    public Vector3 spineRotation;

    public int rifleBullets = 6;
    private int RifleBulletsRemaining;
    private bool isRifleReloading = false;
    private float nextFireRifle;
    private float fireRateRifle = 0.5f;



    public AudioSource rifleShot;
    public AudioSource rifleReload;

    public GameObject crosshairPrefab;

    public GameObject bulletHolePrefab;

    public GameObject rifle;

    public GameObject rifle2;

    public GameObject pistol;

    public GameObject pistol2;

    public static bool ownerAiming;

    public GameObject MuzzleFlash;

    //Ik stuff
    [SerializeField]
    public IK ik;
    [System.Serializable]
    public class IK
    {
        public Transform spine; //the bone where we rotate the body of our character from
                                //The Z/x/y values, doesn't really matter the values here since we ovveride them depending on the weapon
        public float aimingZ = 213.46f;
        public float aimingX = -65.93f;
        public float aimingY = 20.1f;
        //The point in the ray we do from our camera, basically how far the character looks
        public float point = 30;

        public bool DebugAim;
        //Help us debug the aim, basically makes it possible to change the current values 
        //on runtime since we are hardcoding them
    }

    void Start()
    {
        RifleBulletsRemaining = rifleBullets;
        animator = GetComponent<Animator>();
        rifle2.SetActive(false);
        pistol.SetActive(false);
        if (crosshairPrefab != null)
        {
            crosshairPrefab = Instantiate(crosshairPrefab);
            ToggleCrosshair(false);
        }
        MuzzleFlash.SetActive(false);
    }

    void Update()
    {
        SetupUI();
        WeaponSwitching();
        Pistol();
        Rifle();

        if (ownerAiming)
        {
            PositionCrosshair();
        }
        else
        {
            ToggleCrosshair(false);
        }
    }

    void LateUpdate()
    {

        if (Input.GetKey(KeyCode.Mouse1) || CnInputManager.GetButton("Aim"))
        {
            PositionSpine();

        }
    }

    void PositionCrosshair()
    {
        RaycastHit hit;
        Transform cam = Camera.main.transform;
        Ray ray = new Ray(cam.position, cam.forward);

        if (Physics.Raycast(ray, out hit, 200.0f))
        {

            if (crosshairPrefab != null)
            {
                ToggleCrosshair(true);
                crosshairPrefab.transform.position = hit.point;
                crosshairPrefab.transform.LookAt(cam.transform);
            }
            else
            {
                ToggleCrosshair(false);
            }
        }
        else
        {
            ToggleCrosshair(false);
        }

    }

    void CreateBulletHole()
    {
        RaycastHit hit;
        Transform cam = Camera.main.transform;
        Ray ray = new Ray(cam.position, cam.forward);


        if (Physics.Raycast(ray, out hit, 200.0f))
        {

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Object"))
                {


                    Instantiate(bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

                }
            }
        }


    }

    void ToggleCrosshair(bool enabled)
    {
        if (crosshairPrefab != null)
        {
            crosshairPrefab.SetActive(enabled);
        }

    }


    void PositionSpine()
    {
        
            ik.aimingX = -15;
            ik.aimingY = 50;
            ik.aimingZ = 0;
        
       

        Vector3 eulerAngleOffset = Vector3.zero;
        eulerAngleOffset = new Vector3(ik.aimingX, ik.aimingY, ik.aimingZ);

        //do a ray from the center of the camera and forward
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //find where the character should look
        Vector3 lookPosition = ray.GetPoint(ik.point);

        //and apply the rotation to the bone
        ik.spine.LookAt(lookPosition);
        ik.spine.Rotate(eulerAngleOffset, Space.Self);


    }
    void SetupUI()
    {
        if (rifle.activeSelf)
        {

            rifleAmmoText.text = RifleBulletsRemaining + " /  6";
        }
        else
        {
            rifleAmmoText.text = "";
        }
    }

    void WeaponSwitching()
    {
    

            if (Input.GetKeyUp(KeyCode.K))
                if (rifle.activeSelf)
                {
                    rifle.SetActive(false);
                    rifle2.SetActive(true);
                    animator.SetBool("rifleInHand", false);
                }
                else
                {
                    rifle.SetActive(true);
                    animator.SetBool("rifleInHand", true);
                    rifle2.SetActive(false);
                }

    }

    



    void Pistol()
    {
        if (pistol.activeSelf)
        {
            if (Input.GetButton("Fire2") || CnInputManager.GetButton("Fire2"))
            {
                animator.SetBool("AimingPistol", true);

            }
            else
            {
                animator.SetBool("AimingPistol", false);

            }

            if (Input.GetButton("Fire1"))
            {
                animator.SetBool("pistolShot", true);

            }
            else
            {
                animator.SetBool("pistolShot", false);

            }
        }
    }
    void Rifle()
    {
        if (rifle.activeSelf)
        {
            if (Input.GetButton("Fire1"))
            {
                if (RifleBulletsRemaining > 0)
                {
                    if (ownerAiming)
                    {
                        if (!isRifleReloading)
                        {
                            if (Time.time > nextFireRifle)
                            {
                                nextFireRifle = fireRateRifle + Time.time;
                                animator.SetBool("rifleShot", true);
                                CreateBulletHole();
                                rifleShot.Play();
                                StartCoroutine(muzzleFlash());
                                RifleBulletsRemaining--;

                            }

                        }
                    }


                }

            }
            else
            {
                animator.SetBool("rifleShot", false);
            }

            if (RifleBulletsRemaining == 0)
            {


                StartCoroutine(reloadRifle());

                RifleBulletsRemaining = rifleBullets;


            }
        }
    }


    IEnumerator reloadRifle()
    {
        isRifleReloading = true;
        animator.SetBool("isReloadingRifle", true);


        yield return new WaitForSeconds(4.1f);


        animator.SetBool("isReloadingRifle", false);
        isRifleReloading = false;

    }
    IEnumerator muzzleFlash()
    {
        MuzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        MuzzleFlash.SetActive(false);
    }

    }