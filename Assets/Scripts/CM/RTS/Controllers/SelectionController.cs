using System;
using System.Collections.Generic;
using System.Linq;
using Shapes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CM.Units
{
    [Serializable]
    public class SelectionController : ImmediateModeShapeDrawer, PlayerControls.IInteractionActions
    {
        private int _hits;
        private RaycastHit[] _rayCastHits = new RaycastHit[10];

        public LayerMask searchMask;

        private ISelectable _selected;
        public ISelectable Selected => _selected;
        public bool HasSelection => _selected != null;

        private PlayerControls _playerControls;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _playerControls = new PlayerControls();
            _playerControls.Enable();
            
            _playerControls.Interaction.SetCallbacks(this);
            _playerControls.Interaction.Enable();
        }
        
        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            Select();
        }
        
        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            Deselect();
        }

        public void Select()
        {
            _hits = CollectHits();

            var hasCandidate = GetBestCandidate(out var bestCandidate);
            if (!hasCandidate)
                return;

            // Deselect previous
            Deselect();

            // Select new
            _selected = bestCandidate;
            _selected.SetSelected(true);
            _selected.IsSelected.OnValueChanged += OnSelectedChanged;
        }

        private void Deselect()
        {
            if (_selected == null)
                return;

            _selected.SetSelected(false);
        }

        private void OnSelectedChanged(bool newValue)
        {
            if (newValue)
                return;

            _selected.IsSelected.OnValueChanged -= OnSelectedChanged;
            _selected = null;
        }

        private int CollectHits()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(mousePos);

            var hits = Physics.RaycastNonAlloc(ray, _rayCastHits, 1000f, searchMask, QueryTriggerInteraction.Collide);

            return hits;
        }

        private bool GetBestCandidate(out ISelectable bestCandidate)
        {
            bestCandidate = null;
            var selectables = GetSelectables(_hits, _rayCastHits)
                .OrderBy(x => Vector3.Distance(x.Rigidbody.position, _rayCastHits[0].point))
                .ToArray();

            var hasSelectable = selectables.Length > 0;

            if (!hasSelectable)
                return false;

            bestCandidate = selectables[0];
            return true;
        }

        private List<ISelectable> GetSelectables(int num, IReadOnlyList<RaycastHit> hits)
        {
            var selectables = new List<ISelectable>();
            for (int i = 0; i < num; i++)
            {
                var rb = hits[i].rigidbody;

                if (!rb)
                    continue;

                rb.TryGetComponent(out ISelectable iSelectable);

                if (iSelectable == null)
                    continue;

                iSelectable.Rigidbody = rb;
                selectables.Add(iSelectable);
            }

            return selectables;
        }

        public override void DrawShapes(Camera cam)
        {

            if (!HasSelection)
                return;

            using (Draw.Command(cam))
            {
                // set up static parameters. these are used for all following Draw.Line calls
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Pixels;
                Draw.Thickness = 4; // 4px wide

                var rb = Selected.Rigidbody;
                var radius = rb.GetComponentInChildren<SphereCollider>(true).radius;
                
                Draw.Matrix = Matrix4x4.TRS(new Vector3(rb.position.x, 0f, rb.position.z), Quaternion.identity, Vector3.one * radius);
                
                // draw lines
                Draw.Disc(Vector3.zero, Vector3.up, .5f, Color.green);
            }
        }
    }
}