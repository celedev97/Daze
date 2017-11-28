using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Daze {
    public static partial class Engine {

        //Preloading lists
        internal static Dictionary<string, Sprite> sprites;
        internal static Dictionary<string, string> wavPaths;

        //sounds list
        internal static List<Wav> playingWavs;


        #region Methods for preloading
        #region Sprite
        /// <summary>
        /// This method load a Sprite from a resource.
        /// After that you loaded a sprite in this way the next load for the same sprite will not happen and you will just get a reference to the Sprite, so feel free to call it for every GameObject you need, you won't slow down the game due to I/O operations.
        /// </summary>
        /// <param name="resource_Name">The name of the resource</param>
        /// <param name="scale">the scale of the sprite</param>
        /// <param name="rotation">the rotation of this sprite</param>
        /// <returns></returns>
        public static Sprite loadSprite(string resource_Name, float scale = 1, float rotation = 0) {
            string spriteNameWithoutRotation = resource_Name + "_dazeX" + scale;
            //searching if the sprite was already loaded
            Sprite sameSprite = null;
            int steppedRotation = -1;
            foreach(KeyValuePair<string, Sprite> keyVal in sprites) {
                if(keyVal.Key.Contains(spriteNameWithoutRotation)) {
                    //This sprite is the same that i need, but i don't know if it has the same rotation or not
                    sameSprite = keyVal.Value;
                    if(steppedRotation == -1) {
                        //i save the value of the stepped rotation
                        //every sprite with the same base will return the same step rotation, so why calculate it more than once?
                        steppedRotation = keyVal.Value.stepRotation(rotation);
                    }

                    if(steppedRotation == keyVal.Value.rotation) {
                        //questo era esattamente lo sprite che cercavo
                        return keyVal.Value;
                    }
                }
            }
            //i check if i found the same sprite with another rotation
            if(sameSprite != null) {
                Sprite rotatedSprite = sameSprite.cloneBase(steppedRotation);
                sprites.Add(spriteNameWithoutRotation + "_dazeR" + steppedRotation, rotatedSprite);
                return rotatedSprite;
            }
            //if i'm here then it was never loaded, so i have to load it
            //getting namespace from caller method
            Assembly callerAssembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;
            Bitmap bitmap = null;
            //getting all the resource files in the namespace
            foreach(string rsxName in callerAssembly.GetManifestResourceNames()) {
                try {
                    ResourceManager rm = new ResourceManager(rsxName.Replace(".Resources.resources", ".Resources"), callerAssembly);
                    bitmap = (Bitmap)rm.GetObject(resource_Name);
                    break;
                } catch { }
            }
            if(bitmap == null) throw new Exception("Can't find the sprite " + resource_Name + ": the name must be the same as the Resource's name");
            Sprite newSprite = new Sprite(bitmap, resource_Name, scale, rotation);
            sprites.Add(spriteNameWithoutRotation + "_dazeR" + newSprite.rotation, newSprite);
            return newSprite;
        }
        #endregion

        #region Wav
        /// <summary>
        /// This method load a Wav from a resource embedded in your project
        /// </summary>
        /// <param name="resource_Name">The name of the resource representing the wav</param>
        /// <param name="volume">The volume of the Wav (from 0 to 100)</param>
        /// <param name="loop">True if you want the sound to loop till you pause or stop it</param>
        /// <param name="callerAssembly">The assembly in wich the resources are placed, if you don't set this it will be the assembly that call this function</param>
        /// <returns>The Wav loaded</returns>
        public static Wav loadWavFromResources(string resource_Name, int volume = 100, bool loop = false, Assembly callerAssembly = null) {
            //searchig if the Wav was already extracted
            foreach(KeyValuePair<string, string> keyVal in wavPaths) {
                if(keyVal.Key == resource_Name) {
                    return new Wav(keyVal.Value, volume, loop);
                }
            }
            //if I'm here then the Wav was never extracted
            //getting the nameSpace of the method that called this method
            if(callerAssembly == null) callerAssembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;
            //getting all the resource files in the namespace
            string tempFile = null;
            foreach(string rsxName in callerAssembly.GetManifestResourceNames()) {
                try {
                    ResourceManager rm = new ResourceManager(rsxName.Replace(".Resources.resources", ".Resources"), callerAssembly);
                    tempFile = writeStreamToTempLocation(rm.GetStream(resource_Name), ".wav");
                    break;
                } catch(Exception ex) {
                    Console.WriteLine(ex);
                }
            }
            if(tempFile == null) throw new Exception("Can't find the wav " + resource_Name + ": the name must be the same as the Resource's name");
            return new Wav(tempFile, volume, loop);
        }
        /// <summary>
        /// This method load a wav from a file
        /// </summary>
        /// <param name="filePath">The path of the file to load</param>
        /// <param name="volume">The volume of the Wav (from 0 to 100)</param>
        /// <param name="loop">True if you want this Wav to restart at the end of the file</param>
        /// <returns></returns>
        public static Wav loadWavFromFile(string filePath, int volume = 100, bool loop = false) {
            return new Wav(filePath, volume, loop);
        }
        #endregion

        #region Fonts

        #endregion

        private static string writeStreamToTempLocation(UnmanagedMemoryStream inputStream, string extension) {
            string filePath = String.Format(System.IO.Path.GetTempPath() + Guid.NewGuid().ToString("N") + extension);
            using(FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                int readByte;
                byte[] copyBuffer = new byte[524288]; //0,5MB every cycle (512*1024 byte)
                while((readByte = inputStream.Read(copyBuffer, 0, copyBuffer.Length)) > 0) {
                    outputStream.Write(copyBuffer, 0, readByte);
                }
                outputStream.Flush();
            }
            inputStream.Dispose();
            return filePath;
        }

        #endregion

        #region Method for playing sounds for people so lazy that they don't want to create a variable for the Wav
        /// <summary>
        /// This method play a Wav from the resources.
        /// Using this method you don't need to manage the Wav initialization and disposition when the sound ends
        /// </summary>
        /// <param name="resource_Name"></param>
        /// <param name="volume">The volume of the Wav (from 0 to 100)</param>
        public static void playWavFromResources(string resource_Name, int volume = 100) {
            Wav newWav = loadWavFromResources(resource_Name, volume, false, new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly);
            newWav.disposeAtEnd = true;
            playingWavs.Add(newWav);
            newWav.Play();
        }
        /// <summary>
        /// This method play a Wav from a file.
        /// Using this method you don't need to manage the Wav initialization and disposition when the sound ends
        /// </summary>
        /// <param name="filePath">The path of the Wav file</param>
        public static void playWavFromFile(string filePath) {
            Wav newWav = loadWavFromFile(filePath);
            newWav.disposeAtEnd = true;
            playingWavs.Add(newWav);
            newWav.Play();
        }
        #endregion

    }
}
