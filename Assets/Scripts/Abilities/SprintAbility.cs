using System.Collections;
using UnityEngine;

public class SprintAbility : MonoBehaviour
{
    [Header("Scriptable Object Data")]
    [SerializeField] private DataTransferSO dataTransferSO;

    [Header("Game Objects")]
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private GameObject sprintStaminaBar;
    [Space]
    [Header("Options")]
    [SerializeField] private int sprintSpeed = 15;
    [SerializeField] private float maxStaminaLVL1 = 2f;
    [SerializeField] private float maxStaminaLVL2 = 4f;
    [SerializeField] private float regenSpeed = 1.5f;
    [SerializeField] private float staminaDrain = 0.5f;
    [SerializeField] private float StaminaUsedDelay = 3f;
    private float maxStamina = 0;
    private float stamina = 0;
    private float scale;
    private float walkSpeed;
    private bool running = false;
    private bool coroutineNotRunning = true;

    void Start()
    {
        // Check if the ability is unlocked
        int level = dataTransferSO.abilityLevels[1];

        if (level == 0) {
            this.enabled = false;
            sprintStaminaBar.SetActive(false);
        } else if (level == 1) {
            maxStamina = maxStaminaLVL1;
        } else if (level == 2) {
            maxStamina = maxStaminaLVL2;
        }

        // Get the player walkspeed
        walkSpeed = playerCharacter.walkSpeed;
    }

    void Update()
    {
        // Running is false
        running = false;

        // get if the player is crouched
        bool crouched = playerCharacter._requestedCrouch;

        // Unless you are holding shift, a movement key, not crouched, and not on the coroutine delay
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
                if (stamina > 0 && !crouched && coroutineNotRunning) {
                    playerCharacter.walkSpeed = sprintSpeed;
                    running = true;                    
                }
            } 
        }

        // Set the player speed to walking if not running and start the coroutine delay
        if (!running) {
            playerCharacter.walkSpeed = walkSpeed;
            if (coroutineNotRunning && stamina < 0)
                StartCoroutine(recharge());
        }

        // Use stamina if running
        if (running) {
            stamina -= Time.deltaTime * staminaDrain;
        }

        // Regen stamina
        if (stamina <= maxStamina && !running) {
            stamina += Mathf.Clamp(regenSpeed * Time.deltaTime, 0, maxStamina);
        }

        // Set the scale of the stamina bar
        scale = Mathf.Clamp(stamina / maxStamina, 0.01f, 1);
        sprintStaminaBar.transform.localScale = new Vector3(scale, 1, 1);
    }

    IEnumerator recharge () {
        coroutineNotRunning = false;
        yield return new WaitForSeconds(StaminaUsedDelay);
        coroutineNotRunning = true;
    }
}
