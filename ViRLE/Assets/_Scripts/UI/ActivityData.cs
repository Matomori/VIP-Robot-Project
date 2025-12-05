using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CustomUI/Activity Data")]
public class ActivityData : ScriptableObject
{
    public GameObject uiPrefab;
    public List<ContentContainer> contentContainers;
    public int ContentIndex { get; private set; }

    private bool hasStarted = false;    // might be jank, not sure yet

    public void LoadNextContent() {
        ContentIndex = 0;       // TODO: check. i think this method is only for loading a new activity
                                // so maybe this is ok? (check)

        contentContainers[ContentIndex].ResetIndex();
        //if (!hasStarted) {
        //    Debug.Log("got here");
        //    hasStarted = true;
        //    ContentIndex = 0;
        //} else { 
        //    // TODO: add check here
        //    ContentIndex++; 
        //}


        contentContainers[ContentIndex].LoadDialogueFromFile();
    }

    public string Next() {
        return contentContainers[ContentIndex].Next();
    }

    public string Prev() {
        return contentContainers[ContentIndex].Prev();
    }

    public int GetCurrDialogueIndex() { return contentContainers[ContentIndex].Index; }
}
