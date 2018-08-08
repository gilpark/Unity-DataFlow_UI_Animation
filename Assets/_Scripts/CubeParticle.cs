using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CubeParticle : MonoBehaviour
{
	
	public bool MoveUp;
	public bool MakeitBig;
	public FloatReactiveProperty GlobalSpeed;
	public Material[] Mats;
	
	private int _age;
	private float _speedMul;
	private readonly Vector2 _distanceMax = new Vector2(83f,-20f);
	private readonly Vector2 _distanceMin = new Vector2(-80f,20f);
	private Vector3 _pos = new Vector3(0,0,0);
	
	void Start ()
	{
		GetComponent<Animator>().speed = Random.Range(1, 3);
		GetComponent<SpriteRenderer>().material = Mats[Random.Range(0, 3)];
		
		_speedMul = Random.Range(1f, 10f);
		_age = (int) Random.Range(500f, 2000f);

		_pos = new Vector3((int)Random.Range(_distanceMin.x,_distanceMax.x),(int)Random.Range(_distanceMin.y,_distanceMax.y),0);		
		var scale = Random.Range(0.5f, 1f);
		if(MakeitBig)scale = Random.Range(1.5f, 2f);
		
		transform.position = _pos;
		transform.localScale = new Vector3(scale, scale, 1);
		var step = 0f;

		Observable.EveryUpdate().Subscribe(_ =>
		{
			step += Time.deltaTime * _speedMul * GlobalSpeed.Value;
			_pos.x = (int)step;
			if(MoveUp)_pos.y = (int)(step * 0.1);
			transform.position = _pos;
			_age--;
			
			if (_pos.x > _distanceMax.x||_age <0)
			{
				_age = (int)Random.Range(500f, 2000f);
				step = (int)Random.Range(_distanceMin.x,_distanceMax.x);
			}
		}).AddTo(this);
	}
	
}
