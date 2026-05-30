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

public partial class DebugWindow
{
	// Token: 0x06001EDC RID: 7900
	private void DrawClassEditorTab()
	{
		GameObject gameObject = this.FindLocalPlayerObject();
		if (gameObject == null)
		{
			GUILayout.Label("Local player: not found", Array.Empty<GUILayoutOption>());
			return;
		}
		GUILayout.Label("Class Editor", this.sectionLabelStyle, Array.Empty<GUILayoutOption>());
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
		GUILayout.Label(displayName, this.sectionLabelStyle, Array.Empty<GUILayoutOption>());
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
}



