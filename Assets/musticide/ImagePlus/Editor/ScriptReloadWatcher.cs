using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Musticide
{
    public class ScriptReloadWatcher
    {
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var all = Object.FindObjectsByType<Musticide.UI.ImagePlus>(FindObjectsSortMode.None);
            foreach (var instance in all)
            {
                instance.UpdateParams();
            }
        }
    }
}

