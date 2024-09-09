using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum SelectionState
{
    Inert,
    NotSelectable,
    Selectable,
    Selected
}