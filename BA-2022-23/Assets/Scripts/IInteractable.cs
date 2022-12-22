using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool PlayerInTrigger { get; set; }
    bool CanInteract { get; set; }

    public abstract void Interact();
    public abstract void ShowOutline();

}
