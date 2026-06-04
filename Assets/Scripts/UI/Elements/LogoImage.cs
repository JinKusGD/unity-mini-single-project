using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class LogoImage : MonoBehaviour
{
     private RectTransform _rectTransform;
     private RectTransform _parentRect;
     
     private readonly float _peekUpDuration = 0.4f;
     private readonly float _lookAroundDuration = 0.8f;
     private readonly float _squatDuration = 0.3f;
     private readonly float _jumpDuration = 0.5f;
     private readonly float _landDuration = 0.3f;
     
     private readonly float _peekHeightOffset = -170f;
     private readonly float _lookAroundAngle = 8f;
     private readonly float _squatScaleY = 0.7f;

    private float _offScreenY;
    private float _peekY;

    public bool IsFinished { get; private set; }

    private void Awake()
    {
        InitializePositions();
    }

    public void PlayAnimation()
    {
        var cancelToken = this.GetCancellationTokenOnDestroy();
        CatJumpSequenceAsync(cancelToken).Forget();
    }

    private void InitializePositions()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parentRect = transform.parent as RectTransform;

        float parentHeight = _parentRect != null ? _parentRect.rect.height : 1080f;
        float catHeight = _rectTransform.rect.height;

        _offScreenY = -(parentHeight / 2f) - (catHeight / 2f);
        _peekY = -(parentHeight / 2f) + _peekHeightOffset;

        _rectTransform.anchoredPosition = new Vector2(0f, _offScreenY);
        _rectTransform.localScale = Vector3.one;
        _rectTransform.localRotation = Quaternion.identity;

        IsFinished = false;
    }

    private async UniTask CatJumpSequenceAsync(CancellationToken cancellationToken)
    {
        try
        {
            float elapsed = 0f;
            Vector2 startPos = new Vector2(0f, _offScreenY);
            Vector2 peekPos = new Vector2(0f, _peekY);

            while (elapsed < _peekUpDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _peekUpDuration;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                _rectTransform.anchoredPosition = Vector2.Lerp(startPos, peekPos, t);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            _rectTransform.anchoredPosition = peekPos;

            elapsed = 0f;
            while (elapsed < _lookAroundDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _lookAroundDuration;
                float zRot = Mathf.Sin(t * Mathf.PI * 2.5f) * _lookAroundAngle;
                _rectTransform.localRotation = Quaternion.Euler(0, 0, zRot);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            _rectTransform.localRotation = Quaternion.identity;

            elapsed = 0f;
            Vector3 normalScale = Vector3.one;
            Vector3 squatScale = new Vector3(1.2f, _squatScaleY, 1f);
            Vector2 squatPos = peekPos + new Vector2(0f, -30f);

            while (elapsed < _squatDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _squatDuration;
                t = t * t * (3f - 2f * t);
                _rectTransform.localScale = Vector3.Lerp(normalScale, squatScale, t);
                _rectTransform.anchoredPosition = Vector2.Lerp(peekPos, squatPos, t);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            elapsed = 0f;
            Vector3 jumpStretchScale = new Vector3(0.8f, 1.3f, 1f);
            Vector2 centerPos = new Vector2(0f, 0f);

            while (elapsed < _jumpDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _jumpDuration;
                t = t * (2f - t);

                _rectTransform.anchoredPosition = Vector2.Lerp(squatPos, centerPos, t);
                _rectTransform.localScale = Vector3.Lerp(squatScale, jumpStretchScale, t);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            elapsed = 0f;
            Vector3 landSquishScale = new Vector3(1.15f, 0.85f, 1f);
            float landSquishTime = _landDuration * 0.4f;
            while (elapsed < landSquishTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / landSquishTime;
                _rectTransform.localScale = Vector3.Lerp(jumpStretchScale, landSquishScale, t);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            elapsed = 0f;
            float landSettleTime = _landDuration * 0.6f;
            while (elapsed < landSettleTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / landSettleTime;
                float elasticT = Mathf.Sin(t * Mathf.PI * 1.5f) * (1f - t);
                _rectTransform.localScale = Vector3.Lerp(landSquishScale, normalScale, t) + new Vector3(0f, elasticT * 0.1f, 0f);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            _rectTransform.anchoredPosition = centerPos;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.localRotation = Quaternion.identity;
        }
        catch (OperationCanceledException) { }
        finally
        {
            IsFinished = true;
        }
    }
}