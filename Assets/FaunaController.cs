using System.Collections;
using UnityEngine;

public class FaunaController : MonoBehaviour
{
    public GameObject boar;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(10f);
        
        var boarInstance = Instantiate(boar, transform.position, Quaternion.identity);
        InteractionController.SetUpInteractableTriggerCollider(boarInstance);
    }
}