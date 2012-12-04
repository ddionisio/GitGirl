using UnityEngine;
using System.Collections;

public class ModalConfirm : UIController {
	public delegate void OnConfirm(bool yes);
	
	[SerializeField] UILabel title;
	[SerializeField] UILabel message;
	
	[SerializeField] UIEventListener buttonYes;
	[SerializeField] UIEventListener buttonNo;
	
	[SerializeField] NGUILayoutFlow flowLayout;
	[SerializeField] NGUILayoutAnchor panelAnchor;
	
	private OnConfirm mConfirmCallback = null;
	private string mDefaultTitle;
	private bool mRefresh = false;
	
	public static void Open(string aTitle, string aMessage, OnConfirm callback) {
		UIManager.instance.ModalOpen(UIManager.Modal.Confirm);
		
		//...
		ModalConfirm us = (ModalConfirm)UIManager.instance.uis[(int)UIManager.Modal.Confirm].ui;
		
		us.title.text = !string.IsNullOrEmpty(aTitle) ? aTitle : us.mDefaultTitle;
		us.message.text = aMessage;
		us.mConfirmCallback = callback;
	}

	void OnButtonYes(GameObject go) {
		OnConfirm callme = mConfirmCallback;
		UIManager.instance.ModalCloseTop();
		
		if(callme != null) {
			callme(true);
		}
	}
	
	void OnButtonNo(GameObject go) {
		OnConfirm callme = mConfirmCallback;
		UIManager.instance.ModalCloseTop();
		
		if(callme != null) {
			callme(false);
		}
	}
	
	public override void OnShow (bool show) {
		mRefresh = show;
	}
	
	public override void OnClose() {
		mConfirmCallback = null;
	}

	void Awake() {
		buttonYes.onClick += OnButtonYes;
		buttonNo.onClick += OnButtonNo;
		
		mDefaultTitle = title.text;
	}
	
	void LateUpdate() {
		if(mRefresh) {
			flowLayout.Reposition();
			panelAnchor.Reposition();
			mRefresh = false;
		}
	}
}
