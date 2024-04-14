using UnityEngine;

public interface Slottable
{
    public void Slot(int index, GameObject g);

    public GameObject Retrieve(int index);

    public bool IsEmpty();
}
