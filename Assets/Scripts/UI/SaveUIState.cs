using System.Collections.Generic;
using UnityEngine;

public class SaveUIState : MonoBehaviour
{
    [SerializeField] private List<Animator> UI = new List<Animator>();

    void Start()
    {
        foreach (Animator item in UI) {
            item.keepAnimatorStateOnDisable = true;
        }
    }
}
