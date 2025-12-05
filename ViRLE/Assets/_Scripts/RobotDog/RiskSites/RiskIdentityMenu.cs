using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI;


public class RiskIdentityMenu : MonoBehaviour

{

[SerializeField] private List<Button> buttonList;


// Creates menu and hooks up button events

public void CreateMenu(List<string> options, int correctAnsIndex)

{

for (int i = 0; i < buttonList.Count; ++i)

{

// Update the text label of the button

Text buttonLabel = buttonList[i].GetComponentInChildren<Text>();

if (buttonLabel != null)

buttonLabel.text = options[i];


// Clear old listeners first (important if menu is reused)

buttonList[i].onClick.RemoveAllListeners();


// Add correct or wrong answer callback

if (i == correctAnsIndex)

buttonList[i].onClick.AddListener(CorrectAnswer);

else

buttonList[i].onClick.AddListener(WrongAnswer);

}

}


private void WrongAnswer()

{

Debug.Log("Wrong");

}


private void CorrectAnswer()

{

Debug.Log("Correct");

}

}