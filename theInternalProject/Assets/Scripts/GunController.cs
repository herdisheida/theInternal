using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 0.2f;
    public int maxBullets = 30;
    public float reloadTimer = 3f;
    private bool isReloading = false;
    private float timer;
    private int currentBullets;
    public GameObject leftFirePoint;
    public GameObject rightFirePoint;
    private bool isLeftFirePoint;
    

    void Start()
    {
        currentBullets = maxBullets;
        isLeftFirePoint = true;
    }
    void Update()
    {
        
        if (isReloading)
        {
            return;
        }
        else if (currentBullets == 0)
        {
            StartCoroutine(Reload());
            return;
        }
        timer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }

    }

    void Shoot()
    {
        Transform firePoint = isLeftFirePoint ? leftFirePoint.transform : rightFirePoint.transform;
        GameObject bullet = Instantiate(
            bulletPrefab, 
            firePoint.position, 
            firePoint.rotation
        );
        currentBullets--;
        // Avoid compile-time dependency on BulletController; call SetDirection at runtime instead.
        // bullet.SendMessage("SetDirection", firePoint.right, SendMessageOptions.DontRequireReceiver);   

        isLeftFirePoint = !isLeftFirePoint;
    }

    
    System.Collections.IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTimer);

        currentBullets = maxBullets;

        isReloading = false;
    }
}
