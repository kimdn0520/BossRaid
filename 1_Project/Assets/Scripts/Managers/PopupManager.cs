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
    /// Popup���� �θ������Ʈ
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
    /// ���� ȭ���� �����ϴ� �˾��� �����Ѵ�. 
    /// </summary>
    private static readonly LinkedList<PopupItem> popupStack = new LinkedList<PopupItem>();

    
    public static int GetPopupStackCount() => popupStack.Count;

    // ���� �� �տ� �����ִ� �˾��� �����´�.
    public static BasePopupHandler CurrentPopup
    {
        get
        {
            if (popupStack.Count == 0) return null;

            return popupStack.Last().popupHandler;
        }
    }

    /// <summary>
    /// �˾��� �ٷ� �����ְ������ ����.
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
            // ShowProcessAsync�� ����Ǹ� done�� true�� ����
            ShowProcessAsync(popupName, param, closeParam =>
            {
                closeCallback?.Invoke(closeParam);  // closeCallback ����
                done = true;  // closeCallback�� ����� �� done�� true�� ����
            }).Forget();

            // done�� true�� �� ������ ���
            await UniTask.WaitUntil(() => done);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during popup show process: {ex.Message}");

            done = true;  // ���ܰ� �߻��ص� done�� true�� �����Ͽ� ��� ����
        }

        await UniTask.WaitUntil(() => done);
    }

    private static async UniTask ShowProcessAsync(string popupName, object param, System.Action<object> closeCallback = null)
    {
        // TODO : ��ũ�� ��

        BasePopupHandler popupInstance = null;

        // PopupName ������ �� BasePopupHandler ��ũ��Ʈ�� ���� �������� ������. 
        var popupPrefab = Resources.Load<BasePopupHandler>(popupName);

        // ���д�.
        popupPrefab.gameObject.SetActive(false);

        // ���� PopupParent ������ �־��ش�.
        popupInstance = Object.Instantiate(popupPrefab, PopupParent.transform);

        // �˾� ���ÿ� �߰��Ѵ�.
        popupStack.AddLast(new PopupItem(popupInstance, closeCallback, param));

        // ���� �ٲ���ٸ� ��ٸ���. (���� �ٲ��� Show)
        // if (PageManager.IsChanging) await UniTask.WaitUntil(() => !PageManager.IsChanging);    

        // OnEnable() -> Start()
        popupPrefab.gameObject.SetActive(true);

        // �˾��� �����ϱ� ��
        popupInstance.OnBeforeEnter(param);

        // �˾��� ������ �ִϸ��̼�
        await popupInstance.AnimationIn();

        // �˾��� ������ ��
        popupInstance.OnAfterEnter(param);

        // TODO : ��ũ�� ���

    }

    /// <summary>
    /// ���ÿ� �ִ� ��� �˾��� ����.
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
    /// ���ÿ� �ִ� �˾��� �̸��� ���� ù��°�� ã�Ƽ� ����.
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
        // �˾������� �������Ÿ� �����´�.
        var popupItem = popupStack.Last();

        popupStack.RemoveLast();

        var popupName = popupItem.popupHandler.GetName();

        await CloseProcessAsync(popupItem, param);
    }

    private static async UniTask CloseProcessAsync(PopupItem popupItem, object param)
    {
        // TODO : ��ũ�� ��

        // �˾��� ������ ��
        popupItem.popupHandler.OnBeforeLeave();

        // �˾��� ������ �ִϸ��̼�
        await popupItem.popupHandler.AnimationOut();

        // �˾��� ���� ��
        popupItem.popupHandler.OnAfterLeave();

        // popup ������Ʈ�� ���� ��Ų��.
        Object.Destroy(popupItem.popupHandler.gameObject);

        // CloseCallback �� �ִٸ� �����Ѵ�.
        popupItem.closeCallback?.Invoke(param);

        // TODO : ��ũ�� ���

        // ���� �ٻ��� 
        if (popupStack.Count == 0)
        {
            // ShowProcessAsync(?);
        }
    }
}
