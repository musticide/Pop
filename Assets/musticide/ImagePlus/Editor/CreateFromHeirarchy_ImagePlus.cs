using UnityEngine;
using UnityEditor;

namespace Musticide.UI
{
    public class CreateFromHeirarchy_ImagePlus
    {
        [MenuItem("GameObject/UI/ImagePlus", false, 10)]
        public static void CreateImagePlus(MenuCommand command)
        {
            GameObject imagePlus = new GameObject("ImagePlus", typeof(ImagePlus));

            GameObject parent = command.context as GameObject;

            if (parent != null)
                GameObjectUtility.SetParentAndAlign(imagePlus, parent);
            else
                Debug.LogWarning("No parent found for ImagePlus. It will be created at the root of the scene.");

            Undo.RegisterCreatedObjectUndo((Object)imagePlus, "Create ImagePlus");

            Selection.activeGameObject = imagePlus;
        }
    }

}


