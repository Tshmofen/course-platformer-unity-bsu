using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Interface.World;
using UnityEngine;
using Util;

namespace Environment
{
    public class MovableController : MonoBehaviour
    {
        #region Fields and properties

        public InteractButton button;
        
        private List<Movable> _movables;
        private List<DistanceSortable<Movable>> _sortables;
        private Movable _currentMovable;
        private bool _isCurrentMovableSet;

        #endregion

        #region Unity calls

        private void Start()
        {
            _movables = new List<Movable>();
            _sortables = new List<DistanceSortable<Movable>>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tag.Movable)) return;
            var movable = other.GetComponent<Movable>();
            if (movable != null) _movables.Add(movable);
        }

        private void FixedUpdate()
        {
            if (_movables.Count == 0)
            {
                _isCurrentMovableSet = false;
                return;
            }

            _sortables.Clear();
            foreach (var movable in _movables)
            {
                var distance = movable.transform.position - transform.position;
                _sortables.Add(new DistanceSortable<Movable>(movable, distance.magnitude));
            }
            _sortables.Sort();
            _currentMovable = _sortables[0].Item;
            _isCurrentMovableSet = true;
        }

        private void Update()
        {
            if (!_isCurrentMovableSet)
            {
                SwitchButton(false);
                return;
            }

            SwitchButton(true);
            button.transform.position = _currentMovable.transform.position;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(Tag.Movable)) return;
            var movable = other.GetComponent<Movable>();
            if (movable != null) _movables.Remove(movable);
        }

        #endregion

        #region Support Methods

        private void SwitchButton(bool enable)
        {
            button.Sprite.enabled = enable;
        }

        #endregion
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public struct DistanceSortable<T> : IComparable<DistanceSortable<T>>
    {
        public T Item { get; set; }
        public float Distance { get; set; }
 
        public DistanceSortable(T item, float distance)
        {
            Item = item;
            Distance = distance;
        }

        public int CompareTo(DistanceSortable<T> other)
        {
            return Distance.CompareTo(other.Distance);
        }
    }
}