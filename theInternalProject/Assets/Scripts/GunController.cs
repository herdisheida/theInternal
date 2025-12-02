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
    

    void Start()
    {
        currentBullets = maxBullets;
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
        
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        currentBullets -= 1;
        // Avoid compile-time dependency on BulletController; call SetDirection at runtime instead.
        bullet.SendMessage("SetDirection", Vector2.right, SendMessageOptions.DontRequireReceiver);   
    }

    
    System.Collections.IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTimer);

        currentBullets = maxBullets;

        isReloading = false;
    }
}
