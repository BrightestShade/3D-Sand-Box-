using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModularGunScript : MonoBehaviour
{
    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineCapacity, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    // Graphics
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;

    public TextMeshProUGUI text;
    public Text counterText;
    public float counter;

  
        
    
    private void Awake()
    {
        bulletsLeft = magazineCapacity;
        readyToShoot = true;

        //set the ammo count
        text.SetText(bulletsLeft + " / " + magazineCapacity);

    }



    private void Update() 
    {
        MyInput();
    }



    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineCapacity && !reloading) Reload();
        
        //shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            Shoot();
        }
    }
    
     
   



  
    private void Shoot()
    {
        readyToShoot = false;

        //spread 
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate projectile direction with spread
        Vector3 direction = fpsCam .transform.forward + new Vector3(x, y, 0);

        // Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {

            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                // rayHit.collider.GetComponent<EnemyAi>().TakeDamage
                Debug.Log("Enemy hit");
            }

        }

        //shake the cam
        camShake.Shake(camShakeDuration, camShakeMagnitude);

        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

      bulletsLeft--;

        Invoke("ResetShoot", timeBetweenShooting);
    }

    private void ResetShoot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);

    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineCapacity;
        reloading = false;
    }
}
