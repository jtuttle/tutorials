using UnityEngine;

public class SceneSwitcher : MonoBehaviour {
    public void SwitchScene() {
        // TODO: these Application calls are deprecated, work in replacements?
        int nextLevel = (Application.loadedLevel + 1) % Application.levelCount;
        Application.LoadLevel(nextLevel);
    }
}
