using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Extensions;
using Mirror;
using UnityEngine;

public partial class DebugWindow
{
	private void DrawTeleportTab()
	{
		GUILayout.Label("Teleport", this.sectionLabelStyle, Array.Empty<GUILayoutOption>());
		GUIStyle statusStyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		List<PlayerProfile> playerProfiles = this.GetAllTeleportPlayerProfiles();
		this.teleportTargetIndex = Mathf.Clamp(this.teleportTargetIndex, 0, Mathf.Max(0, playerProfiles.Count - 1));
		GUILayout.Label("Local Player: " + ((this.playerProfile != null) ? this.playerProfile.playerName : "not found"), Array.Empty<GUILayoutOption>());
		GUILayout.Label("Players: " + playerProfiles.Count.ToString(CultureInfo.InvariantCulture), Array.Empty<GUILayoutOption>());
		GUILayout.Space(6f);
		this.teleportIncludeLocalPlayer = GUILayout.Toggle(this.teleportIncludeLocalPlayer, "Include local player", Array.Empty<GUILayoutOption>());
		this.DrawTeleportOffsetControls();
		GUILayout.Space(6f);
		if (playerProfiles.Count == 0)
		{
			GUILayout.Label("No synced players found.", statusStyle, Array.Empty<GUILayoutOption>());
		}
		else
		{
			string[] labels = new string[playerProfiles.Count];
			for (int i = 0; i < playerProfiles.Count; i++)
			{
				labels[i] = this.GetTeleportPlayerLabel(playerProfiles[i]);
			}
			GUILayout.Label("Target", Array.Empty<GUILayoutOption>());
			this.teleportTargetIndex = GUILayout.SelectionGrid(this.teleportTargetIndex, labels, 1, Array.Empty<GUILayoutOption>());
			PlayerProfile targetPlayer = playerProfiles[this.teleportTargetIndex];
			this.DrawTeleportTargetDetails(targetPlayer, statusStyle);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Server Teleport", new GUILayoutOption[] { GUILayout.Height(28f) }))
			{
				this.TeleportToPlayerViaCommand(targetPlayer);
			}
			if (GUILayout.Button("Local Teleport", new GUILayoutOption[] { GUILayout.Height(28f) }))
			{
				this.TeleportLocalPlayerTo(targetPlayer);
			}
			GUILayout.EndHorizontal();
		}
		if (!string.IsNullOrEmpty(this.teleportStatus))
		{
			GUILayout.Space(8f);
			GUILayout.Label(this.teleportStatus, statusStyle, Array.Empty<GUILayoutOption>());
		}
	}

	private List<PlayerProfile> GetAllTeleportPlayerProfiles()
	{
		List<PlayerProfile> playerProfiles = new List<PlayerProfile>();
		foreach (PlayerProfile playerProfile in global::UnityEngine.Object.FindObjectsByType<PlayerProfile>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (playerProfile != null && playerProfile.hasSynced && !string.IsNullOrEmpty(playerProfile.playerName) && (this.teleportIncludeLocalPlayer || !this.IsTeleportLocalPlayer(playerProfile)))
			{
				playerProfiles.Add(playerProfile);
			}
		}
		return playerProfiles;
	}

	private bool IsTeleportLocalPlayer(PlayerProfile playerProfile)
	{
		if (playerProfile == null)
		{
			return false;
		}
		if (this.playerProfile != null && playerProfile.netId == this.playerProfile.netId)
		{
			return true;
		}
		NetworkIdentity networkIdentity = playerProfile.GetComponent<NetworkIdentity>();
		return networkIdentity != null && networkIdentity.isLocalPlayer;
	}

	private string GetTeleportPlayerLabel(PlayerProfile playerProfile)
	{
		if (playerProfile == null)
		{
			return "<null>";
		}
		string text = playerProfile.playerName;
		if (this.IsTeleportLocalPlayer(playerProfile))
		{
			text += " (local)";
		}
		GameObject localPlayerObject = this.FindLocalPlayerObject();
		if (localPlayerObject != null)
		{
			float distance = Vector3.Distance(localPlayerObject.transform.position, playerProfile.transform.position);
			text += " - " + distance.ToString("0.0", CultureInfo.InvariantCulture) + "m";
		}
		return text;
	}

