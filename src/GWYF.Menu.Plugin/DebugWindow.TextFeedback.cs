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
			MoneyDisplayAndFeedbacks[] array = global::UnityEngine.Object.FindObjectsByType<MoneyDisplayAndFeedbacks>(FindObjectsSortMode.None);
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
}



