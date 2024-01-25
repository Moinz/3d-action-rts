using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private IInteractable[] _interactables;

    private void Awake()
    {
        _interactables = GetComponentsInChildren<IInteractable>();
        
        SetUpInteractableTriggerColliders();
    }

    private void SetUpInteractableTriggerColliders()
    {
        foreach (var interactable in _interactables)
            SetUpInteractableTriggerCollider(interactable);
    }
    
    public static void SetUpInteractableTriggerCollider(GameObject gameObject)
    {
        var interactables = gameObject.GetComponentsInChildren<IInteractable>();
        
        foreach (var interactable in interactables)
            SetUpInteractableTriggerCollider(interactable);
    }
    
    public static void SetUpInteractableTriggerCollider(IInteractable interactable)
    {
        var interactionCollider = new GameObject("Interaction Collider")
        {
            layer = LayerMask.NameToLayer("Interaction")
        };
            
        interactionCollider.transform.SetParent(interactable.gameObject.transform);
        interactionCollider.transform.localPosition = Vector3.zero;
        
        var bounds = EncapsulateBounds(interactable.gameObject);

        var sc = interactionCollider.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = BoundsToRadius(bounds) * 2f;
    }

    public static Bounds EncapsulateBounds(GameObject gameObject)
    {
        var colliders = gameObject.GetComponentsInChildren<Collider>(true);
        var bounds = new Bounds(gameObject.transform.position, Vector3.zero);

        foreach (var col in colliders)
        {
            bounds.Encapsulate(col.bounds);
        }

        return bounds;
    }
    
    public static float BoundsToRadius(Bounds bounds)
    {
        var radius = bounds.extents.x;
        if (bounds.extents.y > radius)
            radius = bounds.extents.y;
        if (bounds.extents.z > radius)
            radius = bounds.extents.z;

        return radius;
    }
}