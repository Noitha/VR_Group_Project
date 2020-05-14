using UnityEngine;

namespace VR_Group_Project.Scripts
{
    public class PlaceableNavigation : BaseGrabbableObject
    {
        private Vector3 lastBuildPosition;

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            
            transform.SetParent(Level.placeableNavigationObjectsContainer);
            lastBuildPosition = transform.position;
            InvokeRepeating(nameof(CheckPositionChange), 0, GameController.CheckPositionChangeRepeatTime);
        }

        /// <summary>
        /// Check if the position of the object has changed lately. If so update the navigation mesh path.
        /// </summary>
        private void CheckPositionChange()
        {
            var distance = Vector3.Distance(transform.position, lastBuildPosition);

            if (Player.Instance.HasObjectInHand() || !(distance > GameController.RecalculatePathDistanceThreshold) ||
                !(Mathf.Abs(objectRigidbody.velocity.y) < .1f))
            {
                return;
            }
            
            lastBuildPosition = transform.position;
            Level.RebuildSurfaceMesh();
        }

        public override void Grab(Controller c, BaseGrabbableObject baseGrabbableObject)
        {
            base.Grab(c, baseGrabbableObject);
            gameObject.layer = 9;
        }

        public override void UnGrab()
        {
            base.UnGrab();
            gameObject.layer = 10;
        }
    }
}