using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour, EventListener {
	public int amount;
	private EventManager eventManager;

	void Start() {
		eventManager = EventManager.instance;
	}

	public bool spendAmount(int amount) {
		if (this.amount >= amount) {
			this.amount -= amount;
			eventManager.post (new ResourceConsumedEvent(amount,this.amount));
			return true;
		}
		return false;
	}

	public int getAmount() {
		return amount;
	}

	public void addAmount(int amount) {
		this.amount = amount;
	}

	public void onEvent(object e) {
		
	}
}
