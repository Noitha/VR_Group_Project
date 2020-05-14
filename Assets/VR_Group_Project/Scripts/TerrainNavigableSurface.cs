namespace VR_Group_Project.Scripts
{
    public class TerrainNavigableSurface : BaseNavigableSurface
    {
        public override void Initialize(Level level)
        {
            base.Initialize(level);
            
            //Set the layer mask of the surface to: NavMesh & PlaceableNavigationObjects
            NavMeshSurface.layerMask = 1536+4096;
        }
    }
}