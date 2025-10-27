using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSacre : MonoBehaviour
{
    [Header("Jump scare settings")]
    public GameObject jumpScareObject; 
    public float displayTime = 2f;     

    private bool hasTriggered = false;

    private void Start()
    {
        if (jumpScareObject != null)
            jumpScareObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(ShowJumpScare());
        }
    }

    private IEnumerator ShowJumpScare()
    {
        if (jumpScareObject != null)
        {
            jumpScareObject.SetActive(true);  
            yield return new WaitForSeconds(displayTime);
            jumpScareObject.SetActive(false); 
        }
    }
}
