using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BgPresenter : MonoBehaviour
{

	public Tilemap Tilemap;
	public GameObject[] Prefabs;
	public FloatReactiveProperty CubePartilceSpeed = new FloatReactiveProperty(0f);
	public FloatReactiveProperty CubePointSpeed = new FloatReactiveProperty(0f);
	public FloatReactiveProperty FadingSpeed = new FloatReactiveProperty(0f);
	public IntReactiveProperty MaxCpoints = new IntReactiveProperty(150);
	public IntReactiveProperty MaxCParticles = new IntReactiveProperty(15);

	
	private  List<CPoint> _cPoints = new List<CPoint>();
	private List<GameObject> _cubeParticles = new List<GameObject>();
	private readonly Vector4 _bound = new Vector4(-65,-12,64,12);
	private Cell[][] _cells;

	private void AddCubePoints()
	{
		var ypos = (int) Random.Range(0, 130);
		var xpos = (int) Random.Range(0, 25);
			
		var p = new CPoint(ypos,xpos,CubePointSpeed,_cells);
		_cPoints.Add(p);	
	}
	private void AddCubeParticles(int x)
	{
		var c = GameObject.Instantiate(Prefabs[(int) Random.Range(0, 3)]);
		c.transform.parent = gameObject.transform;
		c.layer = 11;
		var cp = c.GetComponent<CubeParticle>();
		cp.GlobalSpeed = CubePartilceSpeed;
		if (x < MaxCParticles.Value*0.3) cp.MakeitBig = true;
		if(x > MaxCParticles.Value - 5) cp.MoveUp = true;
		_cubeParticles.Add(c);
	}

	private void AddCuubeParticle()
	{
		var c = GameObject.Instantiate(Prefabs[(int) Random.Range(0, 3)]);
		c.transform.parent = gameObject.transform;
		c.layer = 11;
		var cp = c.GetComponent<CubeParticle>();
		cp.GlobalSpeed = CubePartilceSpeed;
		_cubeParticles.Add(c);
	}

	private void Start ()
	{
		var yMin = (int) _bound.y;
		var yMax = (int) _bound.w+1;
		var xMin = (int) _bound.x;
		var xMax = (int) _bound.z+1;
		
		var tempCells = new List<List<Cell>>();
		for (int yIndex = yMin; yIndex < yMax; yIndex++)
		{
			var rows = new List<Cell>();
			for (int xIndex = xMin; xIndex < xMax; xIndex++)
			{
				var c = new Cell(xIndex,yIndex,Tilemap,FadingSpeed);
				rows.Add(c);
			}
			tempCells.Add(rows);
		}
		_cells = tempCells.Select(a => a.ToArray()).ToArray();

		
		Observable.Range(0, MaxCpoints.Value).Subscribe(_ => AddCubePoints()).Dispose();
		Observable.Range(0, MaxCParticles.Value).Subscribe(AddCubeParticles).Dispose();


		MaxCpoints.Throttle(TimeSpan.FromMilliseconds(500))
			.Subscribe(len =>
			{
				if (len > _cPoints.Count)
				{
					var add = len - _cPoints.Count;
					Observable.Range(0,add).Subscribe(cp=> AddCubePoints()).Dispose();
				}
				else
				{
					var sub = _cPoints.Count - len;
					Observable.Range(0,sub).Subscribe(cp=> _cPoints.RemoveAt(0)).Dispose();
				}
			}).AddTo(this);

		MaxCParticles.Throttle(TimeSpan.FromMilliseconds(500))
			.Subscribe(size =>
			{
				if (size > _cubeParticles.Count)
				{
					var add = size - _cubeParticles.Count;
					Observable.Range(0,add).Subscribe(cp=> AddCuubeParticle()).Dispose();
				}
				else
				{
					var sub = _cubeParticles.Count - size;
					Observable.Range(0,sub).Subscribe(cp=>
					{
						Destroy(_cubeParticles[0]);
						_cubeParticles.RemoveAt(0);
					}).Dispose();
				}
			}).AddTo(this);
	}

	public int size;
	private void Update()
	{
		size = _cubeParticles.Count;
		_cells.ToList().ForEach(x => x.ToList().ForEach(c => c.Display()));
		for (int i = 0; i < _cPoints.Count; i++)
		{
			if(i < _cPoints.Count * 0.5)_cPoints[i].UpdateX();
			
			else _cPoints[i].UpdateY();
		}
	}

}
