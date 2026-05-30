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
				using (global::System.Drawing.Bitmap bitmap = new global::System.Drawing.Bitmap(fullPath))
				{
					Texture2D texture2D = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.RGBA32, false);
					Color32[] array = new Color32[bitmap.Width * bitmap.Height];
					for (int i = 0; i < bitmap.Height; i++)
					{
						for (int j = 0; j < bitmap.Width; j++)
						{
							global::System.Drawing.Color pixel = bitmap.GetPixel(j, bitmap.Height - 1 - i);
							array[i * bitmap.Width + j] = new Color32(pixel.R, pixel.G, pixel.B, pixel.A);
						}
					}
					texture2D.SetPixels32(array);
					texture2D.Apply(false, false);
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
			gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
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



