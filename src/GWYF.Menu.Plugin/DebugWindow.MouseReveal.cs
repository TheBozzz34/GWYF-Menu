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
	// Token: 0x06001F60 RID: 8032
	private void Update()
	{
		this.UpdateMouseReveal();
	}

	// Token: 0x06001F61 RID: 8033
	private void OnDisable()
	{
		this.RestoreMouseRevealState();
	}

	// Token: 0x06001F62 RID: 8034
	private void UpdateMouseReveal()
	{
		if (this.IsMouseRevealKeyPressed())
		{
			if (!this.mouseRevealActive)
			{
				this.mouseRevealPreviousVisible = Cursor.visible;
				this.mouseRevealPreviousLockState = Cursor.lockState;
				this.mouseRevealActive = true;
			}
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			return;
		}
		this.RestoreMouseRevealState();
	}

	// Token: 0x06001F63 RID: 8035
	private bool IsMouseRevealKeyPressed()
	{
		if (this.mouseRevealKey == KeyCode.None)
		{
			return false;
		}
		global::UnityEngine.InputSystem.Mouse mouse = global::UnityEngine.InputSystem.Mouse.current;
		switch (this.mouseRevealKey)
		{
		case KeyCode.Mouse0:
			return mouse != null && mouse.leftButton.isPressed;
		case KeyCode.Mouse1:
			return mouse != null && mouse.rightButton.isPressed;
		case KeyCode.Mouse2:
			return mouse != null && mouse.middleButton.isPressed;
		case KeyCode.Mouse3:
			return mouse != null && mouse.backButton.isPressed;
		case KeyCode.Mouse4:
			return mouse != null && mouse.forwardButton.isPressed;
		default:
			global::UnityEngine.InputSystem.Key key;
			if (!this.TryMapToInputSystemKey(this.mouseRevealKey, out key))
			{
				return false;
			}
			global::UnityEngine.InputSystem.Keyboard keyboard = global::UnityEngine.InputSystem.Keyboard.current;
			return keyboard != null && keyboard[key].isPressed;
		}
	}

	// Token: 0x06001F64 RID: 8036
	private bool TryMapToInputSystemKey(KeyCode keyCode, out global::UnityEngine.InputSystem.Key key)
	{
		switch (keyCode)
		{
		case KeyCode.Alpha0:
			key = global::UnityEngine.InputSystem.Key.Digit0;
			return true;
		case KeyCode.Alpha1:
			key = global::UnityEngine.InputSystem.Key.Digit1;
			return true;
		case KeyCode.Alpha2:
			key = global::UnityEngine.InputSystem.Key.Digit2;
			return true;
		case KeyCode.Alpha3:
			key = global::UnityEngine.InputSystem.Key.Digit3;
			return true;
		case KeyCode.Alpha4:
			key = global::UnityEngine.InputSystem.Key.Digit4;
			return true;
		case KeyCode.Alpha5:
			key = global::UnityEngine.InputSystem.Key.Digit5;
			return true;
		case KeyCode.Alpha6:
			key = global::UnityEngine.InputSystem.Key.Digit6;
			return true;
		case KeyCode.Alpha7:
			key = global::UnityEngine.InputSystem.Key.Digit7;
			return true;
		case KeyCode.Alpha8:
			key = global::UnityEngine.InputSystem.Key.Digit8;
			return true;
		case KeyCode.Alpha9:
			key = global::UnityEngine.InputSystem.Key.Digit9;
			return true;
		case KeyCode.Keypad0:
			key = global::UnityEngine.InputSystem.Key.Numpad0;
			return true;
		case KeyCode.Keypad1:
			key = global::UnityEngine.InputSystem.Key.Numpad1;
			return true;
		case KeyCode.Keypad2:
			key = global::UnityEngine.InputSystem.Key.Numpad2;
			return true;
		case KeyCode.Keypad3:
			key = global::UnityEngine.InputSystem.Key.Numpad3;
			return true;
		case KeyCode.Keypad4:
			key = global::UnityEngine.InputSystem.Key.Numpad4;
			return true;
		case KeyCode.Keypad5:
			key = global::UnityEngine.InputSystem.Key.Numpad5;
			return true;
		case KeyCode.Keypad6:
			key = global::UnityEngine.InputSystem.Key.Numpad6;
			return true;
		case KeyCode.Keypad7:
			key = global::UnityEngine.InputSystem.Key.Numpad7;
			return true;
		case KeyCode.Keypad8:
			key = global::UnityEngine.InputSystem.Key.Numpad8;
			return true;
		case KeyCode.Keypad9:
			key = global::UnityEngine.InputSystem.Key.Numpad9;
			return true;
		case KeyCode.LeftControl:
			key = global::UnityEngine.InputSystem.Key.LeftCtrl;
			return true;
		case KeyCode.RightControl:
			key = global::UnityEngine.InputSystem.Key.RightCtrl;
			return true;
		case KeyCode.LeftShift:
			key = global::UnityEngine.InputSystem.Key.LeftShift;
			return true;
		case KeyCode.RightShift:
			key = global::UnityEngine.InputSystem.Key.RightShift;
			return true;
		case KeyCode.LeftAlt:
			key = global::UnityEngine.InputSystem.Key.LeftAlt;
			return true;
		case KeyCode.RightAlt:
			key = global::UnityEngine.InputSystem.Key.RightAlt;
			return true;
		default:
			if (Enum.TryParse<global::UnityEngine.InputSystem.Key>(keyCode.ToString(), true, out key))
			{
				return true;
			}
			key = global::UnityEngine.InputSystem.Key.None;
			return false;
		}
	}

	// Token: 0x06001F63 RID: 8035
	private void RestoreMouseRevealState()
	{
		if (!this.mouseRevealActive)
		{
			return;
		}
		Cursor.visible = this.mouseRevealPreviousVisible;
		Cursor.lockState = this.mouseRevealPreviousLockState;
		this.mouseRevealActive = false;
	}

	// Token: 0x0400152B RID: 5419
	public KeyCode mouseRevealKey = KeyCode.Mouse4;

	// Token: 0x0400152C RID: 5420
	private bool mouseRevealActive;

	// Token: 0x0400152D RID: 5421
	private bool mouseRevealPreviousVisible;

	// Token: 0x0400152E RID: 5422
	private CursorLockMode mouseRevealPreviousLockState;
}



