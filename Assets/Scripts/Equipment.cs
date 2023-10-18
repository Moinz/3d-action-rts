using CM.Units;
using UnityEngine;

public class Equipment : MonoBehaviour, IInteractable
{
    public Attachment.Enum_AttachmentType attachmentType;
    public Attachment attachedTo;
    
    public bool IsEquipped => attachedTo != null;
    public static LayerMask Mask => LayerMask.GetMask("Item");

    public bool TryAttach(Attachment attachedTo)
    {
        if (IsEquipped)
            return false;
        
        this.attachedTo = attachedTo;

        transform.SetParent(attachedTo.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        attachedTo._unitController.Attack += Use;
        return true;
    }

    public virtual void Use(GameObject target)
    {
    }

    public void Interact(UnitController unitController)
    {
        unitController.GetAttachment(attachmentType).TryAttach(this);
    }
}