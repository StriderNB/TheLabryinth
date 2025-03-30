using System.Collections;
using UnityEngine;

public class TrapShoot : MonoBehaviour
{
    [SerializeField] private GameObject killBox;
    [SerializeField] private Vector2 shootLength = new Vector2 (1, 3);
    [SerializeField] private Vector2 rechargeDelay = new Vector2 (2, 4);
    private bool canShoot = true;
    public GameObject[] firespouts = new GameObject[]{};
    private void Update()
    {
        if (canShoot) {
            canShoot = false;
            StartCoroutine(shoot());
        }
        
    }

    private IEnumerator shoot () {
        // Turn big flames on
        foreach (var item in firespouts) {
            item.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            item.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        GameObject box = Instantiate(killBox, new Vector3 (transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);

        float duration = Random.Range(shootLength.x, shootLength.y);

        yield return new WaitForSeconds(duration);
        Destroy(box);
        // Turn big flames off
        foreach (var item in firespouts) {
            item.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            item.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(Random.Range(rechargeDelay.x, rechargeDelay.y));

        canShoot = true;
    }
}
