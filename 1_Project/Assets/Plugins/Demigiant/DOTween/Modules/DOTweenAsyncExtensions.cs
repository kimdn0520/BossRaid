using DG.Tweening;
using System.Threading.Tasks;

public static class DOTweenAsyncExtensions
{
    public static async Task AwaitForCompletion(this Tween tween)
    {
        if (tween == null || !tween.IsActive() || tween.IsComplete())
        {
            return;
        }

        var tcs = new TaskCompletionSource<bool>();
        tween.OnKill(() => tcs.TrySetResult(true));
        tween.OnComplete(() => tcs.TrySetResult(true));

        await tcs.Task;
    }

    public static async Task AwaitForRewind(this Tween tween)
    {
        if (tween == null || !tween.IsActive() || !tween.playedOnce)
        {
            return;
        }

        var tcs = new TaskCompletionSource<bool>();
        tween.OnRewind(() => tcs.TrySetResult(true));
        tween.OnKill(() => tcs.TrySetResult(true));

        await tcs.Task;
    }

    public static async Task AwaitForKill(this Tween tween)
    {
        if (tween == null || !tween.IsActive())
        {
            return;
        }

        var tcs = new TaskCompletionSource<bool>();
        tween.OnKill(() => tcs.TrySetResult(true));

        await tcs.Task;
    }

    public static async Task AwaitForElapsedLoops(this Tween tween, int elapsedLoops)
    {
        if (tween == null || !tween.IsActive() || tween.CompletedLoops() >= elapsedLoops)
        {
            return;
        }

        var tcs = new TaskCompletionSource<bool>();
        tween.OnKill(() => tcs.TrySetResult(true));
        tween.OnStepComplete(() =>
        {
            if (tween.CompletedLoops() >= elapsedLoops)
            {
                tcs.TrySetResult(true);
            }
        });

        await tcs.Task;
    }

    public static async Task AwaitForPosition(this Tween tween, float position)
    {
        if (tween == null || !tween.IsActive() || tween.position >= position)
        {
            return;
        }

        var tcs = new TaskCompletionSource<bool>();
        tween.OnKill(() => tcs.TrySetResult(true));
        tween.OnUpdate(() =>
        {
            if (tween.position >= position)
            {
                tcs.TrySetResult(true);
            }
        });

        await tcs.Task;
    }

    public static async Task AwaitForStart(this Tween tween)
    {
        if (tween == null || !tween.IsActive() || tween.playedOnce)
        {
            return;
        }

        var tcs = new TaskCompletionSource<bool>();
        tween.OnKill(() => tcs.TrySetResult(true));
        tween.OnPlay(() => tcs.TrySetResult(true));

        await tcs.Task;
    }
}
