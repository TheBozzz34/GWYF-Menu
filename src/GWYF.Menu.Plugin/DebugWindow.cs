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

	// Token: 0x0400147F RID: 5247
	private int activeTab;

	// Token: 0x04001480 RID: 5248
	private static readonly string[] TabNames = new string[] { "Info", "Controls", "Player Settings", "Scan", "Class Editor", "Organs", "Text", "Images" };

	// Token: 0x04001481 RID: 5249
	private readonly Vector2[] tabScrollPositions = new Vector2[8];
}


