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
}