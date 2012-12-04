using UnityEngine;
using System.Collections;

public class UICommonCallbacks {
	public static void OnButtonClose(GameObject go) {
		UIManager.instance.ModalCloseTop();
	}
}
