using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    public void Execute();
    public void PrepareGridVisual();
    public void UndoGridVisual();

    public void PrepareGridData();
    public void UndoGridData();

}
