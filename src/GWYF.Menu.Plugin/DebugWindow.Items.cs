using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public partial class DebugWindow
{
	private void DrawItemsTab()
	{
		GUILayout.Label("=== Items ===", Array.Empty<GUILayoutOption>());
		GUIStyle statusStyle = new GUIStyle(GUI.skin.label)
		{
			wordWrap = true
		};
		if (!this.itemSpawnInitialized)
		{
			this.RefreshItemSpawnPrefabs();
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Refresh", new GUILayoutOption[] { GUILayout.Height(24f), GUILayout.Width(90f) }))
		{
			this.RefreshItemSpawnPrefabs();
		}
		GUILayout.Label("Found: " + this.itemSpawnAvailablePrefabs.Count.ToString(CultureInfo.InvariantCulture), Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.Space(6f);
		this.DrawItemSpawnOptions();
		GUILayout.Space(6f);
		this.DrawItemSpawnPicker(statusStyle);
		if (!string.IsNullOrEmpty(this.itemSpawnStatus))
		{
			GUILayout.Space(8f);
			GUILayout.Label(this.itemSpawnStatus, statusStyle, Array.Empty<GUILayoutOption>());
		}
	}

	private void RefreshItemSpawnPrefabs()
	{
		this.itemSpawnAvailablePrefabs.Clear();
		this.itemSpawnInitialized = true;
		if (this.itemSpawnableSettings == null)
		{
			this.itemSpawnableSettings = Resources.Load<SpawnableSettings>("SpawnableSettings");
		}
		if (this.itemSpawnableSettings != null && this.itemSpawnableSettings.isEnabled)
		{
			foreach (SpawnableSO spawnableSO in this.itemSpawnableSettings.spawnables)
			{
				if (spawnableSO != null)
				{
					this.itemSpawnAvailablePrefabs.Add(spawnableSO);
				}
			}
			this.itemSpawnStatus = "Found " + this.itemSpawnAvailablePrefabs.Count.ToString(CultureInfo.InvariantCulture) + " spawnable prefabs.";
			this.itemSpawnSelectedIndex = Mathf.Clamp(this.itemSpawnSelectedIndex, 0, Mathf.Max(0, this.itemSpawnAvailablePrefabs.Count - 1));
			return;
		}
		this.itemSpawnStatus = "SpawnableSettings not found or disabled.";
	}

	private void DrawItemSpawnOptions()
	{
		GUILayout.Label("Search", Array.Empty<GUILayoutOption>());
		this.itemSpawnSearchInput = GUILayout.TextField(this.itemSpawnSearchInput, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Spawn Offset", Array.Empty<GUILayoutOption>());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("X", new GUILayoutOption[] { GUILayout.Width(20f) });
		this.itemSpawnOffsetXInput = GUILayout.TextField(this.itemSpawnOffsetXInput, new GUILayoutOption[] { GUILayout.Width(60f) });
		GUILayout.Label("Y", new GUILayoutOption[] { GUILayout.Width(20f) });
		this.itemSpawnOffsetYInput = GUILayout.TextField(this.itemSpawnOffsetYInput, new GUILayoutOption[] { GUILayout.Width(60f) });
		GUILayout.Label("Z", new GUILayoutOption[] { GUILayout.Width(20f) });
		this.itemSpawnOffsetZInput = GUILayout.TextField(this.itemSpawnOffsetZInput, new GUILayoutOption[] { GUILayout.Width(60f) });
		if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Width(70f) }))
		{
			this.itemSpawnOffsetXInput = "0";
			this.itemSpawnOffsetYInput = "1";
			this.itemSpawnOffsetZInput = "2";
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Count", new GUILayoutOption[] { GUILayout.Width(80f) });
		this.itemSpawnCountInput = GUILayout.TextField(this.itemSpawnCountInput, new GUILayoutOption[] { GUILayout.Width(80f) });
		GUILayout.Label("Chip Value", new GUILayoutOption[] { GUILayout.Width(80f) });
		this.itemSpawnChipValueInput = GUILayout.TextField(this.itemSpawnChipValueInput, new GUILayoutOption[] { GUILayout.Width(80f) });
		GUILayout.EndHorizontal();
	}

	private void DrawItemSpawnPicker(GUIStyle statusStyle)
	{
		List<SpawnableSO> filteredPrefabs = this.GetFilteredItemSpawnPrefabs();
		if (filteredPrefabs.Count == 0)
		{
			GUILayout.Label("No spawnable prefabs match the current filter.", statusStyle, Array.Empty<GUILayoutOption>());
			return;
		}
		this.itemSpawnSelectedIndex = Mathf.Clamp(this.itemSpawnSelectedIndex, 0, filteredPrefabs.Count - 1);
		string[] labels = new string[filteredPrefabs.Count];
		for (int i = 0; i < filteredPrefabs.Count; i++)
		{
			labels[i] = this.GetItemSpawnLabel(filteredPrefabs[i]);
		}
		GUILayout.Label("Prefab", Array.Empty<GUILayoutOption>());
		this.itemSpawnSelectedIndex = GUILayout.SelectionGrid(this.itemSpawnSelectedIndex, labels, 1, Array.Empty<GUILayoutOption>());
		SpawnableSO selectedPrefab = filteredPrefabs[this.itemSpawnSelectedIndex];
		this.DrawItemSpawnDetails(selectedPrefab, statusStyle);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Spawn", new GUILayoutOption[] { GUILayout.Height(28f) }))
		{
			this.SpawnSelectedItem(selectedPrefab);
		}
		if (GUILayout.Button("Spawn At Console Position", new GUILayoutOption[] { GUILayout.Height(28f) }))
		{
			this.SpawnSelectedItemAtConsolePosition(selectedPrefab);
		}
		GUILayout.EndHorizontal();
	}

	private List<SpawnableSO> GetFilteredItemSpawnPrefabs()
	{
		List<SpawnableSO> filteredPrefabs = new List<SpawnableSO>();
		string filter = (this.itemSpawnSearchInput ?? "").Trim();
		for (int i = 0; i < this.itemSpawnAvailablePrefabs.Count; i++)
		{
			SpawnableSO spawnableSO = this.itemSpawnAvailablePrefabs[i];
			if (spawnableSO == null)
			{
				continue;
			}
			if (string.IsNullOrEmpty(filter) || this.GetItemSpawnSearchText(spawnableSO).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				filteredPrefabs.Add(spawnableSO);
			}
		}
		return filteredPrefabs;
	}

	private string GetItemSpawnSearchText(SpawnableSO spawnableSO)
	{
		return string.Concat(new string[]
		{
			spawnableSO.spawnableName ?? "",
			" ",
			spawnableSO.spawnableDescription ?? "",
			" ",
			(spawnableSO.prefab != null) ? spawnableSO.prefab.name : "",
			" ",
			spawnableSO.spawnableID.ToString(CultureInfo.InvariantCulture)
		});
	}

	private string GetItemSpawnLabel(SpawnableSO spawnableSO)
	{
		if (spawnableSO == null)
		{
			return "<null>";
		}
		string name = string.IsNullOrEmpty(spawnableSO.spawnableName) ? spawnableSO.name : spawnableSO.spawnableName;
		return name + " [" + spawnableSO.spawnableID.ToString(CultureInfo.InvariantCulture) + "]";
	}

	private void DrawItemSpawnDetails(SpawnableSO spawnableSO, GUIStyle statusStyle)
	{
		if (spawnableSO == null)
		{
			return;
		}
		GUILayout.Space(6f);
		GUILayout.Label("ID: " + spawnableSO.spawnableID.ToString(CultureInfo.InvariantCulture), Array.Empty<GUILayoutOption>());
		GUILayout.Label("Prefab: " + ((spawnableSO.prefab != null) ? spawnableSO.prefab.name : "missing"), Array.Empty<GUILayoutOption>());
		if (!string.IsNullOrEmpty(spawnableSO.spawnableDescription))
		{
			GUILayout.Label(spawnableSO.spawnableDescription, statusStyle, Array.Empty<GUILayoutOption>());
		}
		Vector3 position;
		if (this.TryGetItemSpawnPosition(out position))
		{
			GUILayout.Label("Spawn position: " + this.FormatTeleportVector(position), Array.Empty<GUILayoutOption>());
		}
		else
		{
			GUILayout.Label("Spawn offset must use numeric X/Y/Z values.", statusStyle, Array.Empty<GUILayoutOption>());
		}
	}

	private void SpawnSelectedItem(SpawnableSO spawnableSO)
	{
		Vector3 position;
		if (!this.TryGetItemSpawnPosition(out position))
		{
			this.itemSpawnStatus = "Spawn offset must use numeric X/Y/Z values.";
			return;
		}
		this.SpawnItem(spawnableSO, position);
	}

	private void SpawnSelectedItemAtConsolePosition(SpawnableSO spawnableSO)
	{
		NewConsole console = this.FindNewConsole();
		if (console == null)
		{
			this.itemSpawnStatus = "NewConsole not found.";
			return;
		}
		try
		{
			MethodInfo method = typeof(NewConsole).GetMethod("GetNextSpawnPos", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				this.itemSpawnStatus = "NewConsole.GetNextSpawnPos was not found.";
				return;
			}
			Vector3 position = (Vector3)method.Invoke(console, null);
			this.SpawnItem(spawnableSO, position);
		}
		catch (Exception ex)
		{
			Exception innerException = ex.InnerException ?? ex;
			this.itemSpawnStatus = "Console spawn position failed: " + innerException.GetType().Name + " - " + innerException.Message;
		}
	}

	private void SpawnItem(SpawnableSO spawnableSO, Vector3 basePosition)
	{
		if (spawnableSO == null)
		{
			this.itemSpawnStatus = "Select an item first.";
			return;
		}
		NewConsole console = this.FindNewConsole();
		if (console == null)
		{
			this.itemSpawnStatus = "NewConsole not found.";
			return;
		}
		int count;
		if (!this.TryParseItemSpawnCount(out count))
		{
			this.itemSpawnStatus = "Count must be a whole number from 1 to 50.";
			return;
		}
		int chipValue;
		if (!int.TryParse((this.itemSpawnChipValueInput ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out chipValue))
		{
			this.itemSpawnStatus = "Chip value must be a whole number.";
			return;
		}
		try
		{
			MethodInfo method = typeof(NewConsole).GetMethod("RequestSpawnPrefab", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				this.itemSpawnStatus = "NewConsole.RequestSpawnPrefab was not found.";
				return;
			}
			for (int i = 0; i < count; i++)
			{
				Vector3 position = basePosition + new Vector3((float)(i % 5) * 0.5f, 0f, (float)(i / 5) * 0.5f);
				method.Invoke(console, new object[] { spawnableSO.spawnableID, position, chipValue });
			}
			this.itemSpawnStatus = "Spawn requested: " + this.GetItemSpawnLabel(spawnableSO) + " x" + count.ToString(CultureInfo.InvariantCulture) + ".";
		}
		catch (Exception ex)
		{
			Exception innerException = ex.InnerException ?? ex;
			this.itemSpawnStatus = "Spawn failed: " + innerException.GetType().Name + " - " + innerException.Message;
		}
	}

	private bool TryGetItemSpawnPosition(out Vector3 position)
	{
		float x;
		float y;
		float z;
		if (!float.TryParse(this.itemSpawnOffsetXInput, NumberStyles.Float, CultureInfo.InvariantCulture, out x) || !float.TryParse(this.itemSpawnOffsetYInput, NumberStyles.Float, CultureInfo.InvariantCulture, out y) || !float.TryParse(this.itemSpawnOffsetZInput, NumberStyles.Float, CultureInfo.InvariantCulture, out z))
		{
			position = Vector3.zero;
			return false;
		}
		GameObject localPlayerObject = this.FindLocalPlayerObject();
		if (localPlayerObject == null)
		{
			position = new Vector3(x, y, z);
			return true;
		}
		position = localPlayerObject.transform.position + localPlayerObject.transform.TransformDirection(new Vector3(x, y, z));
		return true;
	}

	private bool TryParseItemSpawnCount(out int count)
	{
		if (!int.TryParse((this.itemSpawnCountInput ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out count))
		{
			return false;
		}
		count = Mathf.Clamp(count, 1, 50);
		return true;
	}

	private SpawnableSettings itemSpawnableSettings;

	private readonly List<SpawnableSO> itemSpawnAvailablePrefabs = new List<SpawnableSO>();

	private bool itemSpawnInitialized;

	private int itemSpawnSelectedIndex;

	private string itemSpawnSearchInput = "";

	private string itemSpawnOffsetXInput = "0";

	private string itemSpawnOffsetYInput = "1";

	private string itemSpawnOffsetZInput = "2";

	private string itemSpawnCountInput = "1";

	private string itemSpawnChipValueInput = "0";

	private string itemSpawnStatus = "";
}
