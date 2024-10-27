using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class PopupManager
{
    private class PopupItem
    {
        public readonly BasePopupHandler popupHandler;
        public readonly Action<object> closeCallback;
        public readonly object param;

        public PopupItem(BasePopupHandler popupHandler, Action<object> closeCallback, object param)
        {
            this.popupHandler = popupHandler;
            this.closeCallback = closeCallback;
            this.param = param;
        }
    }

    /// <summary>
    /// Popup들의 부모오브젝트
    /// </summary>
    private static GameObject popupParent;
    private static GameObject PopupParent
    {
        get
        {
            if (popupParent) return popupParent;
            popupParent = new GameObject("Popup Parent");
            Object.DontDestroyOnLoad(popupParent);
            return popupParent;
        }
    }

    /// <summary>
    /// 현재 화면이 등장하는 팝업을 관리한다. 
    /// </summary>
    private static readonly LinkedList<PopupItem> popupStack = new LinkedList<PopupItem>();

    
    public static int GetPopupStackCount() => popupStack.Count;

    // 현재 맨 앞에 열려있는 팝업을 가져온다.
    public static BasePopupHandler CurrentPopup
    {
        get
        {
            if (popupStack.Count == 0) return null;

            return popupStack.Last().popupHandler;
        }
    }

    /// <summary>
    /// 팝업을 바로 보여주고싶을때 쓴다.
    /// </summary>
    /// <param name="popupName"></param>
    /// <param name="param"></param>
    /// <param name="callback"></param>
    public static void ShowPopup(string popupName, object param = null, System.Action<object> callback = null)
    {
        ShowProcessAsync(popupName, param, callback).Forget();
    }

    public static async void ShowAsync(string popupName, object param = null, Action<object> closeCallback = null)
    {
        var done = false;

        try
        {
            // ShowProcessAsync가 종료되면 done을 true로 설정
            ShowProcessAsync(popupName, param, closeParam =>
            {
                closeCallback?.Invoke(closeParam);  // closeCallback 실행
                done = true;  // closeCallback이 실행된 후 done을 true로 설정
            }).Forget();

            // done이 true가 될 때까지 대기
            await UniTask.WaitUntil(() => done);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during popup show process: {ex.Message}");

            done = true;  // 예외가 발생해도 done을 true로 설정하여 대기 종료
        }

        await UniTask.WaitUntil(() => done);
    }

    private static async UniTask ShowProcessAsync(string popupName, object param, System.Action<object> closeCallback = null)
    {
        // TODO : 스크린 락

        BasePopupHandler popupInstance = null;

        // PopupName 프리팹 중 BasePopupHandler 스크립트를 가진 프리팹을 가져옴. 
        var popupPrefab = Resources.Load<BasePopupHandler>(popupName);

        // 꺼둔다.
        popupPrefab.gameObject.SetActive(false);

        // 만들어서 PopupParent 하위로 넣어준다.
        popupInstance = Object.Instantiate(popupPrefab, PopupParent.transform);

        // 팝업 스택에 추가한다.
        popupStack.AddLast(new PopupItem(popupInstance, closeCallback, param));

        // 씬이 바뀌었다면 기다린다. (씬이 바뀐후 Show)
        // if (PageManager.IsChanging) await UniTask.WaitUntil(() => !PageManager.IsChanging);    

        // OnEnable() -> Start()
        popupPrefab.gameObject.SetActive(true);

        // 팝업이 등장하기 전
        popupInstance.OnBeforeEnter(param);

        // 팝업이 꺼지는 애니메이션
        await popupInstance.AnimationIn();

        // 팝업이 등장한 후
        popupInstance.OnAfterEnter(param);

        // TODO : 스크린 언락

    }

    /// <summary>
    /// 스택에 있는 모든 팝업을 끈다.
    /// </summary>
    /// <param name="param"></param>
    /// <param name="destroy"></param>
    public static async void CloseAll(object param = null)
    {
        while (popupStack.Count > 0)
        {
            var popupItem = popupStack.Last();
            var popupName = popupItem.popupHandler.GetName();
            popupStack.RemoveLast();

            await CloseProcessAsync(popupItem, param);
        }
    }

    /// <summary>
    /// 스택에 있는 팝업을 이름을 통해 첫번째만 찾아서 끈다.
    /// </summary>
    /// <param name="popupName"></param>
    /// <param name="param"></param>
    /// <param name="destroy"></param>
    public static async void ClosePopupByName(string popupName, object param = null)
    {
        var popupItem = popupStack.FirstOrDefault(w => w.popupHandler.GetName() == popupName);

        popupStack.Remove(popupItem);

        await CloseProcessAsync(popupItem, param);
    }

    public static async void ClosePopup(object param = null)
    {
        // 팝업스택의 맨위에거를 가져온다.
        var popupItem = popupStack.Last();

        popupStack.RemoveLast();

        var popupName = popupItem.popupHandler.GetName();

        await CloseProcessAsync(popupItem, param);
    }

    private static async UniTask CloseProcessAsync(PopupItem popupItem, object param)
    {
        // TODO : 스크린 락

        // 팝업이 꺼지기 전
        popupItem.popupHandler.OnBeforeLeave();

        // 팝업이 꺼지는 애니메이션
        await popupItem.popupHandler.AnimationOut();

        // 팝업이 꺼진 후
        popupItem.popupHandler.OnAfterLeave();

        // popup 오브젝트를 삭제 시킨다.
        Object.Destroy(popupItem.popupHandler.gameObject);

        // CloseCallback 이 있다면 실행한다.
        popupItem.closeCallback?.Invoke(param);

        // TODO : 스크린 언락

        // 스택 다빼고 
        if (popupStack.Count == 0)
        {
            // ShowProcessAsync(?);
        }
    }
}
