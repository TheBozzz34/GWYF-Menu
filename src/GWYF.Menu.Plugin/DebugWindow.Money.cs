using System;
using System.Globalization;
using Extensions;
using UnityEngine;

public partial class DebugWindow
{
	private void DrawMoneyTab()
	{
		GUILayout.Label("=== Money ===", Array.Empty<GUILayoutOption>());
		GUIStyle statusStyle = new GUIStyle(GUI.skin.label)	
		{
			wordWrap = true
		};
		MoneyManager moneyManager = NetworkSingleton<MoneyManager>.Instance;
		GUILayout.Label("MoneyManager: " + ((moneyManager != null) ? "found" : "not found"), Array.Empty<GUILayoutOption>());
		GUILayout.Label("PlayerProfile: " + ((this.playerProfile != null) ? this.playerProfile.playerName : "not found"), Array.Empty<GUILayoutOption>());
		GUILayout.Space(6f);
		this.DrawMoneyBalanceControls(moneyManager);
		GUILayout.Space(10f);
		this.DrawMoneyTicketControls(moneyManager);
		if (!string.IsNullOrEmpty(this.moneyStatus))
		{
			GUILayout.Space(8f);
			GUILayout.Label(this.moneyStatus, statusStyle, Array.Empty<GUILayoutOption>());
		}
	}

	private void DrawMoneyBalanceControls(MoneyManager moneyManager)
	{
		GUILayout.Label("Balance", Array.Empty<GUILayoutOption>());
		this.moneyChangeTypeIndex = Mathf.Clamp(this.moneyChangeTypeIndex, 0, DebugWindow.MoneyChangeTypeNames.Length - 1);
		GUILayout.Label("Change Type", Array.Empty<GUILayoutOption>());
		this.moneyChangeTypeIndex = GUILayout.SelectionGrid(this.moneyChangeTypeIndex, DebugWindow.MoneyChangeTypeNames, 3, Array.Empty<GUILayoutOption>());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Amount", new GUILayoutOption[] { GUILayout.Width(80f) });
		this.moneyBalanceAmountInput = GUILayout.TextField(this.moneyBalanceAmountInput, new GUILayoutOption[] { GUILayout.Width(120f) });
		if (GUILayout.Button("Apply +", new GUILayoutOption[] { GUILayout.Width(80f) }))
		{
			this.ApplyMoneyBalanceDelta(moneyManager, 1L);
		}
		if (GUILayout.Button("Apply -", new GUILayoutOption[] { GUILayout.Width(80f) }))
		{
			this.ApplyMoneyBalanceDelta(moneyManager, -1L);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("+100", Array.Empty<GUILayoutOption>()))
		{
			this.ApplyMoneyBalanceAmount(moneyManager, 100L);
		}
		if (GUILayout.Button("+1,000", Array.Empty<GUILayoutOption>()))
		{
			this.ApplyMoneyBalanceAmount(moneyManager, 1000L);
		}
		if (GUILayout.Button("-100", Array.Empty<GUILayoutOption>()))
		{
			this.ApplyMoneyBalanceAmount(moneyManager, -100L);
		}
		GUILayout.EndHorizontal();
	}

	private void DrawMoneyTicketControls(MoneyManager moneyManager)
	{
		GUILayout.Label("Tickets", Array.Empty<GUILayoutOption>());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Amount", new GUILayoutOption[] { GUILayout.Width(80f) });
		this.moneyTicketAmountInput = GUILayout.TextField(this.moneyTicketAmountInput, new GUILayoutOption[] { GUILayout.Width(120f) });
		if (GUILayout.Button("Apply +", new GUILayoutOption[] { GUILayout.Width(80f) }))
		{
			this.ApplyMoneyTicketDelta(moneyManager, 1L);
		}
		if (GUILayout.Button("Apply -", new GUILayoutOption[] { GUILayout.Width(80f) }))
		{
			this.ApplyMoneyTicketDelta(moneyManager, -1L);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("+1", Array.Empty<GUILayoutOption>()))
		{
			this.ApplyMoneyTicketAmount(moneyManager, 1L);
		}
		if (GUILayout.Button("+10", Array.Empty<GUILayoutOption>()))
		{
			this.ApplyMoneyTicketAmount(moneyManager, 10L);
		}
		if (GUILayout.Button("-1", Array.Empty<GUILayoutOption>()))
		{
			this.ApplyMoneyTicketAmount(moneyManager, -1L);
		}
		GUILayout.EndHorizontal();
	}

