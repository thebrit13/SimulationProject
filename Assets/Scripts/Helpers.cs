using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    #region UI
    public static void RemoveAllChildrenUI(this Transform t)
    {
        for (int i = t.childCount-1; i >= 0; i--)
        {
            GameObject.Destroy(t.GetChild(i).gameObject);
        }
    }
    #endregion
}
