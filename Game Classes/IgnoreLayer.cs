using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze {
    /// <summary>
    /// This is a layer for ignoring collisions, you can add Types or specific gameObjects to a layer.
    /// Adding a layer to a gameObject make that gameObject unable to collide with everything that is inside the layer
    /// </summary>
    public class IgnoreLayer{
        /// <summary>
        /// The list of gameObjects that should be ignored while checking collisions
        /// </summary>
        public List<GameObject> gameObjects;

        /// <summary>
        /// The list of Types of gameObjects that should be ignored while checking collisions
        /// </summary>
        public List<Type> types;

        /// <summary>
        /// The constructor simply initialize the lists
        /// </summary>
        public IgnoreLayer(){
            gameObjects = new List<GameObject>();
            types = new List<Type>();
        }
    }
}
