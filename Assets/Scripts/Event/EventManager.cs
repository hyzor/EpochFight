using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
	public static EventManager instance;
	private Dictionary<System.Type,List<EventListener>> map;

	void Awake(){
		if (instance == null) {
			instance = this;
		}
		map = new Dictionary<System.Type,List<EventListener>> ();
	}

	public void register(System.Type type, EventListener listener) {
		List<EventListener> listeners = map [type];
		if (listeners == null) {
			listeners = new List<EventListener> ();
			map.Add (type, listeners);
		}
		if(!listeners.Contains(listener)) {
			listeners.Add(listener);
		}
	}
		
	public void post(object e) {
		List<EventListener> listeners = map[e.GetType()];
		if(listeners == null) {
			return;
		}
		foreach(EventListener l in listeners) {
			l.onEvent(e);
		}
	}
}
