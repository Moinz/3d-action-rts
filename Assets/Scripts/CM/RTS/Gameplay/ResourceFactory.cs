using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceFactory : MonoBehaviour
{
    public static Resource CreateResource(ResourceSO resourceSO, Vector3 position)
    {
        var resource = Instantiate(resourceSO.resource, position, Quaternion.identity);
        
        // TODO: Random Array Index extension
        var visualVariation = resourceSO.gfx_resourceVariations[Random.Range(0, resourceSO.gfx_resourceVariations.Length)]; 
        var visual = Instantiate(visualVariation, resource.transform);
        visual.transform.localPosition = Vector3.zero;
        
        InteractionController.SetUpInteractableTriggerCollider(resource);
        
        return resource;
    }

    public static ResourceNode CreateResourceNode(ResourceSO resourceSO, Vector3 position, Transform parent)
    {
        var resource = Instantiate(resourceSO.resourceNode, position, Quaternion.identity);
        resource.transform.SetParent(parent);
        
        var randomRotation = Random.Range(0f, 360f);
        resource.transform.RotateAround(position, Vector3.up, randomRotation);

        // TODO: Random Array Index extension
        var visualVariation = resourceSO.gfx_resourceNodeVariations[Random.Range(0, resourceSO.gfx_resourceNodeVariations.Length)]; 
        var visual = Instantiate(visualVariation, resource.transform);
        visual.transform.localPosition = Vector3.zero;
        
        InteractionController.SetUpInteractableTriggerCollider(resource);
        
        return resource;
    }

    public static void Throw(GameObject gameObject, Vector3 position)
    {
        var rb = gameObject.GetComponent<Rigidbody>();

        if (!rb)
            return;
        
        Throw(rb, position);
    }

    public static void Throw(Rigidbody rb, Vector3 position)
    {
        var throwVelocity = CalculateThrowVelocityFixedApex(rb.position, position, .5f);

        rb.velocity = throwVelocity;
    }

    public static void ThrowRandom(GameObject gameObject, Vector3 center, float radius)
    {
        var rb = gameObject.GetComponent<Rigidbody>();

        if (!rb)
            return;
        
        ThrowRandom(rb, center, radius);
    }

    public static void ThrowRandom(Rigidbody rb, Vector3 center, float radius)
    {
        var point = Random.insideUnitCircle * radius;
        point = center + new Vector3(point.x, 0f, point.y);
        
        Throw(rb, point);
    }

    private static Vector3 CalculateThrowVelocityFixedApex(Vector3 fromPosition, Vector3 toPosition, float apex)
    {
        float gravity = Physics.gravity.y; // m/s^2

        var displacementY = toPosition.y - fromPosition.y; // m

        // var apex = Mathf.Abs(displacementY * apexMultiplier); //m

        var apexTime = Mathf.Sqrt(-2 * apex / gravity); // sqrt(m/(m/s^2)) = sqrt(s^2)=s
        //var timeToApex = Mathf.Sqrt(-2 * gravity * apex); // sqrt(m * (m/s^2)) = sqrt(m^2/s^2)= ?? m/s??
        var timeFromApex = Mathf.Sqrt(2 * (displacementY - apex) / gravity); // sqrt(m/(m/s^2)) = sqrt(s^2)=s

        var displacementXZ = new Vector3(toPosition.x - fromPosition.x, 0, toPosition.z - fromPosition.z); //m
        var time = apexTime + timeFromApex; // s

        var velocityY = Vector3.up * apexTime; //s
        var velocityXZ = displacementXZ / time; //m/s

        return velocityXZ + velocityY * (-gravity); //m/s + (s*(m/s^2) = m/s)
    }
}