using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WallDestroyAbility : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private DataTransferSO dataTransferSO;
    [Header("Options")]
    [SerializeField] private LayerMask layerMask;
    private int maxShots = 0;
    private int usedShots = 0;
    private float lastShootTime = 0;

    void Start()
    {
        // Check if the ability is unlocked
        int level = dataTransferSO.abilityLevels[2];
        if (level == 0) {
            this.enabled = false;
        } else if (level == 1) {
            maxShots = 0;
        } else if (level == 2) {
            maxShots = 1;
        }

        #if UNITY_EDITOR
        //maxShots = 10000;
        #endif
    }

    void Update()
    {
        float time = Time.time;
        RaycastHit hit;
        
        // Use the ability if you have shots left
        if (Input.GetKey(KeyCode.Alpha1)) {
            if (time - lastShootTime < 1)
                return;

            lastShootTime = time;

            if (usedShots >= maxShots)
                this.enabled = false;

            Vector3 shootDir = playerCharacter.transform.TransformDirection(Vector3.forward);
            Vector3 shootPos = playerCharacter.transform.position;
            Vector3 higherShootPos = new Vector3 (shootPos.x, shootPos.y + 1, shootPos.z);

            Physics.Raycast(higherShootPos, shootDir, out hit, Mathf.Infinity, layerMask);
            if (hit.collider != null) {
                Destroy(hit.collider.GameObject());
                usedShots++;
            }
                

            Debug.DrawRay(higherShootPos, shootDir, Color.black, 1f);;
        }
    }
}
