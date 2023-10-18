using CM.Units;
using UnityEngine;
using Random = UnityEngine.Random;

public class Resource : MonoBehaviour, IInteractable
{
    public ResourceSO SO;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 position)
    {
        var throwVelocity = CalculateThrowVelocityFixedApex(transform.position, position, .5f);

        _rigidbody.velocity = throwVelocity;
    }

    public void ThrowRandom(Vector3 center, float radius)
    {
        var point = Random.insideUnitCircle * radius;
        point = center + new Vector3(point.x, 0f, point.y);
        
        Throw(point);
    }

    private Vector3 CalculateThrowVelocityFixedApex(Vector3 fromPosition, Vector3 toPosition, float apex)
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

    public void Interact(UnitController unitController)
    {
        unitController._inventory.TryAddResource(this);
    }
}