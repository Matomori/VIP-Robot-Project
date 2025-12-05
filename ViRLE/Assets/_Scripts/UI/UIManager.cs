using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject mainMenu;   // drag in the child object called "main menu"


    private GameObject _currTextboxInstance;
    private Text dialogueText;
    private ActivityData currActivity;

    // TODO: different way of organizing this later     --> maybe i can create a list and iterate through it to add all the events?
    // probably also need a way to store the current activity manager type, rn going to use this serilized ref
    [SerializeField] private ActivityManager robotDogActivityManager;

    void Start() {
        _nextButton.onClick.AddListener(NextButtonClick);
        robotDogActivityManager.OnStartActivity += StartActivity;
    }

    /// <summary>
    /// Call when you start a NEW activity
    /// </summary>
    private void StartActivity(ActivityData activityData) {
        currActivity = activityData;

        if (_currTextboxInstance == null) {
            _currTextboxInstance = Instantiate(currActivity.uiPrefab, this.gameObject.transform);
            dialogueText = _currTextboxInstance.GetComponentInChildren<Text>();
        }

        _nextButton.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(true);

        currActivity.LoadNextContent();
        NextPage();
        MainMenuVisability(false);
    }

    private void NextPage() {
        robotDogActivityManager.CheckConditions();  // TODO: change this to a bool return prolly
        string nextText = currActivity.Next();
        if (nextText != null) { dialogueText.text = nextText; }
    }

    private void PrevPage() {
        string prevText = currActivity.Prev();
        if (prevText != null) { dialogueText.text = prevText; }
    }

    /// <summary>
    /// Assign these to the next and back buttons in the ui prefab (kinda jank ik)
    /// </summary>
    public void NextButtonClick() { NextPage(); }

    public void BackButtonClick() { PrevPage(); }

    /// <summary>
    /// Sets the Main Menu visability (only visuals)
    /// </summary>
    public void MainMenuVisability(bool b) {
        mainMenu.SetActive(b);
    }
}
