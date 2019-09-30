using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phyllotaxis : MonoBehaviour
{
    public AudioPeer _audioPeer;
    private Material _trailMat;
    public Color _trailcolor;

    public float _degree, _scale;
    public int _numberStart;
    public int _stepSize;
    public int _maxIteration;

    //lerping
    public bool _useLerping;
    private bool _isLerping;
    private Vector3 _startPosition, _endPosition;
    private float _lerpPosTimer, _lerpPosSpeed;
    public Vector2 _lerpPosSpeedMinMax;
    public AnimationCurve _lerpPosAnimCurve;
    public int _lerpPosBand;

    private int _number;
    private int _currentIteration;
    private TrailRenderer _trailRenderer;

    private Vector2 CalculatePhyllotaxis(float degree, float scale, int number)
    {
        double angle = number * (degree * Mathf.Deg2Rad);
        float r = scale * Mathf.Sqrt(number);
        float x = r * (float)System.Math.Cos(angle);
        float y = r * (float)System.Math.Sin(angle);
        Vector2 vec2 = new Vector2(x, y);
        return vec2;
    }

    private Vector2 _phyllotaxisPosition;

    void SetLerpPositions()
    {
        _phyllotaxisPosition = CalculatePhyllotaxis(_degree, _scale, _number);
        _startPosition = this.transform.localPosition;
        _endPosition = new Vector3(_phyllotaxisPosition.x, _phyllotaxisPosition.y, 0);
    }

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailMat = new Material(_trailRenderer.material);
        _trailMat.SetColor("_TintColor", _trailcolor);
        _trailRenderer.material = _trailMat;
        _number = _numberStart;
        transform.localPosition = CalculatePhyllotaxis(_degree, _scale, _number);
        if (_useLerping)
        {
            _isLerping = true;
            SetLerpPositions();
        }
    }

    private void Update()
    {
        if (_useLerping)
        {
            if (_isLerping)
            {
                _lerpPosSpeed = Mathf.Lerp(_lerpPosSpeedMinMax.x, _lerpPosSpeedMinMax.y, _lerpPosAnimCurve.Evaluate(_audioPeer._audioBand[_lerpPosBand]));
                _lerpPosTimer += Time.deltaTime * _lerpPosSpeed;
                transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, Mathf.Clamp01(_lerpPosTimer));
                if (_lerpPosTimer >= 1)
                {
                    _lerpPosTimer -= 1;
                    _number += _stepSize;
                    _currentIteration++;
                    SetLerpPositions();
                }
       
            }
        }
        if (!_useLerping)
        {
            _phyllotaxisPosition = CalculatePhyllotaxis(_degree, _scale, _number);
            transform.localPosition = new Vector3(_phyllotaxisPosition.x, _phyllotaxisPosition.y, 0);
            _number += _stepSize;
            _currentIteration++;
        }
    }

    //private void FixedUpdate()
    //{
    //    if (_useLerping)
    //    {
    //        if(_isLerping)
    //        {
    //            float timeSinceStarted = Time.time - _timeStartedLerping;
    //            float percentageComplete = timeSinceStarted / _intervalLerp;
    //            transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, percentageComplete);
    //            if(percentageComplete >= 0.97f)
    //            {
    //                transform.localPosition = _endPosition;
    //                _number += _stepSize;
    //                _currentIteration++;
    //                if (_currentIteration <= _maxIteration)
    //                {
    //                    StartLerping();
    //                } else
    //                {
    //                    _isLerping = false;
    //                }
    //            }
    //        }
    //    } else
    //    {
    //        _phyllotaxisPosition = CalculatePhyllotaxis(_degree, _scale, _number);
    //        transform.localPosition = new Vector3(_phyllotaxisPosition.x, _phyllotaxisPosition.y, 0);
    //        _number+= _stepSize;
    //        _currentIteration++;
    //    }
    //}

    //// Update is called once per frame
    //void Update()
    //{
    // if (Input.GetKey(KeyCode.Space))
    //    {
    //        _phyllotaxisPosition = CalculatePhyllotaxis(_degree, _scale, _number);
    //        GameObject dotInstance =(GameObject)Instantiate(_dot);
    //        dotInstance.transform.position = new Vector3(_phyllotaxisPosition.x, _phyllotaxisPosition.y, 0);
    //        dotInstance.transform.localScale = new Vector3(_dotScale, _dotScale, _dotScale);
    //        _number++;
    //    }
    //}
}
