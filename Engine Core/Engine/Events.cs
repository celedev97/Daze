using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Daze {
    public static partial class Engine {

        #region Events
        /// <summary>
        /// This is an event called from the IDrawable used to draw, use GameObject events if you need to detect a mouse event.
        /// </summary>
        public static MouseEventHandler mouseClick, mouseDoubleClick, mouseMove, mouseDown, mouseUp;

        private static List<MouseEvent> mouseClicksList, mouseDoubleClicksList, mouseMovesList, mouseDownsList, mouseUpsList;

        #region Buffering degli eventi per lo svolgimento al momento giusto
        private static void mouseClickedAdd(object sender, MouseEventArgs e) { mouseClicksList.Add(new MouseEvent(sender, e)); }
        private static void mouseDoubleClickedAdd(object sender, MouseEventArgs e) { mouseDoubleClicksList.Add(new MouseEvent(sender, e)); }
        private static void mouseMovedAdd(object sender, MouseEventArgs e) { mouseMovesList.Add(new MouseEvent(sender, e)); }
        private static void mouseButtonDownAdd(object sender, MouseEventArgs e) { mouseDownsList.Add(new MouseEvent(sender, e)); }
        private static void mouseButtonUpAdd(object sender, MouseEventArgs e) { mouseUpsList.Add(new MouseEvent(sender, e)); }
        #endregion

        #endregion


        #region Esecuzione effettiva degli eventi
        private static void mouseClickExecute(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseClick?.Invoke(gameObject, e);
                    }
                }
            }
        }

        private static void mouseDoubleClickExecute(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseDoubleClick?.Invoke(gameObject, e);
                    }
                }
            }
        }

        private static void mouseMoveExecute(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseMove?.Invoke(gameObject, e);
                    }
                }
            }
        }

        private static void mouseDownExecute(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseDown?.Invoke(gameObject, e);
                    }
                }
            }
        }

        private static void mouseUpExecute(object sender, MouseEventArgs e) {
            foreach(GameObject gameObject in gameObjects) {
                if(toDeleteGameObjects.Contains(gameObject)) continue;
                if(gameObject.spriteSet != null && gameObject.spriteSet.sprite != null) {
                    if(Geometry.Utility.between(e.X, gameObject.pixelPosition.x, gameObject.pixelPosition.x + gameObject.spriteSet.sprite.width) &&
                       Geometry.Utility.between(e.Y, gameObject.pixelPosition.y, gameObject.pixelPosition.y + gameObject.spriteSet.sprite.height)) {
                        gameObject.mouseUp?.Invoke(gameObject, e);
                    }
                }
            }
        }
        #endregion
    }
}
