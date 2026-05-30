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
public partial class DebugWindow : MonoBehaviour
{
	// Token: 0x06001EC1 RID: 7873
	private void OnGUI()
	{
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F8)
		{
			this.SetMenuVisible(!this.visible);
		}
		if (!this.visible)
		{
			return;
		}
		this.EnsureModernGuiStyles();
		GUISkin skin = GUI.skin;
		Color color = GUI.color;
		Color backgroundColor = GUI.backgroundColor;
		Color contentColor = GUI.contentColor;
		try
		{
			GUI.skin = this.modernSkin;
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			GUI.contentColor = DebugWindow.TextColor;
			this.BuildLines();
			float num = (float)Screen.height - 20f;
			this.windowRect.width = Mathf.Clamp(this.windowRect.width, 340f, 860f);
			this.windowRect.height = Mathf.Clamp(this.windowRect.height, 420f, num);
			this.windowRect.x = Mathf.Clamp(this.windowRect.x, 10f, (float)Screen.width - this.windowRect.width - 10f);
			this.windowRect.y = Mathf.Clamp(this.windowRect.y, 10f, (float)Screen.height - this.windowRect.height - 10f);
			this.windowRect = GUI.Window(987654, this.windowRect, new GUI.WindowFunction(this.DrawWindow), string.Empty, this.windowStyle);
		}
		finally
		{
			GUI.skin = skin;
			GUI.color = color;
			GUI.backgroundColor = backgroundColor;
			GUI.contentColor = contentColor;
		}
	}

	// Token: 0x06001EC2 RID: 7874
	private void DrawWindow(int id)
	{
		this.DrawTitleBar();
		GUILayout.Space(8f);
		int tabColumnCount = this.GetTabColumnCount();
		int tabRowCount = this.GetTabRowCount(tabColumnCount);
		this.DrawTabBar(tabColumnCount, tabRowCount);
		GUILayout.Space(8f);
		float num = (float)this.windowStyle.padding.vertical + DebugWindow.TitleBarHeight + (float)tabRowCount * DebugWindow.TabBarHeight + 16f + DebugWindow.FooterHeight + 8f;
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
			this.DrawMoneyTab();
			break;
		case 4:
			this.DrawTeleportTab();
			break;
		case 5:
			this.DrawItemsTab();
			break;
		case 6:
			this.DrawScanTab();
			break;
		case 7:
			this.DrawClassEditorTab();
			break;
		case 8:
			this.DrawOrgansTab();
			break;
		case 9:
			this.DrawTextFeedbackTab();
			break;
		case 10:
			this.DrawImageFeedbackTab();
			break;
		}
		GUILayout.EndScrollView();
		GUILayout.Space(8f);
		if (GUILayout.Button("Close Menu", new GUILayoutOption[] { GUILayout.Height(28f) }))
		{
			this.SetMenuVisible(false);
		}
		GUI.DragWindow(new Rect(0f, 0f, 10000f, DebugWindow.TitleBarHeight + 18f));
	}

	private void DrawTitleBar()
	{
		GUILayout.BeginHorizontal(this.titleBarStyle, new GUILayoutOption[] { GUILayout.Height(DebugWindow.TitleBarHeight) });
		GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
		GUILayout.Label("GWYF Menu", this.titleStyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label("F8 toggles this menu", this.subtitleStyle, Array.Empty<GUILayoutOption>());
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close", this.closeButtonStyle, new GUILayoutOption[] { GUILayout.Width(64f), GUILayout.Height(28f) }))
		{
			this.SetMenuVisible(false);
		}
		GUILayout.EndHorizontal();
	}

	private void DrawTabBar(int tabColumnCount, int tabRowCount)
	{
		this.activeTab = Mathf.Clamp(this.activeTab, 0, DebugWindow.TabNames.Length - 1);
		this.activeTab = GUILayout.SelectionGrid(this.activeTab, DebugWindow.TabNames, tabColumnCount, this.tabButtonStyle, new GUILayoutOption[] { GUILayout.Height((float)tabRowCount * DebugWindow.TabBarHeight) });
	}

	private int GetTabColumnCount()
	{
		float availableWidth = Mathf.Max(DebugWindow.MinWidth, this.windowRect.width - (float)this.windowStyle.padding.horizontal);
		return Mathf.Clamp(Mathf.FloorToInt(availableWidth / DebugWindow.MinTabButtonWidth), 2, DebugWindow.TabNames.Length);
	}

	private int GetTabRowCount(int tabColumnCount)
	{
		return Mathf.CeilToInt((float)DebugWindow.TabNames.Length / (float)Mathf.Max(1, tabColumnCount));
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
		foreach (NetworkIdentity networkIdentity in global::UnityEngine.Object.FindObjectsByType<NetworkIdentity>(FindObjectsSortMode.None))
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
		Bank[] array = global::UnityEngine.Object.FindObjectsByType<Bank>(FindObjectsSortMode.None);
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
		GUILayout.Label("Movement", this.sectionLabelStyle, Array.Empty<GUILayoutOption>());
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
		GUILayout.Label("Keybinds", this.sectionLabelStyle, Array.Empty<GUILayoutOption>());
		GUIStyle guistyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		GUILayout.Label("F8  -  Toggle this window and cursor", guistyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label(this.mouseRevealKey.ToString() + "  -  Hold to temporarily show/unlock mouse while the menu is closed", guistyle, Array.Empty<GUILayoutOption>());
	}

	private void SetMenuVisible(bool isVisible)
	{
		if (this.visible == isVisible)
		{
			return;
		}
		this.visible = isVisible;
		if (this.visible)
		{
			this.RestoreMouseRevealState();
			this.ReleaseCursorForMenu();
			return;
		}
		this.RestoreMenuCursorState();
	}

	private void EnsureModernGuiStyles()
	{
		if (this.modernSkin != null)
		{
			return;
		}
		this.modernSkin = UnityEngine.Object.Instantiate<GUISkin>(GUI.skin);
		this.windowStyle = new GUIStyle(GUI.skin.window);
		this.windowStyle.normal.background = this.CreateRoundedTexture(24, 8, DebugWindow.WindowColor);
		this.windowStyle.onNormal.background = this.windowStyle.normal.background;
		this.windowStyle.normal.textColor = DebugWindow.TextColor;
		this.windowStyle.padding = new RectOffset(14, 14, 14, 14);
		this.windowStyle.border = new RectOffset(8, 8, 8, 8);
		this.windowStyle.margin = new RectOffset(0, 0, 0, 0);
		this.modernSkin.window = this.windowStyle;
		this.titleBarStyle = new GUIStyle();
		this.titleBarStyle.normal.background = this.CreateRoundedTexture(24, 8, DebugWindow.TitleBarColor);
		this.titleBarStyle.padding = new RectOffset(12, 8, 8, 8);
		this.titleBarStyle.border = new RectOffset(8, 8, 8, 8);
		this.titleStyle = new GUIStyle(GUI.skin.label);
		this.titleStyle.fontSize = 18;
		this.titleStyle.fontStyle = FontStyle.Bold;
		this.titleStyle.normal.textColor = DebugWindow.TextColor;
		this.titleStyle.margin = new RectOffset(0, 0, 0, 0);
		this.subtitleStyle = new GUIStyle(GUI.skin.label);
		this.subtitleStyle.fontSize = 11;
		this.subtitleStyle.normal.textColor = DebugWindow.MutedTextColor;
		this.subtitleStyle.margin = new RectOffset(1, 0, 0, 0);
		this.sectionLabelStyle = new GUIStyle(GUI.skin.label);
		this.sectionLabelStyle.fontSize = 13;
		this.sectionLabelStyle.fontStyle = FontStyle.Bold;
		this.sectionLabelStyle.normal.textColor = DebugWindow.AccentColor;
		this.sectionLabelStyle.margin = new RectOffset(0, 0, 8, 4);
		this.tabButtonStyle = this.CreateButtonStyle(DebugWindow.TabColor, DebugWindow.TabHoverColor, DebugWindow.AccentColor, 12, FontStyle.Bold);
		this.tabButtonStyle.onNormal.background = this.CreateRoundedTexture(16, 6, DebugWindow.AccentColor);
		this.tabButtonStyle.onHover.background = this.CreateRoundedTexture(16, 6, DebugWindow.AccentHoverColor);
		this.tabButtonStyle.onActive.background = this.CreateRoundedTexture(16, 6, DebugWindow.AccentPressedColor);
		this.tabButtonStyle.padding = new RectOffset(8, 8, 5, 5);
		this.tabButtonStyle.margin = new RectOffset(3, 3, 3, 3);
		this.closeButtonStyle = this.CreateButtonStyle(DebugWindow.CloseButtonColor, DebugWindow.CloseButtonHoverColor, DebugWindow.CloseButtonPressedColor, 12, FontStyle.Bold);
		this.modernSkin.button = this.CreateButtonStyle(DebugWindow.ButtonColor, DebugWindow.ButtonHoverColor, DebugWindow.ButtonPressedColor, 13, FontStyle.Normal);
		this.modernSkin.label = new GUIStyle(GUI.skin.label);
		this.modernSkin.label.normal.textColor = DebugWindow.TextColor;
		this.modernSkin.label.fontSize = 13;
		this.modernSkin.label.margin = new RectOffset(2, 2, 2, 2);
		this.modernSkin.textField = this.CreateInputStyle(GUI.skin.textField);
		this.modernSkin.textArea = this.CreateInputStyle(GUI.skin.textArea);
		this.modernSkin.box = new GUIStyle(GUI.skin.box);
		this.modernSkin.box.normal.background = this.CreateRoundedTexture(18, 6, DebugWindow.PanelColor);
		this.modernSkin.box.normal.textColor = DebugWindow.TextColor;
		this.modernSkin.box.border = new RectOffset(6, 6, 6, 6);
		this.modernSkin.box.padding = new RectOffset(10, 10, 10, 10);
		this.modernSkin.toggle = new GUIStyle(GUI.skin.toggle);
		this.modernSkin.toggle.normal.textColor = DebugWindow.TextColor;
		this.modernSkin.toggle.hover.textColor = Color.white;
		this.modernSkin.toggle.onNormal.textColor = DebugWindow.TextColor;
		this.modernSkin.toggle.onHover.textColor = Color.white;
	}

	private GUIStyle CreateButtonStyle(Color normalColor, Color hoverColor, Color activeColor, int fontSize, FontStyle fontStyle)
	{
		GUIStyle guistyle = new GUIStyle(GUI.skin.button);
		guistyle.normal.background = this.CreateRoundedTexture(16, 6, normalColor);
		guistyle.hover.background = this.CreateRoundedTexture(16, 6, hoverColor);
		guistyle.active.background = this.CreateRoundedTexture(16, 6, activeColor);
		guistyle.focused.background = guistyle.hover.background;
		guistyle.onNormal.background = guistyle.active.background;
		guistyle.onHover.background = guistyle.hover.background;
		guistyle.onActive.background = guistyle.active.background;
		guistyle.normal.textColor = DebugWindow.TextColor;
		guistyle.hover.textColor = Color.white;
		guistyle.active.textColor = Color.white;
		guistyle.focused.textColor = Color.white;
		guistyle.onNormal.textColor = Color.white;
		guistyle.onHover.textColor = Color.white;
		guistyle.onActive.textColor = Color.white;
		guistyle.border = new RectOffset(6, 6, 6, 6);
		guistyle.padding = new RectOffset(10, 10, 5, 5);
		guistyle.margin = new RectOffset(3, 3, 3, 3);
		guistyle.fontSize = fontSize;
		guistyle.fontStyle = fontStyle;
		guistyle.alignment = TextAnchor.MiddleCenter;
		return guistyle;
	}

	private GUIStyle CreateInputStyle(GUIStyle source)
	{
		GUIStyle guistyle = new GUIStyle(source);
		guistyle.normal.background = this.CreateRoundedTexture(16, 5, DebugWindow.InputColor);
		guistyle.focused.background = this.CreateRoundedTexture(16, 5, DebugWindow.InputFocusColor);
		guistyle.hover.background = guistyle.focused.background;
		guistyle.active.background = guistyle.focused.background;
		guistyle.normal.textColor = DebugWindow.TextColor;
		guistyle.focused.textColor = Color.white;
		guistyle.hover.textColor = Color.white;
		guistyle.active.textColor = Color.white;
		guistyle.border = new RectOffset(5, 5, 5, 5);
		guistyle.padding = new RectOffset(8, 8, 5, 5);
		guistyle.margin = new RectOffset(3, 3, 3, 3);
		return guistyle;
	}

	private Texture2D CreateRoundedTexture(int size, int radius, Color color)
	{
		Texture2D texture2D = new Texture2D(size, size, TextureFormat.ARGB32, false);
		texture2D.hideFlags = HideFlags.HideAndDontSave;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.filterMode = FilterMode.Bilinear;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float num = Mathf.Clamp((float)j + 0.5f, (float)radius, (float)(size - radius));
				float num2 = Mathf.Clamp((float)i + 0.5f, (float)radius, (float)(size - radius));
				float num3 = (float)j + 0.5f - num;
				float num4 = (float)i + 0.5f - num2;
				bool flag = num3 * num3 + num4 * num4 <= (float)(radius * radius);
				texture2D.SetPixel(j, i, flag ? color : Color.clear);
			}
		}
		texture2D.Apply();
		return texture2D;
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

	private const float TitleBarHeight = 48f;

	private const float MinTabButtonWidth = 112f;

	// Token: 0x0400147F RID: 5247
	private int activeTab;

	// Token: 0x04001480 RID: 5248
	private static readonly string[] TabNames = new string[] { "Info", "Controls", "Player Settings", "Money", "Teleport", "Items", "Scan", "Class Editor", "Organs", "Text", "Images" };

	// Token: 0x04001481 RID: 5249
	private readonly Vector2[] tabScrollPositions = new Vector2[DebugWindow.TabNames.Length];

	private static readonly Color WindowColor = new Color(0.05882353f, 0.07058824f, 0.09019608f, 0.96f);

	private static readonly Color TitleBarColor = new Color(0.09019608f, 0.1098039f, 0.1372549f, 0.98f);

	private static readonly Color PanelColor = new Color(0.1019608f, 0.1215686f, 0.1490196f, 0.95f);

	private static readonly Color TabColor = new Color(0.1215686f, 0.145098f, 0.1764706f, 0.96f);

	private static readonly Color TabHoverColor = new Color(0.1647059f, 0.2f, 0.2392157f, 0.98f);

	private static readonly Color ButtonColor = new Color(0.1490196f, 0.172549f, 0.2078431f, 0.98f);

	private static readonly Color ButtonHoverColor = new Color(0.1960784f, 0.2352941f, 0.2862745f, 1f);

	private static readonly Color ButtonPressedColor = new Color(0.09803922f, 0.3411765f, 0.3411765f, 1f);

	private static readonly Color CloseButtonColor = new Color(0.2196078f, 0.1372549f, 0.1568628f, 0.96f);

	private static readonly Color CloseButtonHoverColor = new Color(0.345098f, 0.1647059f, 0.1882353f, 1f);

	private static readonly Color CloseButtonPressedColor = new Color(0.4862745f, 0.1882353f, 0.2235294f, 1f);

	private static readonly Color InputColor = new Color(0.04705882f, 0.05882353f, 0.07450981f, 0.98f);

	private static readonly Color InputFocusColor = new Color(0.08235294f, 0.1098039f, 0.1372549f, 1f);

	private static readonly Color AccentColor = new Color(0.08235294f, 0.6078432f, 0.5764706f, 1f);

	private static readonly Color AccentHoverColor = new Color(0.1098039f, 0.7058824f, 0.6705883f, 1f);

	private static readonly Color AccentPressedColor = new Color(0.04705882f, 0.4392157f, 0.427451f, 1f);

	private static readonly Color TextColor = new Color(0.9019608f, 0.9294118f, 0.9372549f, 1f);

	private static readonly Color MutedTextColor = new Color(0.5882353f, 0.6588235f, 0.6941177f, 1f);

	private GUISkin modernSkin;

	private GUIStyle windowStyle;

	private GUIStyle titleBarStyle;

	private GUIStyle titleStyle;

	private GUIStyle subtitleStyle;

	private GUIStyle sectionLabelStyle;

	private GUIStyle tabButtonStyle;

	private GUIStyle closeButtonStyle;
}


