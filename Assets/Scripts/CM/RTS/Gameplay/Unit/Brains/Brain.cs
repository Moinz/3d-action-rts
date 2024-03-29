﻿using System;
using UnityEngine;

namespace CM.Units
{
    [Serializable]
    public abstract class Brain
    {
        public abstract int TickRate { get; }
        
        public UnitStateController _stateController;
        public UnitController _unitController;
        
        public Inventory inventory => _unitController._inventory;
        
        public GameObject Target
        {
            get => _stateController.target;
            set => _stateController.target = value;
        }

        public LayerMask searchMask;

        public abstract void Tick();
        
        public abstract void Initialize(UnitStateController stateController, UnitController unitController);
    }
}