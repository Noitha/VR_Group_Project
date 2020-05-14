namespace VR_Group_Project.Scripts
{
    public class TransformableObject : BaseGrabbableObject
    {
        public int hitCount;
        public ToolType requiredToolType;
        public bool requiresHand;
        
        private PlaceableNavigation _resultObject;


        
        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            gameObject.layer = 15;
        }

        public void Setup(PlaceableNavigation placeableNavigation, int hit, ToolType toolType, bool reqHand)
        {
            _resultObject = placeableNavigation;
            hitCount = hit;
            requiredToolType = toolType;
            requiresHand = reqHand;
        }

        public void Hit(Tool tool)
        {
            if (tool.toolType != requiredToolType || requiresHand && !IsEquipped)
            {
                return;
            }

            hitCount--;

            if (hitCount > 0)
            {
                return;
            }

            Replace();
            ForceUnGrab();
            Destroy(gameObject);
        }

        private void Replace()
        {
            var newObject = Instantiate(_resultObject, transform.position, transform.rotation);
            newObject.Initialize(Level);
        }
    }
}