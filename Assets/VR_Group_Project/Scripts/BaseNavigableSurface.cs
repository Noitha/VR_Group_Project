using UnityEngine;
using UnityEngine.AI;

namespace VR_Group_Project.Scripts
{
    [RequireComponent(typeof(NavMeshSurface))]
    public abstract class BaseNavigableSurface : BaseLevelObject
    {
        protected NavMeshSurface NavMeshSurface;
        
        public override void Initialize(Level level)
        {
            base.Initialize(level);
            
            NavMeshSurface = GetComponent<NavMeshSurface>();
            NavMeshSurface.voxelSize = 0.01f;
            NavMeshSurface.tileSize = 16;
            NavMeshSurface.overrideVoxelSize = true;
            NavMeshSurface.overrideTileSize = true;
            NavMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            NavMeshSurface.collectObjects = CollectObjects.Children;
            
            RebuildSurface();
        }

        public void RebuildSurface()
        {
            NavMeshSurface.RemoveData();
            NavMeshSurface.BuildNavMesh();
        }
    }
}