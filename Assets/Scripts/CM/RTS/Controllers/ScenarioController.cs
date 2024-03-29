using CM.Units;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScenarioController : MonoBehaviour
{
    public UnitArchetype archetypeOne;
    public UnitArchetype archetypeTwo;
    
    public Transform spawnPointOne;
    public Transform spawnPointTwo;

    private void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
            SpawnUnit(archetypeOne, spawnPointOne.position);
        
        if (Keyboard.current.f2Key.wasPressedThisFrame)
            SpawnUnit(archetypeTwo, spawnPointTwo.position);
    }
    
    private void SpawnUnit(UnitArchetype archetype, Vector3 position)
    {
        UnitFactory.CreateUnit(archetype, position);
    }
}