using CM;
using CM.Units;
using UnityEngine;

public class Equipment : EntityBehavior, IInteractable
{
    public Attachment.Enum_AttachmentType attachmentType;
    public Attachment attachedTo;

    private bool _isEquipped;
    public bool IsEquipped => _isEquipped;
    public static LayerMask Mask => LayerMask.GetMask("Item");

    public bool TryAttach(Attachment attachedTo)
    {
        if (IsEquipped)
            return false;
        
        this.attachedTo = attachedTo;
        _isEquipped = true;

        transform.SetParent(attachedTo.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        var rb = transform.GetComponent<Rigidbody>();
        rb.isKinematic = true;

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

    public void TryDetach()
    {
        attachedTo._unitController.Attack -= Use;
        attachedTo = null;
        
        _isEquipped = false;
        
        transform.SetParent(null);
        var rb = transform.GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
}