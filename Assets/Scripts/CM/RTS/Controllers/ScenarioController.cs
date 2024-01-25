using CM.Units;
using UnityEngine;
public class ScenarioController : MonoBehaviour
{
    public UnitArchetype archetypeOne;
    public UnitArchetype archetypeTwo;
    
    private void Start()
    {   
        var spawnPosition = transform.position;
        spawnPosition.y += 0.5f;
        
        // Factorize this
        var instanceOne = UnitFactory.CreateUnit(archetypeOne, spawnPosition);
        
        spawnPosition.x -= 1f;

        var instanceTwo = UnitFactory.CreateUnit(archetypeTwo, spawnPosition);
        
        // Set up targets. Keep it simple
        instanceOne.target = instanceTwo.gameObject;
        instanceTwo.target = instanceOne.gameObject;
    }
}