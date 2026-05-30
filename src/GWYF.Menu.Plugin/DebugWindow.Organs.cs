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
}



