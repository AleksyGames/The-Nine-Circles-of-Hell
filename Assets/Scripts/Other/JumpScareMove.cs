using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScareMove : MonoBehaviour
{
    [Header("Jump scare settings")]
    public GameObject jumpScareObject;   
    public Transform targetPoint;        
    public float moveSpeed = 5f;         
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
        if (jumpScareObject == null || targetPoint == null)
            yield break;

        jumpScareObject.SetActive(true);

        while (Vector3.Distance(jumpScareObject.transform.position, targetPoint.position) > 0.1f)
        {
            jumpScareObject.transform.position = Vector3.MoveTowards
            (
                jumpScareObject.transform.position,
                targetPoint.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        jumpScareObject.SetActive(false);
    }
}
