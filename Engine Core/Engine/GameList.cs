using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daze {
    public static partial class Engine {
        #region Private Game Lists
        //Game Scripts Lists
        private static List<GameScript> gameScripts;

        private static List<GameObject> gameObjects;
        private static List<GameObject> newGameObjects;
        private static List<GameObject> toDeleteGameObjects;
        #endregion
        
        #region Methods for GameObject operations
        #region Methods for getting informations
        /// <summary>
        /// Get all the gameObject currently in the Game
        /// </summary>
        /// <returns>An array that contains all the gameObject that are not deleted</returns>
        public static GameObject[] findGameObjects() {
            GameObject[] output = new GameObject[newGameObjects.Count + gameObjects.Count];
            for(int i = 0; i < gameObjects.Count; i++) {
                output[i] = gameObjects[i];
            }
            for(int i = gameObjects.Count; i < (gameObjects.Count + newGameObjects.Count); i++) {
                output[i] = newGameObjects[i - gameObjects.Count];
            }
            return gameObjects.ToArray();
        }
        /// <summary>
        /// Get all the gameObjects of a given GameObject subClass
        /// </summary>
        /// <typeparam name="GameObjectClass">The type of the gameObjects to find</typeparam>
        public static List<GameObjectClass> findGameObjects<GameObjectClass>() {
            List<GameObjectClass> output = new List<GameObjectClass>();
            foreach(GameObject gameObject in gameObjects) {
                if(typeof(GameObjectClass).IsInstanceOfType(gameObject)) {
                    output.Add((GameObjectClass)Convert.ChangeType(gameObject, typeof(GameObjectClass)));
                }
            }
            foreach(GameObject gameObject in newGameObjects) {
                if(typeof(GameObjectClass).IsInstanceOfType(gameObject)) {
                    output.Add((GameObjectClass)Convert.ChangeType(gameObject, typeof(GameObjectClass)));
                }
            }
            return output;
        }

        /// <summary>
        /// Check if a GameObject exists currently
        /// </summary>
        /// <param name="gameObject">The GameObject to search</param>
        public static bool gameObjectExists(GameObject gameObject) {
            return gameObjects.Contains(gameObject) || newGameObjects.Contains(gameObject);
        }
        /// <summary>
        /// Check if a GameObject is destroyed
        /// </summary>
        /// <param name="gameObject">The GameObject to search</param>
        /// <returns>true if the gameObject doesn't exists</returns>
        public static bool isGameObjectDestroyed(GameObject gameObject) {
            return ((!gameObjects.Contains(gameObject)) && !newGameObjects.Contains(gameObject)) || toDeleteGameObjects.Contains(gameObject);
        }
        #endregion

        #region Methods for modifying the GameObject list
        /// <summary>
        /// NOT RECOMMENDED: This method add forcefully a gameObject in the gameObjects list, when a gameObject is created it is automaticaly added to the list, don't use this method if you are not sure what you are doing
        /// </summary>
        /// <param name="gameObject">The GameObject to add</param>
        public static void AddGameObject(GameObject gameObject) {
            newGameObjects?.Add(gameObject);
        }

        /// <summary>
        /// Delete a gameObject from the gameObject list
        /// </summary>
        /// <param name="gameObject">The gameObject to remove</param>
        public static void DeleteGameObject(GameObject gameObject) {
            toDeleteGameObjects.Add(gameObject);
        }

        #endregion
        #endregion
    }
}
