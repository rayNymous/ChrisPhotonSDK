using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class CharacterSelector : MonoBehaviour
{
    public GameObject Deployed;
    public UILabel Username;
    public UILabel CharacterIndex;
    public UIButton PrevButton;
    public UIButton NextButton;
    public UILabel Breed;
    public GameObject Character;

    private int _currentCharacter = 0;
    private int _charCount = 0;

    public int LastCharacterIndex { get { return _charCount; } }
    public int CurrentCharacterIndex { get { return _currentCharacter + 1; } }

    private CharacterListItem[] _characters;
    private CharacterSelectGui _gui;

    void Start()
    {
        _gui = FindObjectOfType<CharacterSelectGui>();
    }

    public void Initialize(CharacterListItem[] characters)
    {
        _charCount = characters == null ? 0 : characters.Length;
        _characters = characters;
        _currentCharacter = 0;
        UpdateView();
    } 

    public void Next()
    {
        if (HasNextCharacter())
        {
            _currentCharacter++;
            UpdateView();
        }
    }

    public void Prev()
    {
        if (HasPrevCharacter())
        {
            _currentCharacter--;
            UpdateView();
        }
        
    }

    private bool HasNextCharacter()
    {
        return _currentCharacter < LastCharacterIndex - 1;
    }

    private bool HasPrevCharacter()
    {
        return _currentCharacter > 0;
    }

    private void UpdateView()
    {
        NGUITools.SetActive(Breed.gameObject, _charCount == 0);
        NGUITools.SetActive(Character, _charCount != 0);

        if (_charCount > 0)
        {
            Username.text = _characters[_currentCharacter].Name;
            NGUITools.SetActive(Deployed.gameObject, _characters[_currentCharacter].Deployed);
        }
        else
        {
            NGUITools.SetActive(Deployed.gameObject, false);
            NGUITools.SetActive(Username.gameObject.transform.parent.gameObject, false);
        }

        CharacterIndex.text = CurrentCharacterIndex + "/" + LastCharacterIndex;
        NGUITools.SetActive(CharacterIndex.gameObject.transform.parent.gameObject, LastCharacterIndex > 1);

        NextButton.isEnabled = HasNextCharacter();
        PrevButton.isEnabled = HasPrevCharacter();
    }

    public void CreateNew()
    {
        
    }
}
