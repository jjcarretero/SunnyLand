﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundLoop : MonoBehaviour
{
	public GameObject[] objs;
	private Camera mainCamera;
	private Vector2 screenBounds;
	public float choke;
	private float timer = 0.5f, Speed = 4;
	public GameObject text;
	private AudioManager audiomanager;

	private void Start()
	{
		mainCamera = gameObject.GetComponent<Camera>();
		audiomanager = AudioManager.instance;
		screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
		foreach (GameObject obj in objs)
			loadChildObjects(obj);
		audiomanager.StopSound("Music");
		audiomanager.PlaySound("MenuMusic");
	}

	void loadChildObjects(GameObject obj)
	{
		float objectWidth = obj.GetComponent<SpriteRenderer>().bounds.size.x - choke;
		int childsNeeded = (int)Mathf.Ceil(screenBounds.x * 2 / objectWidth);
		GameObject clone = Instantiate(obj) as GameObject;
		for (int i = 0; i <= childsNeeded; i++)
		{
			GameObject c = Instantiate(clone) as GameObject;
			c.transform.SetParent(obj.transform);
			c.transform.position = new Vector3(objectWidth * i, obj.transform.position.y, obj.transform.position.z);
			c.name = obj.name + i;
		}
		Destroy(clone);
		Destroy(obj.GetComponent<SpriteRenderer>());
	}

	void repositionChildObjects(GameObject obj)
	{
		Transform[] children = obj.GetComponentsInChildren<Transform>();
		if(children.Length > 1)
		{
			GameObject firstChild = children[1].gameObject;
			GameObject lastChild = children[children.Length - 1].gameObject;
			float halfObjectWidth = lastChild.GetComponent<SpriteRenderer>().bounds.extents.x - choke;
			if(transform.position.x + screenBounds.x > lastChild.transform.position.x + halfObjectWidth)
			{
				firstChild.transform.SetAsLastSibling();
				firstChild.transform.position = new Vector3(lastChild.transform.position.x + halfObjectWidth * 2, lastChild.transform.position.y, lastChild.transform.position.z);
			}
			else if(transform.position.x - screenBounds.x < firstChild.transform.position.x - halfObjectWidth)
			{
				lastChild.transform.SetAsFirstSibling();
				lastChild.transform.position = new Vector3(firstChild.transform.position.x - halfObjectWidth * 2, firstChild.transform.position.y, firstChild.transform.position.z);
			}
		}
	}

	private void LateUpdate()
	{
		foreach (GameObject obj in objs)
		{
			repositionChildObjects(obj);
		}
	}

	private void Update()
	{
		transform.Translate(Vector2.right * Speed * Time.deltaTime);
		BlinkingText();
		if(Input.GetKeyDown(KeyCode.Return))
		{
			audiomanager.PlaySound("Select");
			SceneManager.LoadScene("NivelPrimero");
		}

	}

	public void BlinkingText()
	{
		timer = timer + Time.deltaTime;
		if (timer >= 0.5)
		{
			text.GetComponent<SpriteRenderer>().enabled = true;
		}
		if (timer >= 1)
		{
			text.GetComponent<SpriteRenderer>().enabled = false;
			timer = 0;
		}
	}
}
