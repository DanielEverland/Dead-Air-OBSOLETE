using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugElement : MonoBehaviour {

    private DebugMenuCategory _category;
    private bool _isDirty;

    public virtual void Initialize(DebugMenuCategory category)
    {
        _category = category;
    }

    protected void SetDirty()
    {
        if(!_isDirty && _category != null)
            _category.SetDirty();
    }
    public void DoReload()
    {
        _isDirty = true;

        Reload();

        _isDirty = false;
    }
    protected abstract void Reload();
}
