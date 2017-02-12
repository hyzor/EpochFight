using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IEntityMessageHandler : IEventSystemHandler
{
    void ReceiveDamage(int dmg);
}
