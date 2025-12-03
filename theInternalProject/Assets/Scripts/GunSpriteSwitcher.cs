using UnityEngine;

public class GunSpriteSwitcher : MonoBehaviour
{
    public GameObject idleSpriteObject;
    public GameObject shootingSpriteObject;
    void Update()
    {
        bool isShooting = Input.GetKey(KeyCode.Space);

        idleSpriteObject.SetActive(!isShooting);
        shootingSpriteObject.SetActive(isShooting);
    }
}
