using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreationUI : MonoBehaviour
{

    private static CharacterCreationUI instance;

    public Animator animator;

    public Button doneButton;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }

    public Button[] genderButtons;
    public OptionPanelCharacterCreateUI[] panels;

    public void SelectGender(string gender)
    {
        foreach (Button b in genderButtons)
        {
            if (b.name.Equals(gender))
            {
                b.interactable = false;
            } else
            {
                b.interactable = true;
            }
        }

        pronouns = gender;
        CheckButton();
    }

    private string pronouns;
    private string playerName;

    public void Finish()
    {
        ClothingController.UpdateColors();

        SceneManager.LoadSceneAsync("Game");
    }

    public void SelectName(string name)
    {
        playerName = name;
        CheckButton();
    }

    public void CheckButton()
    {
        doneButton.interactable = !string.IsNullOrWhiteSpace(pronouns) && !string.IsNullOrWhiteSpace(playerName);
    }

    public void Turn(bool left)
    {
        float x = animator.GetFloat("AnimMoveX");
        float y = animator.GetFloat("AnimMoveY");

        // 0,0 -> front
        // 1,0 -> left
        // -1,0 -> right
        // x,1 -> back

        if (!left)
        {
            if (x == 0 && y == 0) { x = 1; y = 0; }
            else if (x == 1 && y == 0) { y = 1; }
            else if (x == -1 && y == 0) { x = 0; y = 0; }
            else { x = -1; y = 0; }
        } else
        {
            if (x == 0 && y == 0) { x = -1; y = 0; }
            else if (x == 1 && y == 0) { x = 0; y = 0; }
            else if (x == -1 && y == 0) { y = 1; }
            else { x = 1; y = 0; }
        }

        animator.SetFloat("AnimMoveX", x);
        animator.SetFloat("AnimMoveY", y);
    }
}
