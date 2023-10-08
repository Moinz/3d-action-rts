using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Attachment.Enum_AttachmentType attachmentType;
    public Attachment attachedTo;
    
    public bool IsEquipped => attachedTo != null;
    
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
        Debug.Log($"Using {gameObject.name}");
    }
}