	private void DrawTeleportOffsetControls()
	{
		GUILayout.Label("Offset", Array.Empty<GUILayoutOption>());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("X", new GUILayoutOption[] { GUILayout.Width(20f) });
		this.teleportOffsetXInput = GUILayout.TextField(this.teleportOffsetXInput, new GUILayoutOption[] { GUILayout.Width(60f) });
		GUILayout.Label("Y", new GUILayoutOption[] { GUILayout.Width(20f) });
		this.teleportOffsetYInput = GUILayout.TextField(this.teleportOffsetYInput, new GUILayoutOption[] { GUILayout.Width(60f) });
		GUILayout.Label("Z", new GUILayoutOption[] { GUILayout.Width(20f) });
		this.teleportOffsetZInput = GUILayout.TextField(this.teleportOffsetZInput, new GUILayoutOption[] { GUILayout.Width(60f) });
		if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Width(70f) }))
		{
			this.teleportOffsetXInput = "0";
			this.teleportOffsetYInput = "2";
			this.teleportOffsetZInput = "0";
		}
		GUILayout.EndHorizontal();
	}

	private void DrawTeleportTargetDetails(PlayerProfile targetPlayer, GUIStyle statusStyle)
	{
		if (targetPlayer == null)
		{
			return;
		}
		Vector3 offset;
		bool hasOffset = this.TryGetTeleportOffset(out offset);
		Vector3 position = targetPlayer.transform.position + (hasOffset ? offset : Vector3.up * 2f);
		GUILayout.Space(6f);
		GUILayout.Label("netId: " + targetPlayer.netId.ToString(), Array.Empty<GUILayoutOption>());
		GUILayout.Label("Target position: " + this.FormatTeleportVector(targetPlayer.transform.position), Array.Empty<GUILayoutOption>());
		GUILayout.Label("Teleport position: " + this.FormatTeleportVector(position), Array.Empty<GUILayoutOption>());
		if (!hasOffset)
		{
			GUILayout.Label("Offset must use numeric X/Y/Z values.", statusStyle, Array.Empty<GUILayoutOption>());
		}
	}

	private void TeleportToPlayerViaCommand(PlayerProfile targetPlayer)
	{
		if (targetPlayer == null)
		{
			this.teleportStatus = "Select a player first.";
			return;
		}
		Vector3 offset;
		if (!this.TryGetTeleportOffset(out offset))
		{
			this.teleportStatus = "Offset must use numeric X/Y/Z values.";
			return;
		}
		NewConsole console = this.FindNewConsole();
		if (console == null)
		{
			this.teleportStatus = "NewConsole not found.";
			return;
		}
		Vector3 position = targetPlayer.transform.position + offset;
		try
		{
			MethodInfo method = typeof(NewConsole).GetMethod("CmdTeleportToPlayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				this.teleportStatus = "NewConsole.CmdTeleportToPlayer was not found.";
				return;
			}
			method.Invoke(console, new object[] { targetPlayer.netId, position, null });
			this.teleportStatus = "Server teleport sent to " + targetPlayer.playerName + " at " + this.FormatTeleportVector(position) + ".";
		}
		catch (Exception ex)
		{
			Exception innerException = ex.InnerException ?? ex;
			this.teleportStatus = "Server teleport failed: " + innerException.GetType().Name + " - " + innerException.Message;
		}
	}

	private void TeleportLocalPlayerTo(PlayerProfile targetPlayer)
	{
		if (targetPlayer == null)
		{
			this.teleportStatus = "Select a player first.";
			return;
		}
		Vector3 offset;
		if (!this.TryGetTeleportOffset(out offset))
		{
			this.teleportStatus = "Offset must use numeric X/Y/Z values.";
			return;
		}
		GameObject localPlayerObject = this.FindLocalPlayerObject();
		if (localPlayerObject == null)
		{
			this.teleportStatus = "Local player object not found.";
			return;
		}
		Vector3 position = targetPlayer.transform.position + offset;
		localPlayerObject.transform.position = position;
		this.teleportStatus = "Local player moved to " + targetPlayer.playerName + " at " + this.FormatTeleportVector(position) + ".";
	}

	private NewConsole FindNewConsole()
	{
		try
		{
			NewConsole console = NetworkSingleton<NewConsole>.Instance;
			if (console != null)
			{
				return console;
			}
		}
		catch
		{
		}
		NewConsole[] consoles = global::UnityEngine.Object.FindObjectsByType<NewConsole>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		if (consoles != null && consoles.Length != 0)
		{
			return consoles[0];
		}
		return null;
	}

	private bool TryGetTeleportOffset(out Vector3 offset)
	{
		float x;
		float y;
		float z;
		if (float.TryParse(this.teleportOffsetXInput, NumberStyles.Float, CultureInfo.InvariantCulture, out x) && float.TryParse(this.teleportOffsetYInput, NumberStyles.Float, CultureInfo.InvariantCulture, out y) && float.TryParse(this.teleportOffsetZInput, NumberStyles.Float, CultureInfo.InvariantCulture, out z))
		{
			offset = new Vector3(x, y, z);
			return true;
		}
		offset = Vector3.up * 2f;
		return false;
	}

	private string FormatTeleportVector(Vector3 vector)
	{
		return vector.x.ToString("0.##", CultureInfo.InvariantCulture) + ", " + vector.y.ToString("0.##", CultureInfo.InvariantCulture) + ", " + vector.z.ToString("0.##", CultureInfo.InvariantCulture);
	}

	private int teleportTargetIndex;

	private bool teleportIncludeLocalPlayer;

	private string teleportOffsetXInput = "0";

	private string teleportOffsetYInput = "2";

	private string teleportOffsetZInput = "0";

	private string teleportStatus = "";
}
