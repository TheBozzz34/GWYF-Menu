using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Extensions;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003A9 RID: 937
public class DebugWindow : MonoBehaviour
{
	// Token: 0x06001EC1 RID: 7873
	private void OnGUI()
	{
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F8)
		{
			this.visible = !this.visible;
		}
		if (!this.visible)
		{
			return;
		}
		this.BuildLines();
		float num = (float)Screen.height - 20f;
		this.windowRect.width = Mathf.Clamp(this.windowRect.width, 340f, 860f);
		this.windowRect.height = Mathf.Clamp(this.windowRect.height, 420f, num);
		this.windowRect.x = Mathf.Clamp(this.windowRect.x, 10f, (float)Screen.width - this.windowRect.width - 10f);
		this.windowRect.y = Mathf.Clamp(this.windowRect.y, 10f, (float)Screen.height - this.windowRect.height - 10f);
		this.windowRect = GUI.Window(987654, this.windowRect, new GUI.WindowFunction(this.DrawWindow), "GWYF Menu");
	}

	// Token: 0x06001EC2 RID: 7874
	private void DrawWindow(int id)
	{
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		for (int i = 0; i < DebugWindow.TabNames.Length; i++)
		{
			GUIStyle guistyle = ((i == this.activeTab) ? GUI.skin.box : GUI.skin.button);
			if (GUILayout.Button(DebugWindow.TabNames[i], guistyle, new GUILayoutOption[] { GUILayout.Height(26f) }))
			{
				this.activeTab = i;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(4f);
		float num = (float)GUI.skin.window.padding.vertical + 26f + 4f + 32f + 8f;
		float num2 = Mathf.Max(60f, this.windowRect.height - num);
		this.tabScrollPositions[this.activeTab] = GUILayout.BeginScrollView(this.tabScrollPositions[this.activeTab], new GUILayoutOption[] { GUILayout.Height(num2) });
		switch (this.activeTab)
		{
		case 0:
			this.DrawInfoTab();
			break;
		case 1:
			this.DrawControlsTab();
			break;
		case 2:
			this.DrawPlayerSettingsTab();
			break;
		case 3:
			this.DrawScanTab();
			break;
		case 4:
			this.DrawClassEditorTab();
			break;
		case 5:
			this.DrawOrgansTab();
			break;
		case 6:
			this.DrawTextFeedbackTab();
			break;
		case 7:
			this.DrawImageFeedbackTab();
			break;
		}
		GUILayout.EndScrollView();
		GUILayout.Space(8f);
		if (GUILayout.Button("Close", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.visible = false;
		}
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
	}

	// Token: 0x06001EC4 RID: 7876
	private GameObject FindLocalPlayerObject()
	{
		try
		{
			if (NetworkClient.active && NetworkClient.localPlayer != null)
			{
				return NetworkClient.localPlayer.gameObject;
			}
		}
		catch
		{
		}
		foreach (NetworkIdentity networkIdentity in global::UnityEngine.Object.FindObjectsOfType<NetworkIdentity>())
		{
			if (networkIdentity != null && networkIdentity.isLocalPlayer)
			{
				return networkIdentity.gameObject;
			}
		}
		return null;
	}

	// Token: 0x06001EC5 RID: 7877
	private void BuildLines()
	{
		this.lines.Clear();
		this.lines.Add(this.message);
		this.lines.Add("");
		this.lines.Add("Unity Time: " + Time.time.ToString("0.00"));
		this.lines.Add("");
		GameObject gameObject = this.FindLocalPlayerObject();
		if (gameObject == null)
		{
			this.lines.Add("Local player: not found");
			return;
		}
		this.lines.Add("Local player: " + gameObject.name);
		this.noclip = gameObject.GetComponent<Noclip>();
		this.lines.Add("Noclip: " + ((this.noclip != null) ? this.noclip.enabled.ToString() : "not found"));
		this.playerProfile = gameObject.GetComponent<PlayerProfile>();
		this.lines.Add("PlayerProfile: " + ((this.playerProfile != null) ? "found" : "not found"));
		this.playerInventory = gameObject.GetComponent<PlayerInventory>();
		this.lines.Add("PlayerInventory: " + ((this.playerInventory != null) ? "found" : "not found"));
		this.playerOrgans = gameObject.GetComponent<PlayerOrgans>();
		this.lines.Add("PlayerOrgans: " + ((this.playerOrgans != null) ? "found" : "not found"));
		this.bank = this.FindBank();
		this.lines.Add("Bank: " + ((this.bank != null) ? "found" : "not found"));
		if (this.bank != null)
		{
			this.lines.Add("Bank object: " + this.bank.gameObject.name);
			NetworkIdentity component = this.bank.GetComponent<NetworkIdentity>();
			if (component != null)
			{
				this.lines.Add("Bank netId: " + component.netId.ToString());
				this.lines.Add("Bank hasAuthority: " + component.isOwned.ToString());
				this.lines.Add("Bank isServer: " + this.bank.isServer.ToString());
				this.lines.Add("Bank isClient: " + this.bank.isClient.ToString());
			}
		}
		this.playerSettings = this.FindPlayerSettings(gameObject);
		this.lines.Add("PlayerSettings: " + ((this.playerSettings != null) ? "found" : "not found"));
	}

	// Token: 0x06001EC6 RID: 7878
	private Bank FindBank()
	{
		Bank[] array = global::UnityEngine.Object.FindObjectsOfType<Bank>();
		if (array != null && array.Length != 0)
		{
			return array[0];
		}
		return null;
	}

	// Token: 0x06001EC7 RID: 7879
	private void DrawInfoTab()
	{
		GUIStyle guistyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		foreach (string text in this.lines)
		{
			if (string.IsNullOrEmpty(text))
			{
				GUILayout.Space(6f);
			}
			else
			{
				GUILayout.Label(text, guistyle, Array.Empty<GUILayoutOption>());
			}
		}
	}

	// Token: 0x06001EC8 RID: 7880
	private void DrawControlsTab()
	{
		GUILayout.Label("=== Movement ===", Array.Empty<GUILayoutOption>());
		GUILayout.Space(4f);
		if (GUILayout.Button("Toggle Noclip (tcl)", new GUILayoutOption[] { GUILayout.Height(28f) }))
		{
			if (this.noclip != null)
			{
				this.noclip.ToggleNoclip();
			}
			else
			{
				GUILayout.Label("Noclip component not found.", Array.Empty<GUILayoutOption>());
			}
		}
		GUILayout.Space(8f);
		GUILayout.Label("=== Keybinds ===", Array.Empty<GUILayoutOption>());
		GUIStyle guistyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		GUILayout.Label("F8  -  Toggle this window", guistyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label(this.mouseRevealKey.ToString() + "  -  Hold to show/unlock mouse", guistyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x06001EC9 RID: 7881
	private void DrawPlayerSettingsTab()
	{
		if (this.playerSettings == null)
		{
			GUILayout.Label("PlayerSettings: not found on local player.", Array.Empty<GUILayoutOption>());
			return;
		}
		GUILayout.Label("=== Player Settings ===", Array.Empty<GUILayoutOption>());
		GUILayout.Space(6f);
		foreach (FieldInfo fieldInfo in typeof(PlayerSettings).GetFields(BindingFlags.Instance | BindingFlags.Public))
		{
			if (fieldInfo.FieldType == typeof(float))
			{
				this.DrawFloatSetting(fieldInfo);
			}
			else if (fieldInfo.FieldType == typeof(LayerMask))
			{
				this.DrawLayerMaskSetting(fieldInfo);
			}
		}
		GUILayout.Space(8f);
		if (GUILayout.Button("Notify PlayerSettings Changed", Array.Empty<GUILayoutOption>()))
		{
			this.playerSettings.NotifyChanged();
		}
		if (GUILayout.Button("Reset Input Cache", Array.Empty<GUILayoutOption>()))
		{
			this.playerSettingInputs.Clear();
		}
		if (GUILayout.Button("TryChangeTicketBalance", Array.Empty<GUILayoutOption>()))
		{
			NetworkSingleton<MoneyManager>.Instance.TryChangeTicketBalance(1L);
		}
		if (GUILayout.Button("TryChangeBalance", Array.Empty<GUILayoutOption>()))
		{
			NetworkSingleton<MoneyManager>.Instance.TryChangeBalance(100L, this.playerProfile, ChangeType.Misc);
		}
	}

	// Token: 0x06001ECA RID: 7882
	private void DrawScanTab()
	{
		if (GUILayout.Button("Scan Local Player Components", new GUILayoutOption[] { GUILayout.Height(28f) }))
		{
			this.scanClickCount++;
			this.scanOutput = "Scan #" + this.scanClickCount.ToString() + "\n";
			GameObject gameObject = this.FindLocalPlayerObject();
			if (gameObject == null)
			{
				this.scanOutput += "Local player: not found";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Local player: " + gameObject.name);
				Component[] components = gameObject.GetComponents<Component>();
				stringBuilder.AppendLine("Component count: " + components.Length.ToString());
				for (int i = 0; i < components.Length; i++)
				{
					stringBuilder.AppendLine((components[i] != null) ? components[i].GetType().FullName : "<null component>");
				}
				this.scanOutput += stringBuilder.ToString();
			}
		}
		GUILayout.Space(6f);
		if (!string.IsNullOrEmpty(this.scanOutput))
		{
			GUIStyle guistyle = new GUIStyle(GUI.skin.label)
			{
				wordWrap = true
			};
			GUILayout.Label(this.scanOutput, guistyle, Array.Empty<GUILayoutOption>());
		}
	}

	// Token: 0x06001ECB RID: 7883
	private void DrawFloatSetting(FieldInfo field)
	{
		float num = (float)field.GetValue(this.playerSettings);
		if (!this.playerSettingInputs.ContainsKey(field.Name))
		{
			this.playerSettingInputs[field.Name] = num.ToString("0.###", CultureInfo.InvariantCulture);
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label(field.Name, new GUILayoutOption[] { GUILayout.Width(160f) });
		string text = GUILayout.TextField(this.playerSettingInputs[field.Name], new GUILayoutOption[] { GUILayout.Width(80f) });
		this.playerSettingInputs[field.Name] = text;
		if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(24f) }))
		{
			float num2 = num - this.GetStepForSetting(field.Name);
			field.SetValue(this.playerSettings, num2);
			this.playerSettingInputs[field.Name] = num2.ToString("0.###", CultureInfo.InvariantCulture);
			this.playerSettings.NotifyChanged();
		}
		if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(24f) }))
		{
			float num3 = num + this.GetStepForSetting(field.Name);
			field.SetValue(this.playerSettings, num3);
			this.playerSettingInputs[field.Name] = num3.ToString("0.###", CultureInfo.InvariantCulture);
			this.playerSettings.NotifyChanged();
		}
		float num4;
		if (GUILayout.Button("Set", new GUILayoutOption[] { GUILayout.Width(40f) }) && float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num4))
		{
			field.SetValue(this.playerSettings, num4);
			this.playerSettingInputs[field.Name] = num4.ToString("0.###", CultureInfo.InvariantCulture);
			this.playerSettings.NotifyChanged();
		}
		GUILayout.EndHorizontal();
	}

	// Token: 0x06001ECC RID: 7884
	private void DrawLayerMaskSetting(FieldInfo field)
	{
		LayerMask layerMask = (LayerMask)field.GetValue(this.playerSettings);
		string name = field.Name;
		if (!this.playerSettingInputs.ContainsKey(name))
		{
			this.playerSettingInputs[name] = layerMask.value.ToString();
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label(field.Name, new GUILayoutOption[] { GUILayout.Width(160f) });
		string text = GUILayout.TextField(this.playerSettingInputs[name], new GUILayoutOption[] { GUILayout.Width(80f) });
		this.playerSettingInputs[name] = text;
		int num;
		if (GUILayout.Button("Set", new GUILayoutOption[] { GUILayout.Width(40f) }) && int.TryParse(text, out num))
		{
			LayerMask layerMask2 = new LayerMask
			{
				value = num
			};
			field.SetValue(this.playerSettings, layerMask2);
			this.playerSettings.NotifyChanged();
		}
		GUILayout.EndHorizontal();
	}

	// Token: 0x06001ECD RID: 7885
	private float GetStepForSetting(string fieldName)
	{
		string text = fieldName.ToLowerInvariant();
		if (text.Contains("speed"))
		{
			return 1f;
		}
		if (text.Contains("force"))
		{
			return 1f;
		}
		if (text.Contains("gravity"))
		{
			return 1f;
		}
		if (text.Contains("angle"))
		{
			return 5f;
		}
		if (text.Contains("duration"))
		{
			return 0.1f;
		}
		if (text.Contains("radius"))
		{
			return 0.05f;
		}
		if (text.Contains("height"))
		{
			return 0.05f;
		}
		if (text.Contains("distance"))
		{
			return 0.05f;
		}
		return 0.1f;
	}

	// Token: 0x06001ECE RID: 7886
	private PlayerSettings FindPlayerSettings(GameObject localPlayer)
	{
		if (localPlayer == null)
		{
			return null;
		}
		foreach (Component component in localPlayer.GetComponents<Component>())
		{
			if (!(component == null))
			{
				Type type = component.GetType();
				foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (typeof(PlayerSettings).IsAssignableFrom(fieldInfo.FieldType))
					{
						try
						{
							PlayerSettings playerSettings = fieldInfo.GetValue(component) as PlayerSettings;
							if (playerSettings != null)
							{
								return playerSettings;
							}
						}
						catch
						{
						}
					}
				}
				foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (propertyInfo.CanRead && typeof(PlayerSettings).IsAssignableFrom(propertyInfo.PropertyType))
					{
						try
						{
							PlayerSettings playerSettings2 = propertyInfo.GetValue(component, null) as PlayerSettings;
							if (playerSettings2 != null)
							{
								return playerSettings2;
							}
						}
						catch
						{
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06001EDC RID: 7900
	private void DrawClassEditorTab()
	{
		GameObject gameObject = this.FindLocalPlayerObject();
		if (gameObject == null)
		{
			GUILayout.Label("Local player: not found", Array.Empty<GUILayoutOption>());
			return;
		}
		GUILayout.Label("=== Class Editor ===", Array.Empty<GUILayoutOption>());
		GUIStyle guistyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		GUILayout.Label("Runtime editor for local-player classes. Mirror SyncVar changes replicate only when they are applied on the server/host authority; on a remote client they are just local debug edits.", guistyle, Array.Empty<GUILayoutOption>());
		GUILayout.Space(6f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button(this.classEditorShowPrivate ? "Hide Private Fields" : "Show Private Fields", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.classEditorShowPrivate = !this.classEditorShowPrivate;
		}
		if (this.classEditorShowPrivate && GUILayout.Button(this.classEditorEditPrivate ? "Lock Private Editing" : "Allow Private Editing", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.classEditorEditPrivate = !this.classEditorEditPrivate;
		}
		if (GUILayout.Button("Reset Inputs", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.classEditorInputs.Clear();
		}
		GUILayout.EndHorizontal();
		if (!string.IsNullOrEmpty(this.classEditorStatus))
		{
			GUILayout.Label(this.classEditorStatus, guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(4f);
		}
		List<object> list = new List<object>();
		List<string> list2 = new List<string>();
		this.AddClassEditorTarget(list, list2, this.playerProfile, "PlayerProfile");
		this.AddClassEditorTarget(list, list2, this.playerInventory, "PlayerInventory");
		this.AddClassEditorTarget(list, list2, this.playerOrgans, "PlayerOrgans");
		this.AddClassEditorTarget(list, list2, this.playerSettings, "PlayerSettings");
		foreach (Component component in gameObject.GetComponents<Component>())
		{
			if (!(component == null) && !this.ContainsClassEditorTarget(list, component))
			{
				this.AddClassEditorTarget(list, list2, component, component.GetType().Name);
			}
		}
		if (list.Count == 0)
		{
			GUILayout.Label("No editable targets found on local player.", Array.Empty<GUILayoutOption>());
			return;
		}
		this.classEditorTargetIndex = Mathf.Clamp(this.classEditorTargetIndex, 0, list.Count - 1);
		GUILayout.Label("Target", Array.Empty<GUILayoutOption>());
		this.classEditorTargetIndex = GUILayout.SelectionGrid(this.classEditorTargetIndex, list2.ToArray(), 2, Array.Empty<GUILayoutOption>());
		GUILayout.Space(8f);
		this.DrawObjectClassEditor(list[this.classEditorTargetIndex], list2[this.classEditorTargetIndex]);
	}

	// Token: 0x06001EDD RID: 7901
	private void AddClassEditorTarget(List<object> targets, List<string> names, object target, string displayName)
	{
		if (target == null)
		{
			return;
		}
		if (this.ContainsClassEditorTarget(targets, target))
		{
			return;
		}
		targets.Add(target);
		names.Add(displayName);
	}

	// Token: 0x06001EDE RID: 7902
	private bool ContainsClassEditorTarget(List<object> targets, object target)
	{
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] == target)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001EDF RID: 7903
	private void DrawObjectClassEditor(object target, string displayName)
	{
		if (target == null)
		{
			return;
		}
		Type type = target.GetType();
		GUILayout.Label("=== " + displayName + " ===", Array.Empty<GUILayoutOption>());
		GUILayout.Label(type.FullName, Array.Empty<GUILayoutOption>());
		NetworkBehaviour networkBehaviour = target as NetworkBehaviour;
		if (networkBehaviour != null)
		{
			GUILayout.Label(string.Concat(new string[]
			{
				"netId: ",
				networkBehaviour.netId.ToString(),
				" | local: ",
				networkBehaviour.isLocalPlayer.ToString(),
				" | server: ",
				networkBehaviour.isServer.ToString(),
				" | client: ",
				networkBehaviour.isClient.ToString(),
				" | owned: ",
				networkBehaviour.isOwned.ToString()
			}), Array.Empty<GUILayoutOption>());
		}
		GUILayout.Space(6f);
		PlayerInventory playerInventory = target as PlayerInventory;
		if (playerInventory != null)
		{
			this.DrawInventoryClassEditor(playerInventory);
			GUILayout.Space(8f);
		}
		PlayerProfile playerProfile = target as PlayerProfile;
		if (playerProfile != null)
		{
			this.DrawProfileClassEditor(playerProfile);
			GUILayout.Space(8f);
		}
		PlayerOrgans playerOrgans = target as PlayerOrgans;
		if (playerOrgans != null)
		{
			this.DrawOrgansQuickEditor(playerOrgans);
			GUILayout.Space(8f);
		}
		GUILayout.Label("Fields", Array.Empty<GUILayoutOption>());
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
		if (this.classEditorShowPrivate)
		{
			bindingFlags |= BindingFlags.NonPublic;
		}
		FieldInfo[] fields = type.GetFields(bindingFlags);
		if (fields == null || fields.Length == 0)
		{
			GUILayout.Label("No fields found for current visibility filter.", Array.Empty<GUILayoutOption>());
			return;
		}
		for (int i = 0; i < fields.Length; i++)
		{
			this.DrawClassEditorField(target, fields[i]);
		}
	}

	// Token: 0x06001EE0 RID: 7904
	private void DrawInventoryClassEditor(PlayerInventory inventory)
	{
		GUILayout.Label("Inventory quick editor", Array.Empty<GUILayoutOption>());
		GUILayout.Label("Holding: " + this.FormatClassEditorValue(inventory.NetworkholdingItem), Array.Empty<GUILayoutOption>());
		GUILayout.Label("Slots: " + inventory.Pockets.Count.ToString() + " | configured slot count: " + inventory.inventorySlotCount.ToString(), Array.Empty<GUILayoutOption>());
		for (int i = 0; i < inventory.Pockets.Count; i++)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Slot " + (i + 1).ToString(), new GUILayoutOption[] { GUILayout.Width(60f) });
			GUILayout.Label(this.FormatClassEditorValue(inventory.Pockets[i]), new GUILayoutOption[] { GUILayout.Width(260f) });
			bool enabled = GUI.enabled;
			GUI.enabled = enabled && inventory.isLocalPlayer;
			if (GUILayout.Button("Select", new GUILayoutOption[] { GUILayout.Width(70f) }))
			{
				this.TryInvokeClassEditorMethod(inventory, "CmdSelectSlot", new object[] { i });
			}
			GUI.enabled = enabled;
			GUILayout.EndHorizontal();
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		bool enabled2 = GUI.enabled;
		GUI.enabled = enabled2 && NetworkServer.active && inventory.isServer;
		if (GUILayout.Button("Server Drop Holding", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			inventory.ServerDropHoldingItem();
			this.classEditorStatus = "ServerDropHoldingItem called.";
		}
		if (GUILayout.Button("Server Throw Random", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			inventory.ServerThrowItemRandomly();
			this.classEditorStatus = "ServerThrowItemRandomly called.";
		}
		GUI.enabled = enabled2;
		GUILayout.EndHorizontal();
		if (!NetworkServer.active || !inventory.isServer)
		{
			GUILayout.Label("Inventory mutation buttons are disabled unless this instance is server/host. Slot Select still uses the normal local-player command path.", new GUIStyle(GUI.skin.label)
			{
				wordWrap = true
			}, Array.Empty<GUILayoutOption>());
		}
	}

	// Token: 0x06001EE1 RID: 7905
	private void DrawProfileClassEditor(PlayerProfile profile)
	{
		GUILayout.Label("Profile quick editor", Array.Empty<GUILayoutOption>());
		if (!profile.isServer)
		{
			GUILayout.Label("This profile is not running as server/host, so SyncVar edits here will not replicate to other clients.", new GUIStyle(GUI.skin.label)
			{
				wordWrap = true
			}, Array.Empty<GUILayoutOption>());
		}
		string text = this.MakeClassEditorKey(profile, "quick.playerName");
		if (!this.classEditorInputs.ContainsKey(text))
		{
			this.classEditorInputs[text] = profile.playerName ?? "";
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("playerName", new GUILayoutOption[] { GUILayout.Width(130f) });
		this.classEditorInputs[text] = GUILayout.TextField(this.classEditorInputs[text], new GUILayoutOption[] { GUILayout.Width(180f) });
		if (GUILayout.Button("Apply", new GUILayoutOption[] { GUILayout.Width(70f) }))
		{
			profile.NetworkplayerName = this.classEditorInputs[text];
			this.TryInvokeClassEditorMethod(profile, "SetPlayerNameTag", Array.Empty<object>());
			this.classEditorStatus = "Profile name applied.";
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("steamId", new GUILayoutOption[] { GUILayout.Width(130f) });
		GUILayout.Label(profile.steamId.ToString(CultureInfo.InvariantCulture), new GUILayoutOption[] { GUILayout.Width(180f) });
		GUILayout.Label("hasSynced: " + profile.hasSynced.ToString(), Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button(profile.hasSynced ? "Set hasSynced False" : "Set hasSynced True", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			profile.NetworkhasSynced = !profile.hasSynced;
			this.classEditorStatus = "Profile hasSynced toggled.";
		}
		if (GUILayout.Button("Raise Profile Updated Action", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			Action onPlayerProfileUpdated = profile.OnPlayerProfileUpdated;
			if (onPlayerProfileUpdated != null)
			{
				onPlayerProfileUpdated();
			}
			this.classEditorStatus = "OnPlayerProfileUpdated raised if subscribers existed.";
		}
		GUILayout.EndHorizontal();
	}

	// Token: 0x06001EE2 RID: 7906
	private void DrawClassEditorField(object target, FieldInfo field)
	{
		if (field == null || field.IsStatic || field.IsSpecialName)
		{
			return;
		}
		if (typeof(Delegate).IsAssignableFrom(field.FieldType))
		{
			return;
		}
		object obj = null;
		try
		{
			obj = field.GetValue(target);
		}
		catch (Exception ex)
		{
			GUILayout.Label(field.Name + ": <read failed> " + ex.GetType().Name, Array.Empty<GUILayoutOption>());
			return;
		}
		string text = this.MakeClassEditorKey(target, field.DeclaringType.FullName + "." + field.Name);
		if (!this.classEditorInputs.ContainsKey(text))
		{
			this.classEditorInputs[text] = this.SerializeClassEditorValue(obj, field.FieldType);
		}
		string text2 = field.Name;
		if (this.IsSyncVarField(field))
		{
			text2 += " [SyncVar]";
		}
		if (!field.IsPublic)
		{
			text2 += " [private]";
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label(text2, new GUILayoutOption[] { GUILayout.Width(190f) });
		if (!this.CanEditClassEditorField(field))
		{
			GUILayout.Label(this.FormatClassEditorValue(obj), new GUILayoutOption[] { GUILayout.Width(300f) });
		}
		else if (field.FieldType == typeof(bool))
		{
			bool flag2 = obj is bool && (bool)obj;
			if (GUILayout.Button(flag2 ? "True" : "False", new GUILayoutOption[] { GUILayout.Width(70f) }))
			{
				this.SetClassEditorFieldValue(target, field, !flag2, text);
			}
			if (GUILayout.Button("Refresh", new GUILayoutOption[] { GUILayout.Width(70f) }))
			{
				this.classEditorInputs[text] = this.SerializeClassEditorValue(field.GetValue(target), field.FieldType);
			}
		}
		else if (field.FieldType.IsEnum)
		{
			this.classEditorInputs[text] = GUILayout.TextField(this.classEditorInputs[text], new GUILayoutOption[] { GUILayout.Width(160f) });
			if (GUILayout.Button("Set", new GUILayoutOption[] { GUILayout.Width(45f) }))
			{
				object obj2;
				if (this.TryParseClassEditorValue(field.FieldType, this.classEditorInputs[text], out obj2))
				{
					this.SetClassEditorFieldValue(target, field, obj2, text);
				}
				else
				{
					this.classEditorStatus = "Could not parse enum value for " + field.Name;
				}
			}
			if (GUILayout.Button("Next", new GUILayoutOption[] { GUILayout.Width(50f) }))
			{
				Array values = Enum.GetValues(field.FieldType);
				int num = 0;
				for (int i = 0; i < values.Length; i++)
				{
					if (object.Equals(values.GetValue(i), obj))
					{
						num = (i + 1) % values.Length;
						break;
					}
				}
				this.SetClassEditorFieldValue(target, field, values.GetValue(num), text);
			}
		}
		else
		{
			this.classEditorInputs[text] = GUILayout.TextField(this.classEditorInputs[text], new GUILayoutOption[] { GUILayout.Width(160f) });
			if (GUILayout.Button("Set", new GUILayoutOption[] { GUILayout.Width(45f) }))
			{
				object obj3;
				if (this.TryParseClassEditorValue(field.FieldType, this.classEditorInputs[text], out obj3))
				{
					this.SetClassEditorFieldValue(target, field, obj3, text);
				}
				else
				{
					this.classEditorStatus = "Could not parse value for " + field.Name;
				}
			}
			if (GUILayout.Button("Refresh", new GUILayoutOption[] { GUILayout.Width(70f) }))
			{
				this.classEditorInputs[text] = this.SerializeClassEditorValue(field.GetValue(target), field.FieldType);
			}
		}
		GUILayout.EndHorizontal();
	}

	// Token: 0x06001EE3 RID: 7907
	private bool CanEditClassEditorField(FieldInfo field)
	{
		return !(field == null) && !field.IsInitOnly && !field.IsLiteral && (field.IsPublic || this.classEditorEditPrivate) && this.IsClassEditorEditableType(field.FieldType);
	}

	// Token: 0x06001EE4 RID: 7908
	private bool IsClassEditorEditableType(Type type)
	{
		return type == typeof(string) || type == typeof(bool) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(LayerMask) || type.IsEnum;
	}

	// Token: 0x06001EE5 RID: 7909
	private void SetClassEditorFieldValue(object target, FieldInfo field, object value, string inputKey)
	{
		try
		{
			PropertyInfo property = target.GetType().GetProperty("Network" + field.Name, BindingFlags.Instance | BindingFlags.Public);
			if (property != null && property.CanWrite && property.PropertyType == field.FieldType)
			{
				property.SetValue(target, value, null);
			}
			else
			{
				field.SetValue(target, value);
			}
			this.classEditorInputs[inputKey] = this.SerializeClassEditorValue(value, field.FieldType);
			PlayerSettings playerSettings = target as PlayerSettings;
			if (playerSettings != null)
			{
				playerSettings.NotifyChanged();
			}
			PlayerProfile playerProfile = target as PlayerProfile;
			if (playerProfile != null && field.Name == "playerName")
			{
				this.TryInvokeClassEditorMethod(playerProfile, "SetPlayerNameTag", Array.Empty<object>());
			}
			this.classEditorStatus = string.Concat(new string[]
			{
				"Set ",
				target.GetType().Name,
				".",
				field.Name,
				" = ",
				this.SerializeClassEditorValue(value, field.FieldType)
			});
		}
		catch (Exception ex)
		{
			this.classEditorStatus = string.Concat(new string[]
			{
				"Failed to set ",
				field.Name,
				": ",
				ex.GetType().Name,
				" - ",
				ex.Message
			});
		}
	}

	// Token: 0x06001EE6 RID: 7910
	private bool TryParseClassEditorValue(Type type, string input, out object value)
	{
		value = null;
		if (type == typeof(string))
		{
			value = input;
			return true;
		}
		if (type == typeof(bool))
		{
			bool flag;
			if (bool.TryParse(input, out flag))
			{
				value = flag;
				return true;
			}
			if (input == "1")
			{
				value = true;
				return true;
			}
			if (input == "0")
			{
				value = false;
				return true;
			}
			return false;
		}
		else if (type == typeof(int))
		{
			int num;
			if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
			{
				value = num;
				return true;
			}
			return false;
		}
		else if (type == typeof(uint))
		{
			uint num2;
			if (uint.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
			{
				value = num2;
				return true;
			}
			return false;
		}
		else if (type == typeof(long))
		{
			long num3;
			if (long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out num3))
			{
				value = num3;
				return true;
			}
			return false;
		}
		else if (type == typeof(ulong))
		{
			ulong num4;
			if (ulong.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out num4))
			{
				value = num4;
				return true;
			}
			return false;
		}
		else if (type == typeof(float))
		{
			float num5;
			if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out num5))
			{
				value = num5;
				return true;
			}
			return false;
		}
		else if (type == typeof(double))
		{
			double num6;
			if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out num6))
			{
				value = num6;
				return true;
			}
			return false;
		}
		else if (type == typeof(Vector2))
		{
			string[] array = input.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			float num7;
			float num8;
			if (array.Length == 2 && float.TryParse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture, out num7) && float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out num8))
			{
				value = new Vector2(num7, num8);
				return true;
			}
			return false;
		}
		else if (type == typeof(Vector3))
		{
			string[] array2 = input.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			float num9;
			float num10;
			float num11;
			if (array2.Length == 3 && float.TryParse(array2[0], NumberStyles.Float, CultureInfo.InvariantCulture, out num9) && float.TryParse(array2[1], NumberStyles.Float, CultureInfo.InvariantCulture, out num10) && float.TryParse(array2[2], NumberStyles.Float, CultureInfo.InvariantCulture, out num11))
			{
				value = new Vector3(num9, num10, num11);
				return true;
			}
			return false;
		}
		else
		{
			if (!(type == typeof(LayerMask)))
			{
				if (type.IsEnum)
				{
					try
					{
						value = Enum.Parse(type, input, true);
						return true;
					}
					catch
					{
						return false;
					}
					return false;
				}
				return false;
			}
			int num12;
			if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out num12))
			{
				value = new LayerMask
				{
					value = num12
				};
				return true;
			}
			return false;
		}
	}

	// Token: 0x06001EE7 RID: 7911
	private string SerializeClassEditorValue(object value, Type type)
	{
		if (value == null)
		{
			return "";
		}
		if (type == typeof(float))
		{
			return ((float)value).ToString("0.###", CultureInfo.InvariantCulture);
		}
		if (type == typeof(double))
		{
			return ((double)value).ToString("0.###", CultureInfo.InvariantCulture);
		}
		if (type == typeof(Vector2))
		{
			Vector2 vector = (Vector2)value;
			return vector.x.ToString("0.###", CultureInfo.InvariantCulture) + "," + vector.y.ToString("0.###", CultureInfo.InvariantCulture);
		}
		if (type == typeof(Vector3))
		{
			Vector3 vector2 = (Vector3)value;
			return string.Concat(new string[]
			{
				vector2.x.ToString("0.###", CultureInfo.InvariantCulture),
				",",
				vector2.y.ToString("0.###", CultureInfo.InvariantCulture),
				",",
				vector2.z.ToString("0.###", CultureInfo.InvariantCulture)
			});
		}
		if (type == typeof(LayerMask))
		{
			return ((LayerMask)value).value.ToString(CultureInfo.InvariantCulture);
		}
		IFormattable formattable = value as IFormattable;
		if (formattable != null)
		{
			return formattable.ToString(null, CultureInfo.InvariantCulture);
		}
		return value.ToString();
	}

	// Token: 0x06001EE8 RID: 7912
	private string FormatClassEditorValue(object value)
	{
		if (value == null)
		{
			return "null";
		}
		global::UnityEngine.Object @object = value as global::UnityEngine.Object;
		if (@object != null)
		{
			return @object.name + " (" + value.GetType().Name + ")";
		}
		if (value is Vector2)
		{
			return this.SerializeClassEditorValue(value, typeof(Vector2));
		}
		if (value is Vector3)
		{
			return this.SerializeClassEditorValue(value, typeof(Vector3));
		}
		if (value is LayerMask)
		{
			return "LayerMask " + ((LayerMask)value).value.ToString(CultureInfo.InvariantCulture);
		}
		return value.ToString();
	}

	// Token: 0x06001EE9 RID: 7913
	private string MakeClassEditorKey(object target, string suffix)
	{
		return target.GetHashCode().ToString(CultureInfo.InvariantCulture) + ":" + suffix;
	}

	// Token: 0x06001EEA RID: 7914
	private bool IsSyncVarField(FieldInfo field)
	{
		object[] customAttributes = field.GetCustomAttributes(false);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			if (customAttributes[i] != null && customAttributes[i].GetType().Name.IndexOf("SyncVar", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001EEB RID: 7915
	private void TryInvokeClassEditorMethod(object target, string methodName, object[] args)
	{
		try
		{
			MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				this.classEditorStatus = "Method not found: " + methodName;
			}
			else
			{
				method.Invoke(target, args);
				this.classEditorStatus = "Called " + target.GetType().Name + "." + methodName;
			}
		}
		catch (Exception ex)
		{
			this.classEditorStatus = string.Concat(new string[]
			{
				"Failed to call ",
				methodName,
				": ",
				ex.GetType().Name,
				" - ",
				ex.Message
			});
		}
	}

	// Token: 0x06001F1E RID: 7966
	private void DrawOrgansTab()
	{
		GameObject gameObject = this.FindLocalPlayerObject();
		if (gameObject == null)
		{
			GUILayout.Label("Local player: not found", Array.Empty<GUILayoutOption>());
			return;
		}
		this.playerOrgans = gameObject.GetComponent<PlayerOrgans>();
		if (this.playerOrgans == null)
		{
			GUILayout.Label("PlayerOrgans: not found on local player.", Array.Empty<GUILayoutOption>());
			return;
		}
		GUILayout.Label("=== Organ Editor ===", Array.Empty<GUILayoutOption>());
		GUIStyle guistyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		GUILayout.Label("Edit the local player's organ flags. Replicated apply uses PlayerOrgans.ServerSetBodyParts(PlayerOrganData), so it only works when this instance is server/host. Local apply calls the same user-code paths as the organ RPC handlers and is only a local visual/debug change.", guistyle, Array.Empty<GUILayoutOption>());
		GUILayout.Space(6f);
		this.DrawOrgansQuickEditor(this.playerOrgans);
		GUILayout.Space(8f);
		GUILayout.Label("Raw PlayerOrgans fields", Array.Empty<GUILayoutOption>());
		bool flag = this.classEditorShowPrivate;
		bool flag2 = this.classEditorEditPrivate;
		this.classEditorShowPrivate = true;
		this.classEditorEditPrivate = false;
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		FieldInfo[] fields = typeof(PlayerOrgans).GetFields(bindingFlags);
		for (int i = 0; i < fields.Length; i++)
		{
			this.DrawClassEditorField(this.playerOrgans, fields[i]);
		}
		this.classEditorShowPrivate = flag;
		this.classEditorEditPrivate = flag2;
	}

	// Token: 0x06001F1F RID: 7967
	private void DrawOrgansQuickEditor(PlayerOrgans organs)
	{
		if (organs == null)
		{
			GUILayout.Label("PlayerOrgans: not found.", Array.Empty<GUILayoutOption>());
			return;
		}
		if (!this.organEditorInitialized || this.organEditorTarget != organs)
		{
			this.RefreshOrganEditorState(organs);
		}
		GUILayout.Label("Organ quick editor", Array.Empty<GUILayoutOption>());
		GUILayout.Label(string.Concat(new string[]
		{
			"netId: ",
			organs.netId.ToString(),
			" | local: ",
			organs.isLocalPlayer.ToString(),
			" | server: ",
			organs.isServer.ToString(),
			" | client: ",
			organs.isClient.ToString(),
			" | owned: ",
			organs.isOwned.ToString()
		}), Array.Empty<GUILayoutOption>());
		GUILayout.Space(4f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		this.organLeftEye = GUILayout.Toggle(this.organLeftEye, "Left Eye", new GUILayoutOption[] { GUILayout.Width(120f) });
		this.organRightEye = GUILayout.Toggle(this.organRightEye, "Right Eye", new GUILayoutOption[] { GUILayout.Width(120f) });
		this.organBody = GUILayout.Toggle(this.organBody, "Body", new GUILayoutOption[] { GUILayout.Width(120f) });
		this.organMouth = GUILayout.Toggle(this.organMouth, "Mouth", new GUILayoutOption[] { GUILayout.Width(120f) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Load Current", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.RefreshOrganEditorState(organs);
			this.organEditorStatus = "Loaded current local organ state.";
		}
		if (GUILayout.Button("All On", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.SetOrganEditorDraft(true, true, true, true);
		}
		if (GUILayout.Button("All Off", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.SetOrganEditorDraft(false, false, false, false);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("No Eyes", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.SetOrganEditorDraft(false, false, this.organBody, this.organMouth);
		}
		if (GUILayout.Button("Left Eye Only", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.SetOrganEditorDraft(true, false, this.organBody, this.organMouth);
		}
		if (GUILayout.Button("Right Eye Only", new GUILayoutOption[] { GUILayout.Height(24f) }))
		{
			this.SetOrganEditorDraft(false, true, this.organBody, this.organMouth);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		bool enabled = GUI.enabled;
		GUI.enabled = enabled && NetworkServer.active && organs.isServer;
		if (GUILayout.Button("Apply Replicated", new GUILayoutOption[] { GUILayout.Height(28f) }))
		{
			this.ApplyOrganEditorServer(organs);
		}
		GUI.enabled = enabled;
		if (GUILayout.Button("Apply Local Only", new GUILayoutOption[] { GUILayout.Height(28f) }))
		{
			this.ApplyOrganEditorLocal(organs);
		}
		GUILayout.EndHorizontal();
		if (!NetworkServer.active || !organs.isServer)
		{
			GUILayout.Label("Replicated apply is disabled unless this player is running on the server/host. Use Local Only for visual/client-side debugging.", new GUIStyle(GUI.skin.label)
			{
				wordWrap = true
			}, Array.Empty<GUILayoutOption>());
		}
		GUILayout.Space(4f);
		GUILayout.Label(string.Concat(new string[]
		{
			"Current local state: L eye ",
			this.GetOrganBool(organs, "_localLeftEye", false).ToString(),
			" | R eye ",
			this.GetOrganBool(organs, "_localRightEye", false).ToString(),
			" | body ",
			this.GetOrganBool(organs, "_localBody", false).ToString(),
			" | mouth ",
			this.GetOrganBool(organs, "_localMouth", false).ToString()
		}), Array.Empty<GUILayoutOption>());
		if (!string.IsNullOrEmpty(this.organEditorStatus))
		{
			GUILayout.Label(this.organEditorStatus, new GUIStyle(GUI.skin.label)
			{
				wordWrap = true
			}, Array.Empty<GUILayoutOption>());
		}
	}

	// Token: 0x06001F20 RID: 7968
	private void RefreshOrganEditorState(PlayerOrgans organs)
	{
		if (organs == null)
		{
			return;
		}
		this.organEditorTarget = organs;
		this.organLeftEye = this.GetOrganBool(organs, "_localLeftEye", true);
		this.organRightEye = this.GetOrganBool(organs, "_localRightEye", true);
		this.organBody = this.GetOrganBool(organs, "_localBody", true);
		this.organMouth = this.GetOrganBool(organs, "_localMouth", true);
		this.organEditorInitialized = true;
	}

	// Token: 0x06001F21 RID: 7969
	private void SetOrganEditorDraft(bool leftEye, bool rightEye, bool body, bool mouth)
	{
		this.organLeftEye = leftEye;
		this.organRightEye = rightEye;
		this.organBody = body;
		this.organMouth = mouth;
	}

	// Token: 0x06001F22 RID: 7970
	private bool GetOrganBool(PlayerOrgans organs, string fieldName, bool fallback)
	{
		try
		{
			FieldInfo field = typeof(PlayerOrgans).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null && field.FieldType == typeof(bool))
			{
				return (bool)field.GetValue(organs);
			}
		}
		catch
		{
		}
		return fallback;
	}

	// Token: 0x06001F23 RID: 7971
	private void ApplyOrganEditorServer(PlayerOrgans organs)
	{
		if (organs == null)
		{
			return;
		}
		if (!NetworkServer.active || !organs.isServer)
		{
			this.organEditorStatus = "Server apply skipped: this instance is not server/host for PlayerOrgans.";
			return;
		}
		try
		{
			PlayerOrganData playerOrganData = new PlayerOrganData
			{
				organsReference = organs,
				leftEye = this.organLeftEye,
				rightEye = this.organRightEye,
				body = this.organBody,
				mouth = this.organMouth
			};
			organs.ServerSetBodyParts(playerOrganData);
			this.organEditorStatus = "Applied organs through ServerSetBodyParts.";
		}
		catch (Exception ex)
		{
			this.organEditorStatus = "Server organ apply failed: " + ex.GetType().Name + " - " + ex.Message;
		}
	}

	// Token: 0x06001F24 RID: 7972
	private void ApplyOrganEditorLocal(PlayerOrgans organs)
	{
		if (organs == null)
		{
			return;
		}
		try
		{
			this.InvokeOrganUserCode(organs, "UserCode_RpcSetEyes__Boolean__Boolean", new object[] { this.organLeftEye, this.organRightEye });
			this.InvokeOrganUserCode(organs, "UserCode_RpcSetBody__Boolean", new object[] { this.organBody });
			this.InvokeOrganUserCode(organs, "UserCode_RpcSetMouth__Boolean", new object[] { this.organMouth });
			this.organEditorStatus = "Applied organ state locally only.";
		}
		catch (Exception ex)
		{
			this.organEditorStatus = "Local organ apply failed: " + ex.GetType().Name + " - " + ex.Message;
		}
	}

	// Token: 0x06001F25 RID: 7973
	private void InvokeOrganUserCode(PlayerOrgans organs, string methodName, object[] args)
	{
		MethodInfo method = typeof(PlayerOrgans).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null)
		{
			throw new MissingMethodException(typeof(PlayerOrgans).FullName, methodName);
		}
		method.Invoke(organs, args);
	}

	// Token: 0x06001F60 RID: 8032
	private void Update()
	{
		this.UpdateMouseReveal();
	}

	// Token: 0x06001F61 RID: 8033
	private void OnDisable()
	{
		this.RestoreMouseRevealState();
	}

	// Token: 0x06001F62 RID: 8034
	private void UpdateMouseReveal()
	{
		if (Input.GetKey(this.mouseRevealKey))
		{
			if (!this.mouseRevealActive)
			{
				this.mouseRevealPreviousVisible = Cursor.visible;
				this.mouseRevealPreviousLockState = Cursor.lockState;
				this.mouseRevealActive = true;
			}
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			return;
		}
		this.RestoreMouseRevealState();
	}

	// Token: 0x06001F63 RID: 8035
	private void RestoreMouseRevealState()
	{
		if (!this.mouseRevealActive)
		{
			return;
		}
		Cursor.visible = this.mouseRevealPreviousVisible;
		Cursor.lockState = this.mouseRevealPreviousLockState;
		this.mouseRevealActive = false;
	}

	// Token: 0x06002006 RID: 8198
	private void DrawTextFeedbackTab()
	{
		GUILayout.Label("=== Text Feedback ===", Array.Empty<GUILayoutOption>());
		GUIStyle guistyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		GUILayout.Label("Shows arbitrary text using the money-change feedback prefab when it can be found. This does not change balance; it only reuses the UI style and writes custom text into the feedback text fields.", guistyle, Array.Empty<GUILayoutOption>());
		GUILayout.Space(6f);
		if (string.IsNullOrEmpty(this.textFeedbackHeaderInput) && this.playerProfile != null && !string.IsNullOrEmpty(this.playerProfile.playerName))
		{
			this.textFeedbackHeaderInput = this.playerProfile.playerName;
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Header", new GUILayoutOption[] { GUILayout.Width(90f) });
		this.textFeedbackHeaderInput = GUILayout.TextField(this.textFeedbackHeaderInput, new GUILayoutOption[] { GUILayout.Width(260f) });
		if (GUILayout.Button("Use Player", new GUILayoutOption[] { GUILayout.Width(90f) }))
		{
			this.textFeedbackHeaderInput = ((this.playerProfile != null && !string.IsNullOrEmpty(this.playerProfile.playerName)) ? this.playerProfile.playerName : "Player");
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Message", Array.Empty<GUILayoutOption>());
		this.textFeedbackMessageInput = GUILayout.TextArea(this.textFeedbackMessageInput, new GUILayoutOption[] { GUILayout.MinHeight(70f) });
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Duration", new GUILayoutOption[] { GUILayout.Width(90f) });
		this.textFeedbackDurationInput = GUILayout.TextField(this.textFeedbackDurationInput, new GUILayoutOption[] { GUILayout.Width(80f) });
		GUILayout.Label("seconds", new GUILayoutOption[] { GUILayout.Width(80f) });
		this.textFeedbackUseMoneyPrefab = GUILayout.Toggle(this.textFeedbackUseMoneyPrefab, "Use money prefab if available", Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.Label("Color", Array.Empty<GUILayoutOption>());
		this.textFeedbackColorIndex = GUILayout.SelectionGrid(this.textFeedbackColorIndex, DebugWindow.TextFeedbackColorNames, 3, Array.Empty<GUILayoutOption>());
		GUILayout.Space(8f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Show Text", new GUILayoutOption[] { GUILayout.Height(30f) }))
		{
			this.ShowDebugFeedbackText(this.textFeedbackHeaderInput, this.textFeedbackMessageInput);
		}
		if (GUILayout.Button("Sample +", new GUILayoutOption[] { GUILayout.Height(30f) }))
		{
			this.textFeedbackMessageInput = "Item collected";
			this.textFeedbackColorIndex = 1;
			this.ShowDebugFeedbackText(this.textFeedbackHeaderInput, this.textFeedbackMessageInput);
		}
		if (GUILayout.Button("Sample Warning", new GUILayoutOption[] { GUILayout.Height(30f) }))
		{
			this.textFeedbackMessageInput = "Inventory full";
			this.textFeedbackColorIndex = 3;
			this.ShowDebugFeedbackText(this.textFeedbackHeaderInput, this.textFeedbackMessageInput);
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(6f);
		GUILayout.Label("Preview:", Array.Empty<GUILayoutOption>());
		GUIStyle guistyle2 = new GUIStyle(GUI.skin.box)
		{
			wordWrap = true,
			alignment = TextAnchor.MiddleCenter
		};
		GUILayout.Label((string.IsNullOrEmpty(this.textFeedbackHeaderInput) ? "Debug" : this.textFeedbackHeaderInput) + "\n" + this.textFeedbackMessageInput, guistyle2, new GUILayoutOption[] { GUILayout.MinHeight(60f) });
		if (!string.IsNullOrEmpty(this.textFeedbackStatus))
		{
			GUILayout.Label(this.textFeedbackStatus, guistyle, Array.Empty<GUILayoutOption>());
		}
	}

	// Token: 0x06002007 RID: 8199
	private void ShowDebugFeedbackText(string header, string body)
	{
		float num;
		if (!float.TryParse(this.textFeedbackDurationInput, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
		{
			num = 3f;
			this.textFeedbackDurationInput = num.ToString("0.###", CultureInfo.InvariantCulture);
		}
		num = Mathf.Clamp(num, 0.25f, 30f);
		if (string.IsNullOrEmpty(header))
		{
			header = "Debug";
		}
		if (string.IsNullOrEmpty(body))
		{
			body = " ";
		}
		Color textFeedbackColor = this.GetTextFeedbackColor();
		if (this.textFeedbackUseMoneyPrefab && this.TrySpawnMoneyPrefabText(header, body, textFeedbackColor, num))
		{
			this.textFeedbackStatus = "Spawned text using MoneyChangeText prefab.";
			return;
		}
		this.SpawnFallbackDebugText(header, body, textFeedbackColor, num);
		this.textFeedbackStatus = (this.textFeedbackUseMoneyPrefab ? "Money feedback prefab was not found, so fallback UI text was spawned." : "Spawned fallback UI text.");
	}

	// Token: 0x06002008 RID: 8200
	private bool TrySpawnMoneyPrefabText(string header, string body, Color color, float duration)
	{
		bool flag;
		try
		{
			MoneyDisplayAndFeedbacks[] array = global::UnityEngine.Object.FindObjectsOfType<MoneyDisplayAndFeedbacks>();
			if (array == null || array.Length == 0)
			{
				flag = false;
			}
			else
			{
				MoneyDisplayAndFeedbacks moneyDisplayAndFeedbacks = array[0];
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				FieldInfo field = typeof(MoneyDisplayAndFeedbacks).GetField("moneyChangePrefab", bindingFlags);
				FieldInfo field2 = typeof(MoneyDisplayAndFeedbacks).GetField("feedbackParent", bindingFlags);
				if (field == null || field2 == null)
				{
					flag = false;
				}
				else
				{
					MoneyChangeText moneyChangeText = field.GetValue(moneyDisplayAndFeedbacks) as MoneyChangeText;
					Transform transform = field2.GetValue(moneyDisplayAndFeedbacks) as Transform;
					if (moneyChangeText == null || transform == null)
					{
						flag = false;
					}
					else
					{
						MoneyChangeText moneyChangeText2 = global::UnityEngine.Object.Instantiate<MoneyChangeText>(moneyChangeText, transform);
						if (moneyChangeText2.playerNameText != null)
						{
							moneyChangeText2.playerNameText.text = header;
							moneyChangeText2.playerNameText.color = Color.white;
						}
						if (moneyChangeText2.changeAmountText != null)
						{
							moneyChangeText2.changeAmountText.text = body;
							moneyChangeText2.changeAmountText.color = color;
						}
						if (moneyChangeText2.playerColorImage != null)
						{
							moneyChangeText2.playerColorImage.color = color;
						}
						Image component = moneyChangeText2.GetComponent<Image>();
						if (component != null)
						{
							component.color = color;
						}
						global::UnityEngine.Object.Destroy(moneyChangeText2.gameObject, duration);
						flag = true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			this.textFeedbackStatus = "Money prefab text spawn failed: " + ex.GetType().Name + " - " + ex.Message;
			flag = false;
		}
		return flag;
	}

	// Token: 0x06002009 RID: 8201
	private void SpawnFallbackDebugText(string header, string body, Color color, float duration)
	{
		Canvas canvas = this.GetOrCreateTextFeedbackCanvas();
		GameObject gameObject = new GameObject("Debug Text Feedback");
		gameObject.transform.SetParent(canvas.transform, false);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2(0.5f, 1f);
		rectTransform.anchorMax = new Vector2(0.5f, 1f);
		rectTransform.pivot = new Vector2(0.5f, 1f);
		rectTransform.sizeDelta = new Vector2(560f, 120f);
		float y = -80f - (float)(this.textFeedbackSpawnCount % 5) * 70f;
		this.textFeedbackSpawnCount++;
		rectTransform.anchoredPosition = new Vector2(0f, y);
		Text text = gameObject.AddComponent<Text>();
		text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		text.fontSize = 28;
		text.alignment = TextAnchor.MiddleCenter;
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		text.color = color;
		text.text = header + "\n" + body;
		gameObject.AddComponent<Shadow>().effectDistance = new Vector2(2f, -2f);
		global::UnityEngine.Object.Destroy(gameObject, duration);
	}

	// Token: 0x0600200A RID: 8202
	private Canvas GetOrCreateTextFeedbackCanvas()
	{
		if (this.textFeedbackCanvas != null)
		{
			return this.textFeedbackCanvas;
		}
		GameObject gameObject = new GameObject("Debug Text Feedback Canvas");
		global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
		this.textFeedbackCanvas = gameObject.AddComponent<Canvas>();
		this.textFeedbackCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		this.textFeedbackCanvas.sortingOrder = 32760;
		gameObject.AddComponent<CanvasScaler>();
		gameObject.AddComponent<GraphicRaycaster>();
		return this.textFeedbackCanvas;
	}

	// Token: 0x0600200B RID: 8203
	private Color GetTextFeedbackColor()
	{
		switch (this.textFeedbackColorIndex)
		{
		case 1:
			return Color.green;
		case 2:
			return Color.red;
		case 3:
			return Color.yellow;
		case 4:
			return Color.cyan;
		case 5:
			return Color.magenta;
		default:
			return Color.white;
		}
	}

	// Token: 0x0600203D RID: 8253
	private void DrawImageFeedbackTab()
	{
		GUILayout.Label("=== Image Feedback ===", Array.Empty<GUILayoutOption>());
		GUIStyle guistyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		GUILayout.Label("Shows a temporary image overlay separately from the money/text feedback. It can use the local player's Steam/profile picture, a Texture2D or Sprite from Resources, a direct PNG/JPG file path, or a generated placeholder texture.", guistyle, Array.Empty<GUILayoutOption>());
		GUILayout.Space(6f);
		GUILayout.Label("Source", Array.Empty<GUILayoutOption>());
		this.imageFeedbackSourceIndex = GUILayout.SelectionGrid(this.imageFeedbackSourceIndex, DebugWindow.ImageFeedbackSourceNames, 2, Array.Empty<GUILayoutOption>());
		GUILayout.Space(4f);
		if (this.imageFeedbackSourceIndex == 1 || this.imageFeedbackSourceIndex == 2)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Resources path", new GUILayoutOption[] { GUILayout.Width(110f) });
			this.imageFeedbackResourcePath = GUILayout.TextField(this.imageFeedbackResourcePath, new GUILayoutOption[] { GUILayout.Width(300f) });
			GUILayout.EndHorizontal();
			GUILayout.Label("Example: Icons/MyIcon, without file extension. The asset must be under a Resources folder.", guistyle, Array.Empty<GUILayoutOption>());
		}
		if (this.imageFeedbackSourceIndex == 3)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Image file path", new GUILayoutOption[] { GUILayout.Width(110f) });
			this.imageFeedbackFilePath = GUILayout.TextField(this.imageFeedbackFilePath, new GUILayoutOption[] { GUILayout.Width(440f) });
			GUILayout.EndHorizontal();
			GUILayout.Label("Supports direct .png, .jpg, and .jpeg paths. Relative paths are checked against Application.dataPath, Application.persistentDataPath, and the current working directory.", guistyle, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			Texture2D texture2Dt;
			string text;
			if (GUILayout.Button("Load File Preview", new GUILayoutOption[] { GUILayout.Height(24f) }) && this.TryLoadImageFeedbackFileTexture(out texture2Dt, out text, true))
			{
				this.imageFeedbackStatus = "Loaded image file: " + text;
			}
			if (GUILayout.Button("Clear File Cache", new GUILayoutOption[] { GUILayout.Height(24f) }))
			{
				this.ClearImageFeedbackFileCache();
				this.imageFeedbackStatus = "Cleared loaded file texture cache.";
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Caption", new GUILayoutOption[] { GUILayout.Width(110f) });
		this.imageFeedbackCaption = GUILayout.TextField(this.imageFeedbackCaption, new GUILayoutOption[] { GUILayout.Width(300f) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Duration", new GUILayoutOption[] { GUILayout.Width(110f) });
		this.imageFeedbackDurationInput = GUILayout.TextField(this.imageFeedbackDurationInput, new GUILayoutOption[] { GUILayout.Width(70f) });
		GUILayout.Label("seconds", new GUILayoutOption[] { GUILayout.Width(70f) });
		this.imageFeedbackAddBackground = GUILayout.Toggle(this.imageFeedbackAddBackground, "Background panel", Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		this.imageFeedbackFullscreen = GUILayout.Toggle(this.imageFeedbackFullscreen, "Fullscreen", new GUILayoutOption[] { GUILayout.Width(140f) });
		GUILayout.Label(this.imageFeedbackFullscreen ? "Image will fit to the screen and ignore manual size/position." : "Use manual size and position controls.", guistyle, Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUI.enabled = !this.imageFeedbackFullscreen;
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Size", new GUILayoutOption[] { GUILayout.Width(110f) });
		this.imageFeedbackWidthInput = GUILayout.TextField(this.imageFeedbackWidthInput, new GUILayoutOption[] { GUILayout.Width(70f) });
		GUILayout.Label("x", new GUILayoutOption[] { GUILayout.Width(20f) });
		this.imageFeedbackHeightInput = GUILayout.TextField(this.imageFeedbackHeightInput, new GUILayoutOption[] { GUILayout.Width(70f) });
		GUILayout.Label("pixels", new GUILayoutOption[] { GUILayout.Width(70f) });
		GUILayout.EndHorizontal();
		GUILayout.Label("Position", Array.Empty<GUILayoutOption>());
		this.imageFeedbackPositionIndex = GUILayout.SelectionGrid(this.imageFeedbackPositionIndex, DebugWindow.ImageFeedbackPositionNames, 3, Array.Empty<GUILayoutOption>());
		GUI.enabled = true;
		GUILayout.Space(8f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Show Image", new GUILayoutOption[] { GUILayout.Height(30f) }))
		{
			this.ShowDebugFeedbackImage();
		}
		if (GUILayout.Button("Profile Picture", new GUILayoutOption[] { GUILayout.Height(30f) }))
		{
			this.imageFeedbackSourceIndex = 0;
			this.ShowDebugFeedbackImage();
		}
		if (GUILayout.Button("File Path", new GUILayoutOption[] { GUILayout.Height(30f) }))
		{
			this.imageFeedbackSourceIndex = 3;
			this.ShowDebugFeedbackImage();
		}
		if (GUILayout.Button("Placeholder", new GUILayoutOption[] { GUILayout.Height(30f) }))
		{
			this.imageFeedbackSourceIndex = 4;
			this.ShowDebugFeedbackImage();
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(6f);
		GUILayout.Label("Preview source: " + DebugWindow.ImageFeedbackSourceNames[Mathf.Clamp(this.imageFeedbackSourceIndex, 0, DebugWindow.ImageFeedbackSourceNames.Length - 1)], Array.Empty<GUILayoutOption>());
		Texture2D texture2D = this.TryResolveImageFeedbackPreviewTexture();
		if (texture2D != null)
		{
			GUILayout.Label(texture2D, new GUILayoutOption[]
			{
				GUILayout.Width(96f),
				GUILayout.Height(96f)
			});
		}
		else
		{
			GUILayout.Label("No preview texture available for the selected source.", guistyle, Array.Empty<GUILayoutOption>());
		}
		if (!string.IsNullOrEmpty(this.imageFeedbackStatus))
		{
			GUILayout.Label(this.imageFeedbackStatus, guistyle, Array.Empty<GUILayoutOption>());
		}
	}

	// Token: 0x0600203E RID: 8254
	private void ShowDebugFeedbackImage()
	{
		float duration;
		if (!float.TryParse(this.imageFeedbackDurationInput, NumberStyles.Float, CultureInfo.InvariantCulture, out duration))
		{
			duration = 3f;
			this.imageFeedbackDurationInput = duration.ToString("0.###", CultureInfo.InvariantCulture);
		}
		duration = Mathf.Clamp(duration, 0.25f, 60f);
		float width = 220f;
		float height = 220f;
		if (!this.imageFeedbackFullscreen)
		{
			if (!float.TryParse(this.imageFeedbackWidthInput, NumberStyles.Float, CultureInfo.InvariantCulture, out width))
			{
				width = 220f;
				this.imageFeedbackWidthInput = width.ToString("0.###", CultureInfo.InvariantCulture);
			}
			if (!float.TryParse(this.imageFeedbackHeightInput, NumberStyles.Float, CultureInfo.InvariantCulture, out height))
			{
				height = 220f;
				this.imageFeedbackHeightInput = height.ToString("0.###", CultureInfo.InvariantCulture);
			}
			width = Mathf.Clamp(width, 24f, 900f);
			height = Mathf.Clamp(height, 24f, 900f);
		}
		Texture2D texture;
		Sprite sprite;
		string sourceName;
		if (!this.TryResolveImageFeedbackSource(out texture, out sprite, out sourceName))
		{
			this.imageFeedbackStatus = "Could not find image source. For Resources, use a no-extension Resources path. For File Path, use a readable .png, .jpg, or .jpeg file.";
			return;
		}
		this.SpawnFallbackDebugImage(texture, sprite, sourceName, this.imageFeedbackCaption, width, height, duration, this.imageFeedbackFullscreen);
		this.imageFeedbackStatus = "Spawned image from " + sourceName + ".";
	}

	// Token: 0x0600203F RID: 8255
	private Texture2D TryResolveImageFeedbackPreviewTexture()
	{
		Texture2D texture2D;
		Sprite sprite;
		string text;
		if (!this.TryResolveImageFeedbackSource(out texture2D, out sprite, out text))
		{
			return null;
		}
		if (texture2D != null)
		{
			return texture2D;
		}
		if (sprite != null)
		{
			return sprite.texture;
		}
		return null;
	}

	// Token: 0x06002040 RID: 8256
	private bool TryResolveImageFeedbackSource(out Texture2D texture, out Sprite sprite, out string sourceName)
	{
		texture = null;
		sprite = null;
		sourceName = "none";
		switch (this.imageFeedbackSourceIndex)
		{
		case 0:
			if (this.playerProfile != null && this.playerProfile.steamProfilePicture != null)
			{
				texture = this.playerProfile.steamProfilePicture;
				sourceName = "profile picture";
				return true;
			}
			return false;
		case 1:
			if (!string.IsNullOrEmpty(this.imageFeedbackResourcePath))
			{
				texture = Resources.Load<Texture2D>(this.imageFeedbackResourcePath);
				if (texture != null)
				{
					sourceName = "Resources texture: " + this.imageFeedbackResourcePath;
					return true;
				}
			}
			return false;
		case 2:
			if (!string.IsNullOrEmpty(this.imageFeedbackResourcePath))
			{
				sprite = Resources.Load<Sprite>(this.imageFeedbackResourcePath);
				if (sprite != null)
				{
					sourceName = "Resources sprite: " + this.imageFeedbackResourcePath;
					return true;
				}
			}
			return false;
		case 3:
			return this.TryLoadImageFeedbackFileTexture(out texture, out sourceName, false);
		case 4:
			texture = this.GetOrCreateImageFeedbackPlaceholderTexture();
			sourceName = "generated placeholder";
			return texture != null;
		default:
			return false;
		}
	}

	// Token: 0x06002042 RID: 8258
	private void GetImageFeedbackAnchorAndOffset(out Vector2 anchor, out Vector2 offset)
	{
		switch (this.imageFeedbackPositionIndex)
		{
		case 1:
			anchor = new Vector2(0.5f, 1f);
			offset = new Vector2(0f, -80f);
			return;
		case 2:
			anchor = new Vector2(1f, 1f);
			offset = new Vector2(-80f, -80f);
			return;
		case 3:
			anchor = new Vector2(0f, 0.5f);
			offset = new Vector2(80f, 0f);
			return;
		case 4:
			anchor = new Vector2(0.5f, 0.5f);
			offset = Vector2.zero;
			return;
		case 5:
			anchor = new Vector2(1f, 0.5f);
			offset = new Vector2(-80f, 0f);
			return;
		case 6:
			anchor = new Vector2(0f, 0f);
			offset = new Vector2(80f, 80f);
			return;
		case 7:
			anchor = new Vector2(0.5f, 0f);
			offset = new Vector2(0f, 80f);
			return;
		case 8:
			anchor = new Vector2(1f, 0f);
			offset = new Vector2(-80f, 80f);
			return;
		default:
			anchor = new Vector2(0f, 1f);
			offset = new Vector2(80f, -80f);
			return;
		}
	}

	// Token: 0x06002043 RID: 8259
	private Texture2D GetOrCreateImageFeedbackPlaceholderTexture()
	{
		if (this.imageFeedbackPlaceholderTexture != null)
		{
			return this.imageFeedbackPlaceholderTexture;
		}
		Texture2D texture2D = new Texture2D(64, 64, TextureFormat.RGBA32, false);
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				bool flag = (i / 8 + j / 8) % 2 == 0;
				texture2D.SetPixel(j, i, flag ? new Color(0.85f, 0.85f, 0.85f, 1f) : new Color(0.2f, 0.2f, 0.2f, 1f));
			}
		}
		texture2D.Apply();
		this.imageFeedbackPlaceholderTexture = texture2D;
		return this.imageFeedbackPlaceholderTexture;
	}

	// Token: 0x06002079 RID: 8313
	private bool TryLoadImageFeedbackFileTexture(out Texture2D texture, out string sourceName, bool forceReload)
	{
		texture = null;
		sourceName = "file path";
		string fullPath;
		if (!this.TryResolveImageFeedbackFilePath(out fullPath))
		{
			return false;
		}
		string extension = Path.GetExtension(fullPath);
		if (string.IsNullOrEmpty(extension) || (string.Compare(extension, ".png", true, CultureInfo.InvariantCulture) != 0 && string.Compare(extension, ".jpg", true, CultureInfo.InvariantCulture) != 0 && string.Compare(extension, ".jpeg", true, CultureInfo.InvariantCulture) != 0))
		{
			this.imageFeedbackStatus = "File source must be a .png, .jpg, or .jpeg file.";
			return false;
		}
		bool flag;
		try
		{
			long ticks = File.GetLastWriteTimeUtc(fullPath).Ticks;
			if (!forceReload && this.imageFeedbackLoadedFileTexture != null && string.Equals(this.imageFeedbackLoadedFilePath, fullPath, StringComparison.OrdinalIgnoreCase) && this.imageFeedbackLoadedFileWriteTicks == ticks)
			{
				texture = this.imageFeedbackLoadedFileTexture;
				sourceName = "file: " + fullPath;
				flag = true;
			}
			else
			{
				byte[] array = File.ReadAllBytes(fullPath);
				Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);
				if (!texture2D.LoadImage(array, false))
				{
					global::UnityEngine.Object.Destroy(texture2D);
					this.imageFeedbackStatus = "Unity could not decode that image file.";
					flag = false;
				}
				else
				{
					texture2D.name = "Debug File Image - " + Path.GetFileName(fullPath);
					this.ClearImageFeedbackFileCache();
					this.imageFeedbackLoadedFileTexture = texture2D;
					this.imageFeedbackLoadedFilePath = fullPath;
					this.imageFeedbackLoadedFileWriteTicks = ticks;
					texture = texture2D;
					sourceName = "file: " + fullPath;
					flag = true;
				}
			}
		}
		catch (Exception ex)
		{
			this.imageFeedbackStatus = "Image file load failed: " + ex.GetType().Name + " - " + ex.Message;
			flag = false;
		}
		return flag;
	}

	// Token: 0x0600207A RID: 8314
	private bool TryResolveImageFeedbackFilePath(out string fullPath)
	{
		fullPath = null;
		string text = this.imageFeedbackFilePath;
		if (string.IsNullOrEmpty(text))
		{
			this.imageFeedbackStatus = "Enter a .png, .jpg, or .jpeg file path first.";
			return false;
		}
		text = text.Trim().Trim(new char[] { '"', '\'' });
		if (string.IsNullOrEmpty(text))
		{
			this.imageFeedbackStatus = "Enter a .png, .jpg, or .jpeg file path first.";
			return false;
		}
		try
		{
			if (File.Exists(text))
			{
				fullPath = Path.GetFullPath(text);
				return true;
			}
			if (!Path.IsPathRooted(text))
			{
				string[] array = new string[]
				{
					Application.dataPath,
					Application.persistentDataPath,
					Directory.GetCurrentDirectory()
				};
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						string text2 = Path.Combine(array[i], text);
						if (File.Exists(text2))
						{
							fullPath = Path.GetFullPath(text2);
							return true;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			this.imageFeedbackStatus = "Image path check failed: " + ex.GetType().Name + " - " + ex.Message;
			return false;
		}
		this.imageFeedbackStatus = "Image file not found: " + text;
		return false;
	}

	// Token: 0x0600207B RID: 8315
	private void ClearImageFeedbackFileCache()
	{
		if (this.imageFeedbackLoadedFileTexture != null)
		{
			global::UnityEngine.Object.Destroy(this.imageFeedbackLoadedFileTexture);
		}
		this.imageFeedbackLoadedFileTexture = null;
		this.imageFeedbackLoadedFilePath = null;
		this.imageFeedbackLoadedFileWriteTicks = 0L;
	}

	// Token: 0x060020F2 RID: 8434
	private void SpawnFallbackDebugImage(Texture2D texture, Sprite sprite, string sourceName, string caption, float width, float height, float duration, bool fullscreen)
	{
		Canvas canvas = this.GetOrCreateTextFeedbackCanvas();
		GameObject gameObject = new GameObject("Debug Image Feedback");
		gameObject.transform.SetParent(canvas.transform, false);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		float imageWidth = width;
		float imageHeight = height;
		float captionHeight = (string.IsNullOrEmpty(caption) ? 0f : 42f);
		if (fullscreen)
		{
			RectTransform rectTransformCanvas = canvas.transform as RectTransform;
			float canvasWidth = ((rectTransformCanvas != null) ? rectTransformCanvas.rect.width : ((float)Screen.width));
			float canvasHeight = ((rectTransformCanvas != null) ? rectTransformCanvas.rect.height : ((float)Screen.height));
			float num2 = Mathf.Max(64f, canvasWidth - 64f);
			float maxHeight = Mathf.Max(64f, canvasHeight - captionHeight - 64f);
			float sourceWidth = 1f;
			float sourceHeight = 1f;
			if (sprite != null)
			{
				sourceWidth = sprite.rect.width;
				sourceHeight = sprite.rect.height;
			}
			else if (texture != null)
			{
				sourceWidth = (float)texture.width;
				sourceHeight = (float)texture.height;
			}
			float num = Mathf.Min(num2 / Mathf.Max(1f, sourceWidth), maxHeight / Mathf.Max(1f, sourceHeight));
			num = Mathf.Max(0.01f, num);
			imageWidth = Mathf.Max(32f, sourceWidth * num);
			imageHeight = Mathf.Max(32f, sourceHeight * num);
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.sizeDelta = new Vector2(canvasWidth, canvasHeight);
			rectTransform.anchoredPosition = Vector2.zero;
		}
		else
		{
			Vector2 anchor;
			Vector2 offset;
			this.GetImageFeedbackAnchorAndOffset(out anchor, out offset);
			rectTransform.anchorMin = anchor;
			rectTransform.anchorMax = anchor;
			rectTransform.pivot = anchor;
			rectTransform.sizeDelta = new Vector2(imageWidth + (this.imageFeedbackAddBackground ? 28f : 0f), imageHeight + captionHeight + (this.imageFeedbackAddBackground ? 28f : 0f));
			rectTransform.anchoredPosition = offset + new Vector2(0f, (float)(this.imageFeedbackSpawnCount % 4) * -24f);
			this.imageFeedbackSpawnCount++;
		}
		if (this.imageFeedbackAddBackground)
		{
			gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, fullscreen ? 0.82f : 0.65f);
		}
		GameObject gameObject2 = new GameObject("Image");
		gameObject2.transform.SetParent(gameObject.transform, false);
		RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
		if (fullscreen)
		{
			rectTransform2.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform2.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform2.pivot = new Vector2(0.5f, 0.5f);
			rectTransform2.sizeDelta = new Vector2(imageWidth, imageHeight);
			rectTransform2.anchoredPosition = new Vector2(0f, string.IsNullOrEmpty(caption) ? 0f : 12f);
		}
		else
		{
			rectTransform2.anchorMin = new Vector2(0.5f, 1f);
			rectTransform2.anchorMax = new Vector2(0.5f, 1f);
			rectTransform2.pivot = new Vector2(0.5f, 1f);
			rectTransform2.sizeDelta = new Vector2(imageWidth, imageHeight);
			rectTransform2.anchoredPosition = new Vector2(0f, this.imageFeedbackAddBackground ? (-14f) : 0f);
		}
		if (sprite != null)
		{
			Image image = gameObject2.AddComponent<Image>();
			image.sprite = sprite;
			image.preserveAspect = true;
		}
		else
		{
			gameObject2.AddComponent<RawImage>().texture = texture;
		}
		if (!string.IsNullOrEmpty(caption))
		{
			GameObject gameObject3 = new GameObject("Caption");
			gameObject3.transform.SetParent(gameObject.transform, false);
			RectTransform rectTransform3 = gameObject3.AddComponent<RectTransform>();
			if (fullscreen)
			{
				rectTransform3.anchorMin = new Vector2(0.5f, 0f);
				rectTransform3.anchorMax = new Vector2(0.5f, 0f);
				rectTransform3.pivot = new Vector2(0.5f, 0f);
				rectTransform3.sizeDelta = new Vector2(Mathf.Min(imageWidth, 900f), 36f);
				rectTransform3.anchoredPosition = new Vector2(0f, 18f);
			}
			else
			{
				rectTransform3.anchorMin = new Vector2(0.5f, 0f);
				rectTransform3.anchorMax = new Vector2(0.5f, 0f);
				rectTransform3.pivot = new Vector2(0.5f, 0f);
				rectTransform3.sizeDelta = new Vector2(imageWidth, 36f);
				rectTransform3.anchoredPosition = new Vector2(0f, this.imageFeedbackAddBackground ? 14f : 0f);
			}
			Text text = gameObject3.AddComponent<Text>();
			text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			text.fontSize = 22;
			text.alignment = TextAnchor.MiddleCenter;
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Overflow;
			text.color = Color.white;
			text.text = caption;
			gameObject3.AddComponent<Shadow>().effectDistance = new Vector2(2f, -2f);
		}
		global::UnityEngine.Object.Destroy(gameObject, duration);
	}

	// Token: 0x0400146D RID: 5229
	public string message = "Debug window active";

	// Token: 0x0400146E RID: 5230
	private Rect windowRect = new Rect(40f, 40f, 560f, 520f);

	// Token: 0x0400146F RID: 5231
	private bool visible = true;

	// Token: 0x04001470 RID: 5232
	private const float PaddingX = 24f;

	// Token: 0x04001471 RID: 5233
	private const float PaddingY = 58f;

	// Token: 0x04001472 RID: 5234
	private const float MinWidth = 340f;

	// Token: 0x04001473 RID: 5235
	private const float MinHeight = 420f;

	// Token: 0x04001474 RID: 5236
	private const float MaxWidth = 700f;

	// Token: 0x04001475 RID: 5237
	private readonly List<string> lines = new List<string>();

	// Token: 0x04001476 RID: 5238
	private Noclip noclip;

	// Token: 0x04001477 RID: 5239
	private Vector2 scrollPosition;

	// Token: 0x04001478 RID: 5240
	private string scanOutput = "";

	// Token: 0x04001479 RID: 5241
	private int scanClickCount;

	// Token: 0x0400147A RID: 5242
	private PlayerProfile playerProfile;

	// Token: 0x0400147B RID: 5243
	private PlayerInventory playerInventory;

	// Token: 0x0400147C RID: 5244
	private Bank bank;

	// Token: 0x0400147D RID: 5245
	private const float TabBarHeight = 26f;

	// Token: 0x0400147E RID: 5246
	private const float FooterHeight = 32f;

	// Token: 0x0400147F RID: 5247
	private int activeTab;

	// Token: 0x04001480 RID: 5248
	private static readonly string[] TabNames = new string[] { "Info", "Controls", "Player Settings", "Scan", "Class Editor", "Organs", "Text", "Images" };

	// Token: 0x04001481 RID: 5249
	private readonly Vector2[] tabScrollPositions = new Vector2[8];

	// Token: 0x04001482 RID: 5250
	private PlayerSettings playerSettings;

	// Token: 0x04001483 RID: 5251
	private readonly Dictionary<string, string> playerSettingInputs = new Dictionary<string, string>();

	// Token: 0x0400149B RID: 5275
	private int classEditorTargetIndex;

	// Token: 0x0400149C RID: 5276
	private bool classEditorShowPrivate;

	// Token: 0x0400149D RID: 5277
	private bool classEditorEditPrivate;

	// Token: 0x0400149E RID: 5278
	private string classEditorStatus = "";

	// Token: 0x0400149F RID: 5279
	private readonly Dictionary<string, string> classEditorInputs = new Dictionary<string, string>();

	// Token: 0x040014CE RID: 5326
	private PlayerOrgans playerOrgans;

	// Token: 0x040014DC RID: 5340
	private bool organEditorInitialized;

	// Token: 0x040014DD RID: 5341
	private PlayerOrgans organEditorTarget;

	// Token: 0x040014DE RID: 5342
	private bool organLeftEye = true;

	// Token: 0x040014DF RID: 5343
	private bool organRightEye = true;

	// Token: 0x040014E0 RID: 5344
	private bool organBody = true;

	// Token: 0x040014E1 RID: 5345
	private bool organMouth = true;

	// Token: 0x040014E2 RID: 5346
	private string organEditorStatus = "";

	// Token: 0x0400152B RID: 5419
	public KeyCode mouseRevealKey = KeyCode.Mouse4;

	// Token: 0x0400152C RID: 5420
	private bool mouseRevealActive;

	// Token: 0x0400152D RID: 5421
	private bool mouseRevealPreviousVisible;

	// Token: 0x0400152E RID: 5422
	private CursorLockMode mouseRevealPreviousLockState;

	// Token: 0x040015A3 RID: 5539
	private string textFeedbackHeaderInput = "Debug";

	// Token: 0x040015A4 RID: 5540
	private string textFeedbackMessageInput = "Custom feedback text";

	// Token: 0x040015A5 RID: 5541
	private string textFeedbackDurationInput = "3";

	// Token: 0x040015A6 RID: 5542
	private int textFeedbackColorIndex;

	// Token: 0x040015A7 RID: 5543
	private bool textFeedbackUseMoneyPrefab = true;

	// Token: 0x040015A8 RID: 5544
	private string textFeedbackStatus = "";

	// Token: 0x040015A9 RID: 5545
	private Canvas textFeedbackCanvas;

	// Token: 0x040015AA RID: 5546
	private int textFeedbackSpawnCount;

	// Token: 0x040015AB RID: 5547
	private static readonly string[] TextFeedbackColorNames = new string[] { "White", "Green", "Red", "Yellow", "Cyan", "Magenta" };

	// Token: 0x040015DD RID: 5597
	private int imageFeedbackSourceIndex;

	// Token: 0x040015DE RID: 5598
	private string imageFeedbackResourcePath = "";

	// Token: 0x040015DF RID: 5599
	private string imageFeedbackCaption = "";

	// Token: 0x040015E0 RID: 5600
	private string imageFeedbackDurationInput = "3";

	// Token: 0x040015E1 RID: 5601
	private string imageFeedbackWidthInput = "220";

	// Token: 0x040015E2 RID: 5602
	private string imageFeedbackHeightInput = "220";

	// Token: 0x040015E3 RID: 5603
	private int imageFeedbackPositionIndex = 1;

	// Token: 0x040015E4 RID: 5604
	private bool imageFeedbackAddBackground = true;

	// Token: 0x040015E5 RID: 5605
	private string imageFeedbackStatus = "";

	// Token: 0x040015E6 RID: 5606
	private int imageFeedbackSpawnCount;

	// Token: 0x040015E7 RID: 5607
	private Texture2D imageFeedbackPlaceholderTexture;

	// Token: 0x040015E8 RID: 5608
	private static readonly string[] ImageFeedbackSourceNames = new string[] { "Profile", "Resources Texture", "Resources Sprite", "File Path", "Placeholder" };

	// Token: 0x040015E9 RID: 5609
	private static readonly string[] ImageFeedbackPositionNames = new string[] { "Top Left", "Top", "Top Right", "Left", "Center", "Right", "Bottom Left", "Bottom", "Bottom Right" };

	// Token: 0x0400161D RID: 5661
	private string imageFeedbackFilePath = "";

	// Token: 0x0400161E RID: 5662
	private string imageFeedbackLoadedFilePath;

	// Token: 0x0400161F RID: 5663
	private long imageFeedbackLoadedFileWriteTicks;

	// Token: 0x04001620 RID: 5664
	private Texture2D imageFeedbackLoadedFileTexture;

	// Token: 0x040016AB RID: 5803
	private bool imageFeedbackFullscreen;
}
