using UnityEngine;
using System.Collections;

public class ModalDialog : UIController {
	[SerializeField] UISprite portraitWidget; //around 60x60
	[SerializeField] UILabel nameWidget;
	[SerializeField] UILabel content;
	
	[SerializeField] UIEventListener clickArea;
	
	private string[] mPages;
	private int mCurPage=0;
	
	public static void Open(string portraitRef, string name, string[] pages) {
		if(UIManager.instance.ModalGetTop() != UIManager.Modal.Dialog) {
			UIManager.instance.ModalOpen(UIManager.Modal.Dialog);
		}
		
		//...
		ModalDialog us = (ModalDialog)UIManager.instance.uis[(int)UIManager.Modal.Dialog].ui;
		
		us.mPages = pages;
		us.mCurPage = 0;
		
		us.portraitWidget.spriteName = portraitRef;
		
		us.nameWidget.text = name;
		
		us.content.text = pages[0];
	}
	
	void OnPageClick(GameObject go) {
		mCurPage++;
		if(mPages == null || mCurPage == mPages.Length) {
			UIManager.instance.ModalCloseTop();
		}
		else {
			content.text = mPages[mCurPage];
		}
	}
		
	void Awake() {
		clickArea.onClick += OnPageClick;
	}
	
	//TODO: fancy open
	
	public override void OnClose() {
		mPages = null;
	}
}
