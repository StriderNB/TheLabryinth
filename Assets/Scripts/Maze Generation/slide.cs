using System.Collections;
using Unity.Collections;
using UnityEngine;

public class slide : MonoBehaviour
{
    [SerializeField] private float wallHalfExtents;
    [SerializeField] private Vector2 waitTime;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float slideTime;
    [Space]
    public AudioClip clip;

    private bool canRunCoroutine = true;
    private float elapsedTime;
    private Vector3 desiredLocation;

    private void Start()
    {
        desiredLocation = transform.position;
    } 

    private void Update()
    {
        // Timer is run every waitTime
        if (canRunCoroutine) {
            canRunCoroutine = false;
            StartCoroutine(Timer());
        }

        // Only move if not at the desired location
        if (transform.position != desiredLocation) {
            Move(desiredLocation);
        }
    } 
    private IEnumerator Timer () {
        yield return new WaitForSeconds(Random.Range(waitTime.x, waitTime.y));
        desiredLocation = Random.Range (0,2) == 0 ? CalculateMoveLeft(wallHalfExtents * 2) : CalculateMoveRight(wallHalfExtents * 2); // Sets desiredLocation to a random Vector3 from MoveLeft or MoveRight
        canRunCoroutine = true; // Allows the Coroutine to be run again
    }

    // Returns a vector to the right if it can move right
    private Vector3 CalculateMoveRight (float distance) {
        if (CanMoveRight()) {
            if (transform.localEulerAngles.y == 0) {
                elapsedTime = 0; // Resets the elapsedTime so that the wall lerps properly
                return new Vector3 (transform.position.x, transform.position.y, transform.position.z - distance); // Move right in z axis if the object has the correct rotation
            } else if (transform.localEulerAngles.y == 90) {
                elapsedTime = 0;
                return new Vector3 (transform.position.x  - distance, transform.position.y, transform.position.z); // Move right in x axis if object has correct rotation
            } else {
                Debug.LogError("Wall prefabs rotation has to be 0 or 90 in order for the slide script to function properly, the walls current rotation is " + transform.transform.localEulerAngles.y); // Else the rotation is not correct
                return transform.position;
            }
        } else 
            return transform.position; // If it can't move right it returns it's current position
    }

    // Returns a vector to the left if it can move left
    private Vector3 CalculateMoveLeft (float distance) {
        if (CanMoveLeft()) {
            if (transform.localEulerAngles.y == 0) {
                elapsedTime = 0;
                return new Vector3 (transform.position.x, transform.position.y, transform.position.z + distance); // Move left in z axis if the object has the correct rotation
            } else if (transform.transform.localEulerAngles.y == 90) {
                elapsedTime = 0;
                return new Vector3 (transform.position.x  + distance, transform.position.y, transform.position.z); // Move left in x axis if object has correct rotation
            } else {
                Debug.LogError("Wall prefabs rotation has to be 0 or 90 in order for the slide script to function properly, the walls current rotation is " + transform.transform.localEulerAngles.y); // Else the rotation is not correct
                return transform.position;
            }
        } else 
            return transform.position; // If it can't move right it returns it's current position
    }

    // Checks if there are any objects to the right
    private bool CanMoveRight () {
        if (Physics.Raycast(transform.position, -transform.forward, wallHalfExtents * 2, mask)) {
            return false;
        } else 
            return true;
    }

    // Checks if there are any objects to the left
    private bool CanMoveLeft () {
        if (Physics.Raycast(transform.position, transform.forward, wallHalfExtents * 2, mask)) {
            return false;
        } else 
            return true;
    }

    // Slides the wall in the given direction over time using lerp
    private void Move(Vector3 destination) {
        bool canPlaySound = true;
        float x = 0;

        if (canPlaySound) {
            //AudioManager.instance.PlaySound(clip, transform, 1f);  
            canPlaySound = false;
        }

        if (Mathf.Round(x) == 1) {
            canPlaySound = true;
        }
        
        elapsedTime += Time.deltaTime;
        x = elapsedTime/slideTime;
        transform.position = Vector3.Lerp(transform.position, destination, x);
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * wallHalfExtents * 2);
        Gizmos.DrawRay(transform.position, -transform.forward * wallHalfExtents * 2);
    }
    #endif
}
