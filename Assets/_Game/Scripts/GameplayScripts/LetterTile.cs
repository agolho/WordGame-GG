using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TileGame
{
public class LetterTile : MonoBehaviour
{
    private TileManager _tileManager;
    private WordFormArea _wordFormingArea;
    
    [Header("Components & References")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI letterText;
    [SerializeField] private Image tileImage;
    
    [Header("Colors")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color lockedColor;
    
    private readonly List<LetterTile> _parentList = new List<LetterTile>();
    private List<LetterTile> _childrenList = new List<LetterTile>();
    
    private bool _isLocked;
    
    private Transform _originalTransform;
    private Transform _originalParent;
    
    #region Initialization

    public void InitializeTile(TileData tileData, WordFormArea wordFormingArea, TileManager tileManager)
    {
        ResetTile();
        SetLetterText(tileData.character);
        SetTilePosition(tileData);
        CacheTransform();
        SetManagers(wordFormingArea, tileManager);
        SlideInAnimation(tileData);
        AddListenerToButton();
    }

    private void SetManagers(WordFormArea wordFormingArea, TileManager tileManager)
    {
        _tileManager = tileManager;
        _wordFormingArea = wordFormingArea;
    }

    private void SetTilePosition(TileData tileData)
    {
        var offsetY = new Vector3(0, 100, 0);
        var positionUpTheScreen = tileData.position + offsetY;
        transform.position = positionUpTheScreen;
    }
    private void CacheTransform()
    {
        _originalTransform = transform;
        _originalParent = transform.parent;
    }

    private void SetLetterText(string tileCharacter)
    {
        letterText.text = tileCharacter;
    }

    private void AddListenerToButton()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    #endregion

    #region Animations

    private void SlideInAnimation(TileData tileData)
    {
       transform.DOMove(tileData.position, .5f).SetEase(Ease.OutQuad).SetDelay(tileData.id * .025f);
    }

    #endregion

    #region Tile Management

    private void OnButtonClick()
    {
        if (_isLocked) return;
        if (!_wordFormingArea.IsAvailable()) return;
        if (_wordFormingArea.GetCurrentLetterCount() > 6) return;
        
        _wordFormingArea.TakeLetterTile(this, _originalParent, _originalTransform);
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(_wordFormingArea.UndoLastMove);
        
        _tileManager.MoveTile(this);
        
        // Unlock children
        foreach (var child in _childrenList)
        {
            child.RemoveParent(this);
        }

        // Clear parent list as it's now independent
        _parentList.Clear(); 
    }
    private void SetLock(bool state, LetterTile parent = null)
    {
        _isLocked = state;
        tileImage.color = state ? lockedColor : normalColor;
        if (parent != null)
        {
            _parentList.Add(parent);
        }
    }
    
    #endregion

    #region Public Methods

    public string GetLetter()
    {
        return letterText.text;
    }
    
    public LetterTile[] GetChildren()
    {
        return _childrenList.ToArray();
    }
    public void AddParent(LetterTile parent)
    {
        _parentList.Add(parent);
        SetLock(true);
    }
    public void RemoveParent(LetterTile parent)
    {
        _parentList.Remove(parent);
        if (_parentList.Count == 0)
        {
            SetLock(false);
        }
    }
    public void SetInWordArea(bool state)
    {
        _isLocked = state;
        
        var block = button.colors;

        if (!state) return;
        
        block.disabledColor = button.colors.normalColor;
        button.colors = block;
    }
    public void SetChildren(List<LetterTile> tileDataChildren)
    {
        _childrenList = tileDataChildren;
        
        foreach (var child in _childrenList)
        {
            child.SetLock(true, this);
        }
    }
    
    public void ResetTile()
    {
        _isLocked = false;
        tileImage.color = normalColor;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
        _parentList.Clear();
        _childrenList.Clear();
        letterText.text = "";
        _originalParent = null;
        _originalTransform = null;
        transform.localScale = Vector3.one;
    }
    
    public void MoveToOriginalPosition(Vector3 originalPosition)
    {
        _tileManager.AddToActiveTiles(this);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
        
        transform.DOMove(originalPosition, 0.5f).SetEase(Ease.OutQuad);
        
        foreach (var child in _childrenList)
        {
            child.SetLock(true);
        }
    }
    
    #endregion






}
}
