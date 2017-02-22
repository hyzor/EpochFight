using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoToTask : BaseTask {

    public override void OnDestReached()
    {
        completed = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (!isBusy)
        {
            if (this.transform.parent != null)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitResume());
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitToCoords(taskCoords));
                isBusy = true;
                isActive = true;
            }
        }
    }
}
