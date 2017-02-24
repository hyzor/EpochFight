using System;

public struct ResourceConsumedEvent{
	public int amount;
	public int currentBalance;

	public ResourceConsumedEvent(int amount, int currentBalance) {
		this.amount = amount;
		this.currentBalance = currentBalance;
	}
}

