using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "CustomUI/Content Container")]
public class ContentContainer : ScriptableObject
{
    public TextAsset dialogueFile;  // .txt file
    public List<string> dialogueText = new List<string>();
    public Image image;
    public VideoClip video;

    // might add a string ID here which tells what activity to link to another?

    public int Index { get; private set; }

    public void ResetIndex() {  Index = 0; }

    /// <summary>
    /// Converts the lines in the .txt file to a list of string lines
    /// </summary>
    public void LoadDialogueFromFile() {
        dialogueText.Clear();

        if (dialogueFile != null) {
            string[] lines = dialogueFile.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            dialogueText.AddRange(lines);
        } else {
            Debug.Log("No dialogue .txt file in content scriptable object");
        }
    }

    public string Next() {
        if (Index >= dialogueText.Count) { return null; }
        return dialogueText[Index++];
    }

    public string Prev() {
        if (Index - 1 < 0) { return null; }
        return dialogueText[--Index];
    }
}
