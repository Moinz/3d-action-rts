using System.Collections;
using System.Collections.Generic;
using CM.Units;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private UnitStateController _playerUnit;
    
    private void Start()
    {
        var player = Instantiate(_playerUnit, transform.position, Quaternion.identity);
        player.Initialize(new PlayerBrain());
    }
}
