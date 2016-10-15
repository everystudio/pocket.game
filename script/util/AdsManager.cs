﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NendUnityPlugin.AD;

public class AdsManager : Singleton<AdsManager> {

	[SerializeField]
	private GameObject m_goAdIcon;
	[SerializeField]
	private GameObject m_goAdBanner;
	[SerializeField]
	private List<GameObject> m_goAdNativePanelList;
	private int m_iAdNativePanelIndex;

	#if UNITY_ANDROID
	//private NendAdIcon m_nendAdIcon;
	//private bool m_bIsIcon = true;
	#endif
	private NendAdBanner m_nendAdBanner;


	#if UNITY_IPHONE
	public const string ASSET_BUNDLE_PREFIX             = "iphone";
	public const string ASSET_BUNDLES_ROOT              = "AssetBundles/iOS";
	#elif UNITY_ANDROID
	public const string ASSET_BUNDLE_PREFIX             = "android";
	public const string ASSET_BUNDLES_ROOT              = "AssetBundles/Android";
	#endif

	public static readonly int START_GOLD = 300;
	#if UNITY_IPHONE
	//iOS
	public static readonly string IMOBILE_PID = "34367";
	public static readonly string IMOBILE_MID = "215443";
	public static readonly string IMOBILE_SID_ICON = "371577";		// 使ってないけどね
	public static readonly string IMOBILE_SID_BANNER = "622054";
	//public static readonly string IMOBILE_SID_RECT = "391442";
	#elif UNITY_ANDROID
	public static readonly string IMOBILE_PID = "34367";
	public static readonly string IMOBILE_MID = "247749";
	public static readonly string IMOBILE_SID_ICON = "760610";
	public static readonly string IMOBILE_SID_BANNER = "760609";
	public static readonly string IMOBILE_SID_RECT = "412437";
#endif

	[SerializeField]
	private Image m_imgBannerDummy;

	public void CallInterstitial(){
		// 通常表示
		NendAdInterstitial.Instance.Show( DataManager.Instance.config.Read(DataManager.Instance.KEY_INTERSTENTIAL_GAME_STOPID) );
	}
	

	public override void Initialize ()
	{
#if UNITY_EDITOR
		if (m_imgBannerDummy == null)
		{
			m_imgBannerDummy = GameObject.Find("CanvasNendAd").GetComponent<ImageHolder>().image;
		}
		m_imgBannerDummy.gameObject.SetActive(true);
#else
		/*
		//if( m_imgBannerDummy != null )
		{
			//m_imgBannerDummy.gameObject.SetActive(false);
		}
		*/
#endif
		if (m_nendAdBanner == null) {
			m_nendAdBanner = m_goAdBanner.GetComponent<NendAdBanner> ();
		}
		// 最初はでないようにする
		foreach (GameObject obj in m_goAdNativePanelList) {
			if (obj != null) {
				obj.SetActive (false);
			}
		}
		m_iAdNativePanelIndex = m_goAdNativePanelList.Count-1;
		if (m_goAdNativePanelList [m_iAdNativePanelIndex] != null) {
			m_goAdNativePanelList [m_iAdNativePanelIndex].SetActive (false);
		}
	}

#if USE_IMOBILE
	private int m_iIMobileBannerId = 0;
#endif

	public int m_iShowBannerLock;
	public void ShowAdBanner( bool _bFlag ){

		if (_bFlag) {
			if (0 < m_iShowBannerLock)
			{
				m_iShowBannerLock -= 1;
			}
			if (m_iShowBannerLock == 0)
			{
				m_nendAdBanner.Show();
#if UNITY_EDITOR
				m_imgBannerDummy.gameObject.SetActive(true);
#endif
			}
		} else {
			if(m_iShowBannerLock < 0)
			{
				m_iShowBannerLock = 0;
			}
			m_iShowBannerLock += 1;
			m_nendAdBanner.Hide ();
#if UNITY_EDITOR
			m_imgBannerDummy.gameObject.SetActive(false);
#endif
		}

		Debug.LogError(string.Format("ShowAdBanner:{0} Lock{1}", _bFlag, m_iShowBannerLock));

	}

#if USE_IMOBILE
	static private int m_iIMobileIconId = 0;
#endif
	public void ShowIcon( bool _bFlag ){

		int[] prob_table = new int[2]{
			DataManager.Instance.config.HasKey( "nativead_1_prob" ) ? DataManager.Instance.config.ReadInt( "nativead_1_prob" ) : 0 ,
			DataManager.Instance.config.HasKey( "nativead_2_prob" ) ? DataManager.Instance.config.ReadInt( "nativead_2_prob" ) : 0 				
		};
		int buf = 0;
		foreach (int temp in prob_table) {
			buf += temp;
		}
		if (buf == 0) {
			return;
		}

		if (_bFlag == true) {
			m_iAdNativePanelIndex = UtilRand.GetIndex (prob_table);
			m_iAdNativePanelIndex %= m_goAdNativePanelList.Count;
		}
		if (m_goAdNativePanelList [m_iAdNativePanelIndex] != null) {
			m_goAdNativePanelList [m_iAdNativePanelIndex].SetActive (_bFlag);
		}
		return;
	}

	// Use this for initialization
	void Start () {
		//Debug.LogError ("AdsManager Start");
		/*
		#if UNITY_IPHONE
		NendAdInterstitial.Instance.Load("46ee0b186cac0cbb2681ab10f6ec1de605e72b14", "562605");
		#elif UNITY_ANDROID
		NendAdInterstitial.Instance.Load("5ac03f9f1f7b354bfdfb0f423ba6696a694ad27c", "554786");
		#else
		...
		#endif
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
