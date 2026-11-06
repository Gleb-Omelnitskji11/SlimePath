using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace UI
{
    public class BasicPopup : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup CanvasGroup;
        [SerializeField] protected Canvas Canvas;
        [SerializeField] protected float _appearTime = .5f;
        [SerializeField] protected float _closeTime = .3f;
        protected TweenerCore<float, float, FloatOptions> _anim;

        public bool IsOpen => Canvas.enabled;

        public virtual void Open()
        {
            if (_anim?.IsActive() ?? false)
                return;
            Canvas.enabled = true;
            _anim = CanvasGroup.DOFade(1f, _appearTime);
            _anim.onComplete += OnShown;
        }

        protected virtual void OnShown()
        {
            OnShow?.Invoke(this);
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        public virtual void Close()
        {
            if (_anim?.IsActive() ?? false)
                return;

            CanvasGroup.blocksRaycasts = false;
            _anim = CanvasGroup.DOFade(0f, _closeTime);
            _anim.onComplete += OnClosed;
        }
        
        protected virtual void OnClosed()
        {
            Canvas.enabled = false;
            CanvasGroup.interactable = false;

            OnClose?.Invoke(this);
            Destroy(this.gameObject);
        }

        public event Action<BasicPopup> OnShow;
        public event Action<BasicPopup> OnClose;
    }
}