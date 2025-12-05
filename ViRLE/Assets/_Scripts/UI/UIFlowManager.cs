using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIFlowManager : MonoBehaviour
{
    [Header("UI Elements")]
    // 1. Drag your Parent 'borderPanel' here
    [SerializeField] private GameObject borderPanel;
    // 2. Drag your Child 'MenuPanel' here
    [SerializeField] private GameObject menuPanel;

    [Header("Sequences (in order)")]
    [SerializeField] private List<Sequence> sequences = new();

    private int activeSeq = -1;
    private int activeSlide = -1;

    [System.Serializable]
    public class Sequence
    {
        public string id;                    // e.g., "fieldprinter"
        public List<GameObject> slides;      // ordered slide panels
        public UnityEvent onSequenceStart;
        public UnityEvent onSequenceEnd;
    }

    void Start()
    {
        BackToMenu();
    }

    // ===== Public API =====
    public void OpenFieldPrinterIntro() => OpenSequenceById("fieldprinter");
    public void OpenRobotProperties() => OpenSequenceById("robotprops");
    public void OpenExampleApplication() => OpenSequenceById("example");

    public void Next()
    {
        if (activeSeq < 0) return;
        activeSlide++;
        var seq = sequences[activeSeq];

        // End of sequence -> Back to menu
        if (activeSlide >= seq.slides.Count)
        {
            seq.onSequenceEnd?.Invoke();
            BackToMenu();
            return;
        }

        ShowOnly(seq.slides[activeSlide]);
    }

    public void BackToMenu()
    {
        // Hide all slides
        foreach (var s in sequences)
            foreach (var slide in s.slides)
                if (slide) slide.SetActive(false);

        activeSeq = -1;
        activeSlide = -1;

        // 3. Turn the BORDER (and Menu) back ON
        if (borderPanel) borderPanel.SetActive(true);
        if (menuPanel) menuPanel.SetActive(true);
    }

    // ===== Internals =====
    private void OpenSequenceById(string id)
    {
        int idx = sequences.FindIndex(s => s.id == id);
        if (idx < 0) { Debug.LogWarning($"Sequence '{id}' not found"); return; }

        activeSeq = idx;
        activeSlide = 0;

        // 4. Turn the BORDER OFF (Hides buttons automatically since they are children)
        if (borderPanel) borderPanel.SetActive(false);

        sequences[idx].onSequenceStart?.Invoke();
        ShowOnly(sequences[idx].slides[activeSlide]);
    }

    private void ShowOnly(GameObject target)
    {
        if (!target) return;

        // We don't need to hide menuPanel explicitly if borderPanel is hidden, 
        // but we do it anyway to be safe.
        if (menuPanel) menuPanel.SetActive(false);

        foreach (var s in sequences)
            foreach (var slide in s.slides)
                if (slide) slide.SetActive(false);

        target.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) BackToMenu();
    }
}