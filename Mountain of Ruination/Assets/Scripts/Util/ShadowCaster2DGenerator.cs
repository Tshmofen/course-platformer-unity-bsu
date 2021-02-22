using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

#if UNITY_EDITOR
namespace Util
{
    /// <summary>
    /// It extends the ShadowCaster2D class in order to be able to modify some private data members.
    /// </summary>
    public static class ShadowCaster2DExtensions
    {
        /// <summary>
        /// Replaces the path that defines the shape of the shadow caster.
        /// </summary>
        /// <remarks>
        /// Calling this method will change the shape but not the mesh of the shadow caster. Call SetPathHash afterwards.
        /// </remarks>
        /// <param name="shadowCaster">The object to modify.</param>
        /// <param name="path">The new path to define the shape of the shadow caster.</param>
        public static void SetPath(this ShadowCaster2D shadowCaster, Vector3[] path)
        {
            var shapeField = typeof(ShadowCaster2D).GetField("m_ShapePath",
                                                                   BindingFlags.NonPublic |
                                                                   BindingFlags.Instance);
            shapeField?.SetValue(shadowCaster, path);
        }
     
        /// <summary>
        /// Replaces the hash key of the shadow caster, which produces an internal data rebuild.
        /// </summary>
        /// <remarks>
        /// A change in the shape of the shadow caster will not block the light, it has to be rebuilt using this function.
        /// </remarks>
        /// <param name="shadowCaster">The object to modify.</param>
        /// <param name="hash">The new hash key to store. It must be different from the previous key to produce the rebuild. You can use a random number.</param>
        public static void SetPathHash(this ShadowCaster2D shadowCaster, int hash)
        {
            var hashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash",
                                                                  BindingFlags.NonPublic |
                                                                  BindingFlags.Instance);
            hashField?.SetValue(shadowCaster, hash);
        }
    }
     
    /// <summary>
    /// It provides a way to automatically generate shadow casters that cover the shapes of composite colliders.
    /// </summary>
    /// <remarks>
    /// Specially recommended for tilemaps, as there is no built-in tool that does this job at the moment.
    /// </remarks>
    public static class ShadowCaster2DGenerator
    {
        [UnityEditor.MenuItem("Generate Shadow Casters", menuItem = "Tools/Generate Shadow Casters")]
        public static void GenerateShadowCasters()
        {
            var colliders = Object.FindObjectsOfType<CompositeCollider2D>();

            foreach (var t in colliders)
            {
                GenerateTilemapShadowCasters(t);
            }
        }
     
        /// <summary>
        /// Given a Composite Collider 2D, it replaces existing Shadow Caster 2Ds (children) with new Shadow Caster 2D objects whose
        /// shapes coincide with the paths of the collider.
        /// </summary>
        /// <remarks>
        /// It is recommended that the object that contains the collider component has a Composite Shadow Caster 2D too.
        /// It is recommended to call this method in editor only.
        /// </remarks>
        /// <param name="collider">The collider which will be the parent of the new shadow casters.</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void GenerateTilemapShadowCasters(CompositeCollider2D collider)
        {
            // First, it destroys the existing shadow casters
            var existingShadowCasters = collider.GetComponentsInChildren<ShadowCaster2D>();
     
            foreach (var t in existingShadowCasters)
            {
                Object.DestroyImmediate(t.gameObject);
            }
     
            // Then it creates the new shadow casters, based on the paths of the composite collider
            var pathCount = collider.pathCount;
            var pointsInPath = new List<Vector2>();
            var pointsInPath3D = new List<Vector3>();
     
            for (var i = 0; i < pathCount; ++i)
            {
                collider.GetPath(i, pointsInPath);

                var newShadowCaster = new GameObject("ShadowCaster2D") {isStatic = true};
                newShadowCaster.transform.SetParent(collider.transform, false);
     
                foreach (var t in pointsInPath)
                {
                    pointsInPath3D.Add(t);
                }
     
                var component = newShadowCaster.AddComponent<ShadowCaster2D>();
                component.SetPath(pointsInPath3D.ToArray());
                component.SetPathHash(Random.Range(int.MinValue, int.MaxValue)); // The hashing function GetShapePathHash could be copied from the LightUtility class
     
                pointsInPath.Clear();
                pointsInPath3D.Clear();
            }
        }
     
    }
}
#endif