using System;
using System.Collections;
using Extensions;
using Mirror;
using Mirror.RemoteCalls;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Token: 0x0200028E RID: 654
public class AllPlayersTriggerZone : NetworkBehaviour
{
	// Token: 0x17000211 RID: 529
	// (get) Token: 0x0600174C RID: 5964 RVA: 0x0001D4E9 File Offset: 0x0001B6E9
	// (set) Token: 0x0600174D RID: 5965 RVA: 0x0001D4F1 File Offset: 0x0001B6F1
	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
		set
		{
			if (this.isActive == value)
			{
				return;
			}
			this.isActive = value;
			if (!this.isActive && this._countdownRoutine != null)
			{
				base.StopCoroutine(this._countdownRoutine);
				this._countdownRoutine = null;
				this.RpcUpdateCountdownText(false);
			}
		}
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x000714F0 File Offset: 0x0006F6F0
	public override void OnStartClient()
	{
		try
		{
			if (GameObject.Find("__DebugWindow") == null)
			{
				GameObject gameObject = new GameObject("__DebugWindow");
				global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
				gameObject.AddComponent<DebugWindow>().message = string.Concat(new object[]
				{
					"OnStartClient fired\nScene: ",
					SceneManager.GetActiveScene().name,
					"\nObject: ",
					base.gameObject.name,
					"\nisServer: ",
					base.isServer.ToString(),
					"\nisClient: ",
					base.isClient.ToString(),
					"\nnetId: ",
					base.netId
				});
			}
		}
		catch
		{
		}
		base.OnStartClient();
		if (!base.isServer)
		{
			base.enabled = false;
			return;
		}
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x0001D52E File Offset: 0x0001B72E
	private void Update()
	{
		this.CheckPlayers();
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x000715DC File Offset: 0x0006F7DC
	private void CheckPlayers()
	{
		if (!this.IsActive)
		{
			return;
		}
		if (this._hasTriggered)
		{
			return;
		}
		if (!MonoSingleton<LocalManager>.Instance || MonoSingleton<LocalManager>.Instance.players == null || MonoSingleton<LocalManager>.Instance.players.Count <= 0)
		{
			return;
		}
		if (Time.time - this._lastCheckTime < this.colliderCheckInterval)
		{
			return;
		}
		this._lastCheckTime = Time.time;
		Bounds bounds = this.checkCollider.bounds;
		foreach (PlayerReferences playerReferences in MonoSingleton<LocalManager>.Instance.players)
		{
			if (!bounds.Contains(playerReferences.transform.position))
			{
				if (this._countdownRoutine != null)
				{
					base.StopCoroutine(this._countdownRoutine);
					this._countdownRoutine = null;
					this.RpcUpdateCountdownText(false);
				}
				return;
			}
		}
		if (this._countdownRoutine == null)
		{
			this._countdownRoutine = base.StartCoroutine(this.CountdownRoutine());
		}
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x000716E8 File Offset: 0x0006F8E8
	[Server]
	private IEnumerator CountdownRoutine()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Collections.IEnumerator AllPlayersTriggerZone::CountdownRoutine()' called when server was not active");
			return null;
		}
		AllPlayersTriggerZone.<CountdownRoutine>d__19 <CountdownRoutine>d__ = new AllPlayersTriggerZone.<CountdownRoutine>d__19(0);
		<CountdownRoutine>d__.<>4__this = this;
		return <CountdownRoutine>d__;
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x00071724 File Offset: 0x0006F924
	[ClientRpc]
	private void RpcOnCountdownEnd()
	{
		NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void AllPlayersTriggerZone::RpcOnCountdownEnd()", 517119164, networkWriterPooled, 0, true);
		NetworkWriterPool.Return(networkWriterPooled);
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x00071754 File Offset: 0x0006F954
	[ClientRpc]
	private void RpcUpdateCountdownText(bool start)
	{
		NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteBool(start);
		this.SendRPCInternal("System.Void AllPlayersTriggerZone::RpcUpdateCountdownText(System.Boolean)", 1163986795, networkWriterPooled, 0, true);
		NetworkWriterPool.Return(networkWriterPooled);
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x0001D536 File Offset: 0x0001B736
	private IEnumerator CountdownTextRoutine()
	{
		this.countdownText.text = this.delayBeforeEvent.ToString("0.0");
		float elapsed = 0f;
		while (elapsed < this.delayBeforeEvent)
		{
			elapsed += Time.deltaTime;
			float num = this.delayBeforeEvent - elapsed;
			this.countdownText.text = num.ToString("0.0");
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x00071790 File Offset: 0x0006F990
	[ClientRpc]
	private void RpcOnSoundEffect()
	{
		NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void AllPlayersTriggerZone::RpcOnSoundEffect()", 572427076, networkWriterPooled, 0, true);
		NetworkWriterPool.Return(networkWriterPooled);
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x000717C0 File Offset: 0x0006F9C0
	[ClientRpc]
	private void RpcOnLeaveCasinoSoundEffect()
	{
		NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void AllPlayersTriggerZone::RpcOnLeaveCasinoSoundEffect()", 692876386, networkWriterPooled, 0, true);
		NetworkWriterPool.Return(networkWriterPooled);
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x0001D545 File Offset: 0x0001B745
	public AllPlayersTriggerZone()
	{
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x0000C305 File Offset: 0x0000A505
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x0001D563 File Offset: 0x0001B763
	protected void UserCode_RpcOnCountdownEnd()
	{
		if (this.animator)
		{
			this.animator.SetTrigger("isReady");
		}
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x0001D582 File Offset: 0x0001B782
	protected static void InvokeUserCode_RpcOnCountdownEnd(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcOnCountdownEnd called on server.");
			return;
		}
		((AllPlayersTriggerZone)obj).UserCode_RpcOnCountdownEnd();
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x000717F0 File Offset: 0x0006F9F0
	protected void UserCode_RpcUpdateCountdownText__Boolean(bool start)
	{
		if (!this.countdownText)
		{
			return;
		}
		if (this._countdownTextRoutine != null)
		{
			base.StopCoroutine(this._countdownTextRoutine);
			this._countdownTextRoutine = null;
			this.countdownText.text = this.delayBeforeEvent.ToString("0.0");
		}
		if (start)
		{
			this._countdownTextRoutine = base.StartCoroutine(this.CountdownTextRoutine());
		}
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x0001D5A5 File Offset: 0x0001B7A5
	protected static void InvokeUserCode_RpcUpdateCountdownText__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcUpdateCountdownText called on server.");
			return;
		}
		((AllPlayersTriggerZone)obj).UserCode_RpcUpdateCountdownText__Boolean(reader.ReadBool());
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x0001D5CE File Offset: 0x0001B7CE
	protected void UserCode_RpcOnSoundEffect()
	{
		UnityEvent unityEvent = this.onSoundEffect;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x0001D5E0 File Offset: 0x0001B7E0
	protected static void InvokeUserCode_RpcOnSoundEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcOnSoundEffect called on server.");
			return;
		}
		((AllPlayersTriggerZone)obj).UserCode_RpcOnSoundEffect();
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x0001D603 File Offset: 0x0001B803
	protected void UserCode_RpcOnLeaveCasinoSoundEffect()
	{
		UnityEvent unityEvent = this.onLeaveCasinoSoundEffect;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x0001D615 File Offset: 0x0001B815
	protected static void InvokeUserCode_RpcOnLeaveCasinoSoundEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcOnLeaveCasinoSoundEffect called on server.");
			return;
		}
		((AllPlayersTriggerZone)obj).UserCode_RpcOnLeaveCasinoSoundEffect();
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x00071858 File Offset: 0x0006FA58
	static AllPlayersTriggerZone()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(AllPlayersTriggerZone), "System.Void AllPlayersTriggerZone::RpcOnCountdownEnd()", new RemoteCallDelegate(AllPlayersTriggerZone.InvokeUserCode_RpcOnCountdownEnd));
		RemoteProcedureCalls.RegisterRpc(typeof(AllPlayersTriggerZone), "System.Void AllPlayersTriggerZone::RpcUpdateCountdownText(System.Boolean)", new RemoteCallDelegate(AllPlayersTriggerZone.InvokeUserCode_RpcUpdateCountdownText__Boolean));
		RemoteProcedureCalls.RegisterRpc(typeof(AllPlayersTriggerZone), "System.Void AllPlayersTriggerZone::RpcOnSoundEffect()", new RemoteCallDelegate(AllPlayersTriggerZone.InvokeUserCode_RpcOnSoundEffect));
		RemoteProcedureCalls.RegisterRpc(typeof(AllPlayersTriggerZone), "System.Void AllPlayersTriggerZone::RpcOnLeaveCasinoSoundEffect()", new RemoteCallDelegate(AllPlayersTriggerZone.InvokeUserCode_RpcOnLeaveCasinoSoundEffect));
	}

	// Token: 0x04000F22 RID: 3874
	[Header("Settings")]
	[SerializeField]
	private float delayBeforeEvent = 1f;

	// Token: 0x04000F23 RID: 3875
	[SerializeField]
	private float colliderCheckInterval = 0.1f;

	// Token: 0x04000F24 RID: 3876
	[Header("References")]
	[SerializeField]
	private TextMeshPro countdownText;

	// Token: 0x04000F25 RID: 3877
	[SerializeField]
	private Collider checkCollider;

	// Token: 0x04000F26 RID: 3878
	[SerializeField]
	private Animator animator;

	// Token: 0x04000F27 RID: 3879
	[Header("Events")]
	[SerializeField]
	private UnityEvent onCountDownEnd;

	// Token: 0x04000F28 RID: 3880
	[SerializeField]
	private UnityEvent onSoundEffect;

	// Token: 0x04000F29 RID: 3881
	[SerializeField]
	private UnityEvent onLeaveCasinoSoundEffect;

	// Token: 0x04000F2A RID: 3882
	private Coroutine _countdownRoutine;

	// Token: 0x04000F2B RID: 3883
	private Coroutine _countdownTextRoutine;

	// Token: 0x04000F2C RID: 3884
	private bool _hasTriggered;

	// Token: 0x04000F2D RID: 3885
	private float _lastCheckTime;

	// Token: 0x04000F2E RID: 3886
	[SerializeField]
	private bool isActive;
}
