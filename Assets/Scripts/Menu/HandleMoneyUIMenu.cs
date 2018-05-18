using UnityEngine.UI;

using UnityEngine;

public class HandleMoneyUIMenu : MonoBehaviour {

	void OnEnable () {
        GetComponent<Text>().text = DatabaseManager.Db.Money.ToString();
	}

}
