using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Defense : MonoBehaviour {

	[SerializeField]
	Collider2D _shield;
	[SerializeField]
	Collider2D _sword;

	[SerializeField]
	GUIStyle _myGUI;

	private int _damage;
	private int _defense;
	private float _timeAttak =0;
	private float _blockTimeAttak =0;

	public bool _swiping;
	public bool _attk;

	public float _minSwipeDistance;
	public float _maxSwipeDistance;

	public SwipeDirection _direction = SwipeDirection.None;

	public enum SwipeDirection {Right, Left, Up, Down, None}

	private Touch _initialTouch;

	private string _attakMSG;
	private string _defendMSG;

	// Use this for initialization
	void Start () {
		Input.multiTouchEnabled = true;
		_damage = 0;
		_defense = 0;

		_minSwipeDistance =2* Screen.width / 16;
		_maxSwipeDistance =2* Screen.width / 8;
	}

	void OnGUI() {
		GUI.Label (new Rect (Screen.width/5, Screen.height/7, 100, 100), _attakMSG + _damage.ToString(),_myGUI);
		GUI.Label (new Rect (3*Screen.width/5, Screen.height/7, 100, 100), _defendMSG + _defense.ToString(),_myGUI);

		if (GUI.Button (new Rect (7*Screen.width/9, 1, Screen.width/10, Screen.height/6), "Quit", _myGUI)) {
			System.Diagnostics.Process.GetCurrentProcess().Kill();
			Application.Quit ();
		}

	}

	// Update is called once per frame
	void FixedUpdate () {
		_timeAttak += Time.deltaTime;
		print (_timeAttak.ToString ());
		if (_timeAttak >= 4) {
			
			_timeAttak = 0;
			_damage = 0;
			_defense = 0;
			_attakMSG = " ";
			_defendMSG = " ";
		}

		if (Input.touchCount <= 0)
			return;

		foreach (var touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began)
			{
				_initialTouch = touch;
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				var deltaX = touch.position.x - _initialTouch.position.x; //greater than 0 is right and less than zero is left
				var deltaY = touch.position.y - _initialTouch.position.y; //greater than 0 is up and less than zero is down
				var swipeDistance = Mathf.Abs(deltaX) + Mathf.Abs(deltaY);
				var _attakStr = "Атака ";
				var _defensStr = "Защита ";


				if (swipeDistance > _minSwipeDistance && (Mathf.Abs(deltaX) > 0 || Mathf.Abs(deltaY) > 0))
				{
					Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
					RaycastHit2D hit = Physics2D.Raycast(worldPoint, -Vector2.up, 1000);

					if(hit.collider.CompareTag("sword"))
					{
						_attk = true;
						_swiping = true;

						CalculateSwipeDirection(deltaX, deltaY, _attakStr);
						_damage = 5;
					}
					if(hit.collider.CompareTag("shield"))
					{
						_attk = false;
						_swiping = true;

						CalculateSwipeDirection(deltaX, deltaY, _defensStr);	
						_defense = 5;
					}
				}

				if (swipeDistance > _maxSwipeDistance && (Mathf.Abs(deltaX) > 0 || Mathf.Abs(deltaY) > 0))
				{
					Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
					RaycastHit2D hit = Physics2D.Raycast(worldPoint, -Vector2.up, 1000);

					if(hit.collider.CompareTag("sword"))
					{
						_attk = true;
						_swiping = true;

						CalculateSwipeDirection(deltaX, deltaY, _attakStr);

						_damage = 10;
						if (_defense == 5)
							_damage = 15;
						_sword.enabled = false;
						_shield.enabled = false;
						StartCoroutine (LateCall());
					}
					if(hit.collider.CompareTag("shield"))
					{
						_attk = false;
						_swiping = true;

						CalculateSwipeDirection(deltaX, deltaY, _defensStr);	
						_defense = 10;
						_sword.enabled = false;
						_shield.enabled = false;
						StartCoroutine (LateCall());
					}
				}

			}
			else if (touch.phase == TouchPhase.Ended)
			{
				_initialTouch = new Touch();
				_swiping = false;
				_direction = SwipeDirection.None;
			}
			else if (touch.phase == TouchPhase.Canceled)
			{
				_initialTouch = new Touch();
				_swiping = false;
				_direction = SwipeDirection.None;
			}
		}
	}

	IEnumerator LateCall()
	{
		yield return new WaitForSeconds (2);
		_sword.enabled = true;
		_shield.enabled = true;
	}

	void CalculateSwipeDirection(float deltaX, float deltaY, string action)
	{
		bool isHorizontalSwipe = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);

		// horizontal swipe
		if (isHorizontalSwipe)
		{
			//right
			if (deltaX > 0) {
				_direction = SwipeDirection.Right;
				if(_attk)
					_attakMSG =action + "вправо: ";
				if(!_attk)
					_defendMSG =action + "вправо: ";

			}
			//left
			 if (deltaX < 0) {
				_direction = SwipeDirection.Left;
				if(_attk)
					_attakMSG = action+ "влево: ";
				if(!_attk)
					_defendMSG = action+ "влево: ";

			}
		}
		//vertical swipe
		else if (!isHorizontalSwipe)
		{
			//up
			if (deltaY > 0) {
				_direction = SwipeDirection.Up;
				if(_attk)
					_attakMSG = action + "вверх: ";
				if(!_attk)
					_defendMSG = action + "вверх: ";

			}
			//down
			 if (deltaY < 0) {
				_direction = SwipeDirection.Down;
				if(_attk)
					_attakMSG = action + "вниз: ";
				if(!_attk)
					_defendMSG = action + "вниз: ";

			}
		}
		//diagonal swipe
		else
		{
			_swiping = false;
		}
	}


}
