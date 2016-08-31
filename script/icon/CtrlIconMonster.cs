﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CtrlIconMonster : CtrlIconBase {

	public int m_iMealLevel;
	public int m_iCleanLevel;

	private void CollectCharge(int _iGold , int _iExp )
	{
		//Debug.LogError(string.Format("exp={0}", _iExp));
		m_iconTamashii.gameObject.SetActive(true);
	}
	private void CollectMonster( int _iMonsterSerial)
	{
		m_iconTamashii.gameObject.SetActive(false);
	}

	protected CtrlIconTamashii m_iconTamashii;
	protected CollectBase m_collectBase;
	public void Initialize( UI2DSprite _sprite , DataMonsterParam _dataMonster , int _iSize , CtrlIconTamashii _iconTamashii , CollectBase _collectBase ){

		m_fukidashi.Initialize();

		m_iconTamashii = _iconTamashii;
		m_collectBase = _collectBase;
		m_collectBase.CollectCharge.AddListener(CollectCharge);
		m_collectBase.CollectMonster.AddListener(CollectMonster);
		SetSize (_iSize);

		myTransform.localPosition = GetMovePos ();
		m_sprIcon = _sprite;
		m_dataMonster = _dataMonster;

		m_eStep = STEP.IDLE;
		m_eStepPre = STEP.MAX;

		AnimationIdol (true);

	}

	public GameObject m_goDust;
	//public GameObject m_goMeal;

	public void createDust (){
		if (m_goDust == null) {
			m_goDust = PrefabManager.Instance.MakeObject ("prefab/PrefDust", gameObject);
			m_goDust.transform.parent = gameObject.transform.parent;
			m_goDust.transform.localPosition = GetMovePos ();
			m_goDust.transform.localPosition *= 0.8f;

			m_goDust.transform.localPosition += 0.2f * (DefineOld.CELL_X_DIR + DefineOld.CELL_Y_DIR);

			m_goDust.GetComponent<UI2DSprite> ().depth = m_sprIcon.depth-DataManager.Instance.DEPTH_MONSTER + DataManager.Instance.DEPTH_DUST;
		}
	}
	override public bool CleanDust(){
		bool bRet = false;
		int iCleanLevel = 0;
		int iMealLevel = 0;
		m_dataMonster.GetConditions (ref iCleanLevel, ref iMealLevel);
		if (m_goDust != null ||  iCleanLevel < 5) {
			bRet = true;
			Destroy (m_goDust);
			m_goDust = null;
			Dictionary< string , string > dict = new Dictionary< string , string > ();
			dict.Add ("clean_time", "\"" + TimeManager.StrGetTime () + "\"");
			DataManager.Instance.dataMonster.Update (m_dataMonster.monster_serial, dict);
			m_dataMonster = DataManager.Instance.dataMonster.Select (m_dataMonster.monster_serial);
		}
		return bRet;
	}

	override public bool Meal(){
		bool bRet = false;
		int iCleanLevel = 0;
		int iMealLevel = 0;
		m_dataMonster.GetConditions (ref iCleanLevel, ref iMealLevel);

		if (iMealLevel < 5) {
			m_eStep = STEP.EAT;

			Dictionary< string , string > dict = new Dictionary< string , string > ();
			dict.Add ("meal_time", "\"" + TimeManager.StrGetTime () + "\"");
			DataManager.Instance.dataMonster.Update (m_dataMonster.monster_serial, dict);
			m_dataMonster = DataManager.Instance.dataMonster.Select (m_dataMonster.monster_serial);
			bRet = true;
		}
		return bRet;
	}

	public int m_iLocalCondition;

	override public void update_idle( bool _bInit ){
		if (_bInit) {
			//Debug.LogError("here");
			// 空腹状態
			// 新しくする
			m_dataMonster = DataManager.Instance.dataMonster.Select (m_dataMonster.monster_serial);

			m_iLocalCondition = -1;

			int iCleanLevel = 0;
			int iMealLevel = 0;
			m_dataMonster.GetConditions (ref iCleanLevel, ref iMealLevel);
			m_iCleanLevel = iCleanLevel;
			m_iMealLevel = iMealLevel;
			
			if (iCleanLevel < 1 && m_dataMonster.condition == (int)DefineOld.Monster.CONDITION.FINE ) {
				Dictionary< string , string > dict = new Dictionary< string , string > ();
				dict.Add ("condition", ((int)(DefineOld.Monster.CONDITION.SICK)).ToString ()); 
				DataManager.Instance.dataMonster.Update (m_dataMonster.monster_serial, dict);
				m_dataMonster = DataManager.Instance.dataMonster.Select (m_dataMonster.monster_serial);
			}

			//if (m_dataMonster.condition == (int)(DefineOld.Monster.CONDITION.SICK) && iCleanLevel == 0) {
			if (m_dataMonster.condition == (int)(DefineOld.Monster.CONDITION.SICK)) {
				m_fukidashi.SetStatus (CtrlIconFukidashi.STATUS.SICK2 ,m_sprIcon.depth);
			//} else if (m_dataMonster.condition == (int)(DefineOld.Monster.CONDITION.SICK) && iCleanLevel == 1) {
			} else if (m_dataMonster.condition == (int)(DefineOld.Monster.CONDITION.SICK) ) {
				m_fukidashi.SetStatus (CtrlIconFukidashi.STATUS.SICK1,m_sprIcon.depth);
			} else if (iCleanLevel < 3 ) {
				m_fukidashi.SetStatus (CtrlIconFukidashi.STATUS.DUST,m_sprIcon.depth);
			} else if (iMealLevel < 3 ) {
				m_fukidashi.SetStatus (CtrlIconFukidashi.STATUS.HUNGRY,m_sprIcon.depth);
			} else {
				if( m_fukidashi.SetStatus (CtrlIconFukidashi.STATUS.NONE,m_sprIcon.depth))
				{
					m_collectBase.ResetCollectTime();
				}
			}

			if (iCleanLevel != 5) {
				createDust ();
			}

			// 待機状態の一定確率で叫ぶ
			if(85 < UtilRand.GetRand(100))
			{
				ScareStart();
			}

		}
	}

	override public void AnimationIdol(bool _bInit){
		if (_bInit) {
			string strName = string.Format( "chara{0:D2}_eat1" ,m_dataMonster.monster_id );
			//m_sprIcon.atlas = AtlasManager.Instance.GetAtlas (strName);
			m_sprIcon.sprite2D = SpriteManager.Instance.Load( string.Format( "texture/monster/{0}.png" , strName ));
			m_sprIcon.width = (int)m_sprIcon.sprite2D.textureRect.width;// + (int)m_sprIcon.sprite2D.textureRectOffset.x;
			m_sprIcon.height = (int)m_sprIcon.sprite2D.textureRect.height;// + (int)m_sprIcon.sprite2D.textureRectOffset.y;
			//m_sprIcon.spriteName = strName;
		}
	}

	override public void AnimationMove(bool _bInit){
		if (_bInit) {
			m_fAnimationInterval = 0.2f;
			m_fAnimationTime = m_fAnimationInterval;
			m_iAnimationFrame = 0;
		}

		m_fAnimationTime += Time.deltaTime;
		if (m_fAnimationInterval < m_fAnimationTime) {
			m_fAnimationTime -= m_fAnimationInterval;

			m_iAnimationFrame += 1;
			m_iAnimationFrame %= 2;		// トータル２フレーム

			int iDispFrame = m_iAnimationFrame + 1;

			string strName = string.Format( "chara{0:D2}_move{1}" , m_dataMonster.monster_id , iDispFrame);
			m_sprIcon.sprite2D = SpriteManager.Instance.Load( string.Format( "texture/monster/{0}.png" , strName ));
			m_sprIcon.width = (int)m_sprIcon.sprite2D.textureRect.width;// + (int)m_sprIcon.sprite2D.textureRectOffset.x;
			m_sprIcon.height = (int)m_sprIcon.sprite2D.textureRect.height;// + (int)m_sprIcon.sprite2D.textureRectOffset.y;
		}
		return;
	}

	private float m_fEatSoundTimer;
	override public void AnimationEat(bool _bInit , int _iMealId ){
		if (_bInit) {
			m_fAnimationTime = 0.0f;
			m_iAnimationFrame = 0;
			m_fAnimationInterval = 0.8f;
			m_fEatSoundTimer = 0.0f;
		}

		m_fEatSoundTimer += Time.deltaTime;
		if (1.0f < m_fEatSoundTimer) {
			m_fEatSoundTimer -= 1.0f;
			SoundManager.Instance.PlaySE ("se_eating" , "https://s3-ap-northeast-1.amazonaws.com/every-studio/app/sound/se");
		}

		m_fAnimationTime += Time.deltaTime;
		if (m_fAnimationInterval < m_fAnimationTime) {
			m_fAnimationTime -= m_fAnimationInterval;

			m_iAnimationFrame += 1;
			m_iAnimationFrame %= 2;		// トータル２フレーム

			int iDispFrame = m_iAnimationFrame + 1;

			string strName = string.Format( "chara{0:D2}_eat{1}", m_dataMonster.monster_id , iDispFrame);
			m_sprIcon.sprite2D = SpriteManager.Instance.Load( string.Format( "texture/monster/{0}.png" , strName ));
			m_sprIcon.width = (int)m_sprIcon.sprite2D.textureRect.width;// + (int)m_sprIcon.sprite2D.textureRectOffset.x;
			m_sprIcon.height = (int)m_sprIcon.sprite2D.textureRect.height;// + (int)m_sprIcon.sprite2D.textureRectOffset.y;
		}
		return;
	}

	public override void AnimationScare(bool _bInit)
	{
		if(_bInit)
		{
			string strName = string.Format("chara{0:D2}_scare", m_dataMonster.monster_id);
			m_sprIcon.sprite2D = SpriteManager.Instance.Load(string.Format("texture/monster/{0}.png", strName));
			m_sprIcon.width = (int)m_sprIcon.sprite2D.textureRect.width;// + (int)m_sprIcon.sprite2D.textureRectOffset.x;
			m_sprIcon.height = (int)m_sprIcon.sprite2D.textureRect.height;// + (int)m_sprIcon.sprite2D.textureRectOffset.y;
		}
	}



}


















