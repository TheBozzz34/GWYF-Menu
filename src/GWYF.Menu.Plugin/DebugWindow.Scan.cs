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

	// Token: 0x04001478 RID: 5240
	private string scanOutput = "";

	// Token: 0x04001479 RID: 5241
	private int scanClickCount;
}



