using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour{

	// Data Members ------------------------------------------
	// Singleton design pattern
	public static GameManager Instance {get; private set;}
	public float maxSelfEsteem{get {return _maxSelfEsteem;}}
	public float selfEsteem{get {return _selfEsteem;}}
	public float maxAnxiety{get {return _maxAnxiety;}}
	public float anxiety{get {return _anxiety;}}
	
	private float _maxSelfEsteem;
	private  float _selfEsteem;
	private float _maxAnxiety;
	private float _anxiety;
	public bool isMasked;
	public float minTint = 0.25f;
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void Start()
	{
		Instance = this;
		_maxSelfEsteem = 100f;
		_selfEsteem = _maxSelfEsteem;
		
		_maxAnxiety = 100f;
		_anxiety = 0f;
		Debug.Log ("Self Esteem: " + selfEsteem);
	}
	// -------------------------------------------------------
	public void Update()
	{
		if(Input.GetAxis ("Cancel") == 1)
		{
			Application.Quit();
		}
	}
	// Mutators --------------------------------------------------------
	public void SetSelfEsteem(float value)
	{
		if(value < _maxSelfEsteem && value > 0)
		{
			_selfEsteem = value;
		}

	}
	// -------------------------------------------------------
	public void AddSelfEsteem(float value)
	{
		if(_selfEsteem < _maxSelfEsteem)
		{
			_selfEsteem = Mathf.Min(_selfEsteem + value, _maxSelfEsteem);
		}

	}
	// -------------------------------------------------------
	public void DamageSelfEsteem(float value)
	{
		if(_selfEsteem > 0)
		{
			// decrease self esteem, ensuring it does not go lower than 0
			_selfEsteem = Mathf.Max (_selfEsteem - value, 0);
		}
		//Debug.Log ("Self Esteem: " + selfEsteem);
	}
	// -------------------------------------------------------
	public void SetAnxiety(float value)
	{
		_anxiety = value;
	}
	// -------------------------------------------------------
	public void AddAnxiety(float value)
	{
		_anxiety += value;
	}
	// -------------------------------------------------------
	public void DamageAnxiety(float value)
	{
		_anxiety -= value;
	}
	// End Mutators ----------------------------------------------------
	// End Methods -------------------------------------------------------------
}
