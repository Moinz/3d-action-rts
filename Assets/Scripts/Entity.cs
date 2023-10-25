using UnityEngine;

namespace CM.Units
{
    public abstract class Entity : MonoBehaviour, ISelectable
    {
        private Rigidbody _rigidbody;
        public Observable<bool> IsSelected { get; set; }

        private void Awake()
        {
            IsSelected = new Observable<bool>(false);
        }

        Rigidbody ISelectable.Rigidbody
        {
            get => _rigidbody;
            set => _rigidbody = value;
        }
    }
}