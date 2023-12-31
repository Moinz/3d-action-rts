﻿using CM.Units;
using UnityEngine;
using UnityEngine.Serialization;

public class Attachment : MonoBehaviour
{
    public enum Enum_AttachmentType
    {
        Shield,
        Weapon,
        Armor,
        Helmet,
        Feet
    }

    public Enum_AttachmentType attachmentType;
    [FormerlySerializedAs("equipment")] public Equipment attachedEquipment;
    
    public UnitController _unitController;
    public bool HasAttachment => attachedEquipment != null;

    private void Awake()
    {
        _unitController = GetComponentInParent<UnitController>();
        
        if (HasAttachment)
            attachedEquipment.TryAttach(this);
    }
    
    public bool TryAttach(Equipment equipment)
    {
        if (equipment.attachmentType != attachmentType)
            return false;

        if (attachedEquipment)
        {
            attachedEquipment.TryDetach();
            attachedEquipment = null;
        }
        
        var successfullAttachment = equipment.TryAttach(this);
        
        if (!successfullAttachment)
        {
            attachedEquipment = null;
            return false;
        }

        attachedEquipment = equipment;
        Debug.Log(_unitController.gameObject.name + $"Attached {equipment}");
        return true;
    }

}