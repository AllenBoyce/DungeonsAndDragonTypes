using UnityEngine;
using System.Collections.Generic;
public class APParent : MonoBehaviour
{
    [SerializeField] private List<APDisplay> _apDisplays;
    private int _apDisplayCount;
    private int _currentFullAP;
    private int _currentTempAP;
    void Start()
    {
        _apDisplayCount = _apDisplays.Count;
    }

    public void SetAP(int fullAP)
    {
        _currentFullAP = fullAP;
        _currentTempAP = 0;
        UpdateAPDisplays();
    }
    public void SetTempAP(int tempAP)
    {
        _currentTempAP = tempAP;
        _currentFullAP -= tempAP;
        UpdateAPDisplays();
    }

    public void DisplayTempAP(int tempAP)
    {
        int emptyAP = _apDisplayCount - _currentFullAP - _currentTempAP;
        int fullAP = _apDisplayCount - emptyAP - tempAP;
        for(int i = 0; i < fullAP; i++)
        {
            _apDisplays[i].SetAP(2);
        }
        for(int i = fullAP; i < fullAP + tempAP; i++)
        {
            _apDisplays[i].SetAP(1);
        }
        for(int i = fullAP + tempAP; i < _apDisplayCount; i++)
        {
            _apDisplays[i].SetAP(0);
        }
    }

    public void SetAP(int fullAP, int tempAP)
    {
        SetAP(fullAP);
        SetTempAP(tempAP);
    }

    public void UpdateAP(Unit u)
    {
        SetAP(u.CurrentAP);
    }

    private void UpdateAPDisplays()
    {
        if(_currentFullAP + _currentTempAP > _apDisplayCount) {Debug.LogError("APParent: UpdateAPDisplays: _currentFullAP + _currentTempAP > _apDisplayCount"); return;}
        ResetAPDisplays();
        for(int i = 0; i < _currentFullAP; i++)
        {
            _apDisplays[i].SetAP(2);
        }
        for(int i = _currentFullAP; i < _currentFullAP + _currentTempAP; i++)
        {
            _apDisplays[i].SetAP(1);
        }
    }

    private void ResetAPDisplays()
    {
        for(int i = 0; i < _apDisplays.Count; i++)
        {
            _apDisplays[i].SetAP(0);
        }
    }

    public void ClearAP()
    {
        for (int i = 0; i < _apDisplays.Count; i++)
        {
            _apDisplays[i].gameObject.SetActive(false);
        }
    }

    public void ShowAP()
    {
        for (int i = 0; i < _apDisplays.Count; i++)
        {   
            _apDisplays[i].gameObject.SetActive(true);  
        }
    }
}
