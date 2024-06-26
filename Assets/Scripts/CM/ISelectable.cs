﻿using UnityEngine;

namespace CM.Units
{
    public interface ISelectable
    {
        public Observable<bool> IsSelected { get; set; }

        public void SetSelected(bool isSelected);
        
        public Rigidbody Rigidbody { get; internal set; }
    }
}