	private void ApplyMoneyBalanceDelta(MoneyManager moneyManager, long multiplier)
	{
		if (moneyManager == null)
		{
			this.moneyStatus = "MoneyManager not found.";
			return;
		}
		if (this.playerProfile == null)
		{
			this.moneyStatus = "PlayerProfile not found.";
			return;
		}
		long amount;
		if (!this.TryParseMoneyAmount(this.moneyBalanceAmountInput, out amount))
		{
			this.moneyStatus = "Balance amount must be a whole number.";
			return;
		}
		amount *= multiplier;
		this.ApplyMoneyBalanceAmount(moneyManager, amount);
	}

	private void ApplyMoneyBalanceAmount(MoneyManager moneyManager, long amount)
	{
		if (moneyManager == null)
		{
			this.moneyStatus = "MoneyManager not found.";
			return;
		}
		if (this.playerProfile == null)
		{
			this.moneyStatus = "PlayerProfile not found.";
			return;
		}
		ChangeType changeType = this.GetSelectedMoneyChangeType();
		try
		{
			moneyManager.TryChangeBalance(amount, this.playerProfile, changeType);
			this.moneyStatus = "Balance change sent: " + amount.ToString(CultureInfo.InvariantCulture) + " (" + changeType.ToString() + ").";
		}
		catch (Exception ex)
		{
			this.moneyStatus = "Balance change failed: " + ex.GetType().Name + " - " + ex.Message;
		}
	}

	private void ApplyMoneyTicketDelta(MoneyManager moneyManager, long multiplier)
	{
		if (moneyManager == null)
		{
			this.moneyStatus = "MoneyManager not found.";
			return;
		}
		long amount;
		if (!this.TryParseMoneyAmount(this.moneyTicketAmountInput, out amount))
		{
			this.moneyStatus = "Ticket amount must be a whole number.";
			return;
		}
		amount *= multiplier;
		this.ApplyMoneyTicketAmount(moneyManager, amount);
	}

	private void ApplyMoneyTicketAmount(MoneyManager moneyManager, long amount)
	{
		if (moneyManager == null)
		{
			this.moneyStatus = "MoneyManager not found.";
			return;
		}
		try
		{
			moneyManager.TryChangeTicketBalance(amount);
			this.moneyStatus = "Ticket change sent: " + amount.ToString(CultureInfo.InvariantCulture) + ".";
		}
		catch (Exception ex)
		{
			this.moneyStatus = "Ticket change failed: " + ex.GetType().Name + " - " + ex.Message;
		}
	}

	private bool TryParseMoneyAmount(string input, out long amount)
	{
		if (string.IsNullOrEmpty(input))
		{
			amount = 0L;
			return false;
		}
		return long.TryParse(input.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out amount);
	}

	private ChangeType GetSelectedMoneyChangeType()
	{
		Array values = Enum.GetValues(typeof(ChangeType));
		if (values == null || values.Length == 0)
		{
			return ChangeType.Misc;
		}
		this.moneyChangeTypeIndex = Mathf.Clamp(this.moneyChangeTypeIndex, 0, values.Length - 1);
		return (ChangeType)values.GetValue(this.moneyChangeTypeIndex);
	}

	private string moneyBalanceAmountInput = "100";

	private string moneyTicketAmountInput = "1";

	private int moneyChangeTypeIndex = Array.IndexOf(DebugWindow.MoneyChangeTypeNames, ChangeType.Misc.ToString());

	private string moneyStatus = "";

	private static readonly string[] MoneyChangeTypeNames = Enum.GetNames(typeof(ChangeType));
}
