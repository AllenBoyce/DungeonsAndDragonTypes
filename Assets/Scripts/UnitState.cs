using UnityEngine;

abstract class UnitState {
    string stateName;

    public string StateName {
        get {
            return stateName;
        }
        set {
            stateName = value;
        }
    }   
}