using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 게임 내에 팝업에 공통되는 처리를 담당하는 클래스
/// </summary>
public class BasePopupHandler : MonoBehaviour {
    private bool initialized;
    private int lockCount;
    
    private Canvas canvas;
    public Canvas Canvas {
        get {
            if (!initialized) Initialize();
            return canvas;
        }
    }

    private Transform animationRoot;
    protected virtual Transform AnimationRoot {
        get {
            if (!initialized) Initialize();
            return animationRoot;
        }
    }

    private void Initialize() {
        initialized = true;
        animationRoot = transform.Find("Canvas/PopupCommon");
        canvas = GetComponentInChildren<Canvas>(true);
    }

    protected virtual bool UseBlocker => true;             // 배경에 안눌리게 하는 blocker 를 채울건지
    protected virtual bool HandleClickOutside => false;    // 검은배경 눌렀을때 반응 할건지
    protected virtual bool IsPointerDownHandler => true;   // 검은배경 터치 이벤트 타입 (IPointerDownHandler / IPointerClickHandler)
    
    protected virtual bool HandleEscapeKey => true;
    protected virtual float CurtainAlpha => .8f;
    
    private const int BaseSortingOrder = 0;
    protected virtual int SortingGroupBonusOrder => 0;
    protected virtual string PopupSoundEnter => "popup_in";     
    protected virtual string PopupSoundLeave => "popup_out";

    //팝업을 현재 닫을 수 있는 상태인가? (연출 / 튜토리얼 중인지 체크)
    protected virtual bool CanEscape() {
        //if (ScreenLock.IsLock) return false;
        //if (ScreenMultiLock.IsLock) return false;
        //if (!ReferenceEquals(this, PopupUtility.CurrentPopup)) return false;
        return true;
    }

    /// <summary>
    /// 팝업이 띄워지기 전 (AnimationIn 전)
    /// </summary>
    /// <param name="param"></param>
    public virtual void OnBeforeEnter(object param) {
        //if (!string.IsNullOrEmpty(PopupSoundEnter)) SoundManager.Play(PopupSoundEnter);

        int popupCount = PopupManager.GetPopupStackCount();

        if (!ReferenceEquals(null, Canvas))
        {
            Canvas.worldCamera = Camera.main;
            Canvas.sortingOrder = BaseSortingOrder + 1000 * popupCount;
            
            // if (UseBlocker) UIBlocker.Setup(GetInstanceID(), Canvas.sortingLayerName, Canvas.sortingOrder - 1, CurtainAlpha, IsPointerDownHandler);
        }
    }

    /// <summary>
    /// 팝업이 띄워진 후 (AnimationIn 후)
    /// </summary>
    /// <param name="param"></param>
    public virtual void OnAfterEnter(object param) {

    }

    /// <summary>
    /// 팝업이 꺼지기 전 (AnimationOut 전)
    /// </summary>
    public virtual void OnBeforeLeave() {

    }

    /// <summary>
    /// 팝업이 꺼진 후 (AnimtionOut 후)
    /// </summary>
    public virtual void OnAfterLeave() {
        //if (!string.IsNullOrEmpty(PopupSoundLeave)) SoundManager.Play(PopupSoundLeave);
        //if (UseBlocker && !ReferenceEquals(null, Canvas))
        //    UIBlocker.Hide(GetInstanceID());
    }

    protected virtual void OnClickOutside() {
        // if (!HandleClickOutside) return;
        // if (!CanEscape()) return;
        // Scene.CloseImmediate();
    }
    
    public virtual void OnClickEscape() {
        //if (!HandleEscapeKey) return;
        //if (!CanEscape()) return;
        //Scene.CloseImmediate();
    }

    public string GetName() {
        return name;
    }

    public virtual void OnClickClose() {
        PopupManager.ClosePopup();
    }

    public virtual void OnClickCloseReturnBool(bool b) {
        PopupManager.ClosePopup(b);
    }

    public virtual async UniTask AnimationIn()
    {
        if (!AnimationRoot) return;

        var orgScale = Vector3.one;
        var sequence = DOTween.Sequence()
            .Append(AnimationRoot.DOScale(orgScale * 1.02f, .12f).SetEase(Ease.OutQuart).From(orgScale))
            .Append(AnimationRoot.DOScale(orgScale * .99f, .08f).SetEase(Ease.OutQuart))
            .Append(AnimationRoot.DOScale(orgScale, .05f));

        await sequence.AwaitForCompletion();
    }

    public virtual UniTask AnimationOut() {
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// 팝업의 정렬 순서를 설정하는 메서드
    /// </summary>
    /// <param name="sortingOrder">새로운 정렬 순서</param>
    public void AddSortingOrder(int sortingOrder)
    {
        Canvas.sortingOrder = sortingOrder + SortingGroupBonusOrder;
    }

    /// <summary>
    /// 현재 팝업의 정렬 순서를 반환하는 메서드
    /// </summary>
    /// <returns>정렬 순서</returns>
    public int GetSortingOrder()
    {
        return Canvas.sortingOrder;
    }
}
