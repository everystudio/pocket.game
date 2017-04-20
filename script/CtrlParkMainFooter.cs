﻿using UnityEngine;
using System.Collections;

public class CtrlParkMainFooter : MonoBehaviourEx {

	void Start(){

		#if UNITY_IOS
		if (SystemInfo.deviceModel.Contains ("iPad")) {
			gameObject.transform.localPosition = new Vector3 (gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + 20.0f, gameObject.transform.localPosition.z);
		}
		#endif
	}

	public void TriggerClearAll(){
		m_PageButton.ButtonInit ();
		m_PageButton.TriggerClearAll();

		m_CollectButton.TriggerClear ();
	}

	[SerializeField]
	private ButtonManager m_PageButton;

	[SerializeField]
	private ButtonBase m_CollectButton;

	public void OnPushedBook()
	{
		pushed(0);
	}
	public void OnPushedWork()
	{
		pushed(1);
	}
	public void OnPushedItem()
	{
		pushed(2);
	}
	private void pushed(int _iIndex)
	{
		UIAssistant.main.ShowPage("None");
		SoundManager.Instance.PlaySE(SoundName.BUTTON_PUSH, DataManager.Instance.SOUND_PATH);
		GameMain.STATUS status = GameMain.STATUS.NONE;
		switch (_iIndex)
		{
			case 0:
				SoundManager.Instance.PlayBGM(DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_BOOK), "https://s3-ap-northeast-1.amazonaws.com/every-studio/app/pocket/kimodameshi/ver02/AssetBundles/" + AssetBundles.Utility.GetPlatformName() + "/assets/assetbundles/bgm");
				status = GameMain.STATUS.BOOK;
				break;
			case 1:
				SoundManager.Instance.PlayBGM(DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_WORK), "https://s3-ap-northeast-1.amazonaws.com/every-studio/app/pocket/kimodameshi/ver02/AssetBundles/" + AssetBundles.Utility.GetPlatformName() + "/assets/assetbundles/bgm");
				status = GameMain.STATUS.WORK;
				break;
			case 2:
				SoundManager.Instance.PlayBGM(DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_SHOP), "https://s3-ap-northeast-1.amazonaws.com/every-studio/app/pocket/kimodameshi/ver02/AssetBundles/" + AssetBundles.Utility.GetPlatformName() + "/assets/assetbundles/bgm");
				status = GameMain.STATUS.ITEM;
				break;
			default:
				break;
		}
		GameMain.Instance.SetStatus(status);
		return;
	}

	/*
	// いったん消しておきます
	void Update(){
		if (m_PageButton.ButtonPushed) {
			//Debug.Log (m_PageButton.Index);
			m_PageButton.TriggerClearAll ();

			SoundManager.Instance.PlaySE (SoundName.BUTTON_PUSH , DataManager.Instance.SOUND_PATH);

			GameMain.STATUS status = GameMain.STATUS.NONE;
			switch (m_PageButton.Index) {
			case 0:
				SoundManager.Instance.PlayBGM (DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_BOOK)  , "https://s3-ap-northeast-1.amazonaws.com/every-studio/app/pocket/kimodameshi/ver02/AssetBundles/" + AssetBundles.Utility.GetPlatformName() + "/assets/assetbundles/bgm");
				//SoundManager.Instance.PlayBgmMidi( "sound/midi/bgm" , DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_BOOK) , "sound/midi/bank" , "GMBank.bank" );
				status = GameMain.STATUS.BOOK;
				break;
			case 1:
				status = GameMain.STATUS.WORK;
				SoundManager.Instance.PlayBGM ( DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_WORK)  , "https://s3-ap-northeast-1.amazonaws.com/every-studio/app/pocket/kimodameshi/ver02/AssetBundles/" + AssetBundles.Utility.GetPlatformName() + "/assets/assetbundles/bgm");
				//SoundManager.Instance.PlayBgmMidi( "sound/midi/bgm" , DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_WORK) , "sound/midi/bank" , "GMBank.bank" );
				break;
			case 2:
				SoundManager.Instance.PlayBGM (DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_SHOP)  , "https://s3-ap-northeast-1.amazonaws.com/every-studio/app/pocket/kimodameshi/ver02/AssetBundles/" + AssetBundles.Utility.GetPlatformName() + "/assets/assetbundles/bgm");
				//SoundManager.Instance.PlayBgmMidi( "sound/midi/bgm" , DataManager.Instance.config.Read(DataManager.Instance.KEY_BGM_SHOP) , "sound/midi/bank" , "GMBank.bank" );
				status = GameMain.STATUS.ITEM;
				break;
			default:
				break;
			}
			m_PageButton.TriggerClearAll ();
			GameMain.Instance.SetStatus (status);
		}
	}
	*/

}
