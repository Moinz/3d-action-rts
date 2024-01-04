using System.Collections;
using CM.Units;
using UnityEngine;

public class FaunaController : MonoBehaviour
{
    public UnitStateController boar;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(10f);
        
        var boarInstance = Instantiate(boar, transform.position, Quaternion.identity);
        boarInstance.Initialize(new BoarBrain());
        
        InteractionController.SetUpInteractableTriggerCollider(boarInstance.gameObject);
    }
}