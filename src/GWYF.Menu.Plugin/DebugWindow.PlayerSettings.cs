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

	// Token: 0x04001482 RID: 5250
	private PlayerSettings playerSettings;

	// Token: 0x04001483 RID: 5251
	private readonly Dictionary<string, string> playerSettingInputs = new Dictionary<string, string>();
}



