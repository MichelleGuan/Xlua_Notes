using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class test : MonoBehaviour {
	UnityEvent m_MyEvent;
	
	void Start() {
		if (m_MyEvent == null)
			m_MyEvent = new UnityEvent ();
		
		m_MyEvent.AddListener (Ping);
        m_MyEvent.AddListener(Try);
	}
	
	void Update() {
		if (Input.anyKeyDown && m_MyEvent != null)
		{
			m_MyEvent.Invoke ();
		}
	}
	
	void Ping() {
		Debug.Log ("Ping");
	}
    void Try()
    {
        Debug.Log("Try");
    }
}