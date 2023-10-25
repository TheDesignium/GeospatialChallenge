using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRenderDraw : MonoBehaviour {

    private LineRenderer m_linerend = null;
    private bool m_enable = true;

    //----------------------------------
    //  setLines(list<Vector3>)
    //----------------------------------
    public void setLines(List<Vector3> _vertices)
    {
        m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
        m_linerend.positionCount = _vertices.Count;
        for (int i = 0; i < _vertices.Count; i++)
        {
            m_linerend.SetPosition(i, _vertices[i]);
        }
    }

    public void setLines(Vector3[] _vertices) {
        m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
        m_linerend.positionCount = _vertices.Length;
        for (int i = 0; i < _vertices.Length; i++) {
            m_linerend.SetPosition(i, _vertices[i]);
        }
    }

    public void setLines(Vector3 p1, Vector3 p2) {
        m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
        m_linerend.positionCount = 2;
        m_linerend.SetPosition(0, p1);
        m_linerend.SetPosition(1, p2);
    }

    //----------------------------------
    //  setColor
    //----------------------------------

    public void setColor(Color _color)
    {
        m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
        m_linerend.material.color = _color;
        m_linerend.startColor = _color;
        m_linerend.endColor = _color;
    }

    public void setColor(Color _stcolor, Color _endcolor)
    {
        m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
        m_linerend.material.color = _stcolor;
        m_linerend.startColor = _stcolor;
        m_linerend.endColor = _endcolor;
    }


    //----------------------------------
    //  setRect (Rect)
    //----------------------------------
    public void setRect(Rect _rect)
    {
        m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
        m_linerend.positionCount = 4;
        m_linerend.SetPosition(0, _rect.min);
        m_linerend.SetPosition(1, new Vector2(_rect.min.x, _rect.max.y));
        m_linerend.SetPosition(2, _rect.max);
        m_linerend.SetPosition(3, new Vector2(_rect.max.x, _rect.min.y));
    }

    //----------------------------------
    //  setRect (Rect, Color)
    //----------------------------------
    public void setRect(Rect _rect, Color _color)
    {
        m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
        m_linerend.positionCount = 4;
        m_linerend.material.color = _color;
        m_linerend.startColor = _color;
        m_linerend.endColor = _color;
        m_linerend.SetPosition(0, _rect.min);
        m_linerend.SetPosition(1, new Vector2(_rect.min.x, _rect.max.y));
        m_linerend.SetPosition(2, _rect.max);
        m_linerend.SetPosition(3, new Vector2(_rect.max.x, _rect.min.y));
    }

    //----------------------------------
    //  setRect (pos, w, h, Color)
    //----------------------------------
    public void setRect(Vector3 _pos, float _w, float _h, Color _color)
    {
        m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
        m_linerend.positionCount = 4;
        m_linerend.material.color = _color;
        m_linerend.startColor = _color;
        m_linerend.endColor = _color;
        m_linerend.SetPosition(0, new Vector2(_pos.x - _w / 2, _pos.y - _h / 2));
        m_linerend.SetPosition(1, new Vector2(_pos.x - _w / 2, _pos.y + _h / 2));
        m_linerend.SetPosition(2, new Vector2(_pos.x + _w / 2, _pos.y + _h / 2));
        m_linerend.SetPosition(3, new Vector2(_pos.x + _w / 2, _pos.y - _h / 2));
    }

    //----------------------------------
    //  draw Self ( Color)
    //----------------------------------
    public void drawSelf(Color _color) {
        setRect(gameObject.transform.position, gameObject.transform.localScale.x, gameObject.transform.localScale.y, _color);
    }

    //----------------------------------
    //  enable draw rect border
    //----------------------------------
    public void setEnabled(bool _B)
    {
        m_enable = _B;
        if (m_linerend == null)
            m_linerend = GetComponent<LineRenderer>();
        m_linerend.enabled = m_enable;
    }
}
