using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "CM/Data/Resource", fileName = "Resource Data", order = 0)]
public class ResourceSO : ScriptableObject
{
    [FormerlySerializedAs("_resource")] public Resource resource;
}