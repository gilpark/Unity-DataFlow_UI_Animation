using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CPoint
{

    private FloatReactiveProperty _speed;
    private float _speedMul;
    private int _x,_y=0;
    private int _maxX, _maxY;
    private float _idx;
    private Cell[][] _cells;
	
    public CPoint(int x, int y, FloatReactiveProperty spped, Cell[][] cells)
    {
        _x = x;
        _y = y;
        _cells = cells;
        _maxY = cells.Length;
        _maxX = cells[0].Length;
        _speedMul = Random.Range(1f, 10f);
        _speed = spped;
    }

    public void UpdateX()
    {
        _idx += Time.deltaTime * _speedMul * _speed.Value;
        var idx = (int)_idx % _maxX;
        var a = Random.Range(0, _speedMul*0.1f);
        _cells[_y][idx].AddAlpha(a);		
    }
	
    public void UpdateY()
    {
        _idx += Time.deltaTime * _speedMul * _speed.Value;
        var idx = (int)_idx % _maxY;
        var a = Random.Range(0, _speedMul*0.1f);
        _cells[idx][_x].AddAlpha(a);		
    }

}

public class Cell
{
    private int _x, _y;
    private float _alpha = 0;
    private Tilemap _tilemap;
    private Vector3Int _pos;
    private Color _color = Color.white;
    private FloatReactiveProperty _fadingSpeed;
	
    public Cell(int x, int y, Tilemap tilemap, FloatReactiveProperty fadingSpeed)
    {
        _x = x;
        _y = y;
        _pos = new Vector3Int(_x,_y,0);
        _tilemap = tilemap;
        _color.a = 0;
        _fadingSpeed = fadingSpeed;
    }

    public void SetAlpha(float alpah)
    {
        _alpha = alpah;
    }

    public void AddAlpha(float alpah)
    {
        if(_alpha < 1)
            _alpha += alpah;
    }
	
    private void Set(float alpah)
    {
        _color.a = alpah;
        _tilemap.SetTileFlags(_pos,TileFlags.None);
        _tilemap.SetColor(_pos,_color);
    }

    public void Display()
    {
        if(_alpha <= 0)return;
        _alpha -= Time.deltaTime * _fadingSpeed.Value;

        Set(_alpha);

    }
}