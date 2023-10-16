using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using System.Threading.Tasks;
using Spine.Unity;
using System.Threading;

public class Character : MonoBehaviour
{
    [System.Serializable]
    enum MovingType
    {
        Idle = 0,
        Forward = 1,
        Backward = 2,
    }

    [System.Serializable]
    enum MovementStopReason
    {
        SegmentEnded = 0,
        Interrupted = 1,
        PathEnded = 2,
    }

    [System.Serializable]
    struct AnimationInfo
    {
        public string AnimationName;
        public MovingType MovingType;
    }

    [SerializeField] private List<Transform> points;
    [SerializeField] private List<AnimationInfo> animationInfos;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    private int currentPointIndex = 0;
    private int? unreachedPointIndex = null;
    private bool isMoving = false;
    private Task<bool> currentTask;
    private CancellationTokenSource cancellationTokenSource;

    private string idleAnimName => animationInfos.Find(anim => anim.MovingType == MovingType.Idle).AnimationName;


    public async void MoveToPoint(int pointIndex)
    {
        if (isMoving)
            return;

        if (pointIndex == currentPointIndex && unreachedPointIndex == null)
            return;

        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = new CancellationTokenSource();

        List<int> pointsIndexes = GetPathToPoint(pointIndex);


        for (int i = 0; i < pointsIndexes.Count; i++)
        {
            isMoving = true;

            var index = pointsIndexes[i];
            var movingType = GetMovingType(pointsIndexes[i], out bool needMirrorChar);
            var animationName = animationInfos.Find(anim => movingType == anim.MovingType).AnimationName;
            var duration = GetTimeToCompleteSegment(pointsIndexes[i]);

            currentTask = MoveTo(index, animationName, needMirrorChar, duration, cancellationTokenSource);

            await currentTask;

            if (i == pointsIndexes.Count - 1 && currentTask.Result == true)
            {
                OnMovementStopped(MovementStopReason.PathEnded, index);
            }
        }
    }

    public void CancelToken()
    {
        cancellationTokenSource?.Cancel();
    }

    private async Task<bool> MoveTo(int pointIndex, string animationName, bool needMirrorChar, float duration, CancellationTokenSource canc)
    {
        var startTime = Time.time;
        var endTime = Time.time + duration;
        Vector3 startPos = this.transform.position;
        Vector3 finishPos = points[pointIndex].position;

        this.transform.localScale = needMirrorChar ? new Vector3(-1, 1, 1) : Vector3.one;
        skeletonAnimation.AnimationName = animationName;
        skeletonAnimation.loop = true;

        while (endTime > Time.time)
        {
            if (canc.IsCancellationRequested)
            {
                OnMovementStopped(MovementStopReason.Interrupted, pointIndex);
                return false;
            }
            this.transform.position = Vector3.Lerp(startPos, finishPos, (Time.time - startTime) / duration);
            await Task.Yield();
        }

        OnMovementStopped(MovementStopReason.SegmentEnded, pointIndex);
        return true;
    }

    private MovingType GetMovingType(int targetPointIndex, out bool needMirrorCharacter)
    {
        Vector3 indexPos = points[targetPointIndex].position;
        Vector3 result = indexPos - transform.position;

        needMirrorCharacter = result.z * result.x < 0;

        if (result.z >= 0)
        {
            return MovingType.Forward;
        }
        else
        {
            return MovingType.Backward;
        }
    }

    private float GetTimeToCompleteSegment(int targetPointIndex)
    {
        return (this.transform.position - points[targetPointIndex].position).magnitude / movementSpeed;
    }

    private List<int> GetPathToPoint(int pointIndex)
    {
        List<int> result = new List<int>();
        float realCurrentPos = currentPointIndex;
        if (unreachedPointIndex.HasValue)
        {
            realCurrentPos = (unreachedPointIndex.Value + realCurrentPos) / 2;
        }

        if (pointIndex > realCurrentPos)
        {
            for (int i = Mathf.FloorToInt(realCurrentPos + 1); i <= pointIndex; i++)
            {
                result.Add(i);
            }
        }
        else
        {
            for (int i = Mathf.CeilToInt(realCurrentPos - 1); i >= pointIndex; i--)
            {
                result.Add(i);
            }
        }

        return result;
    }

    private void OnMovementStopped(MovementStopReason reason, int pointIndex)
    {
        switch(reason)
        {
            case MovementStopReason.SegmentEnded:
                isMoving = false;
                currentPointIndex = pointIndex;
                unreachedPointIndex = null;
                break;
            case MovementStopReason.Interrupted:
                isMoving = false;
                unreachedPointIndex ??= pointIndex;
                skeletonAnimation.AnimationName = idleAnimName;
                break;
            case MovementStopReason.PathEnded:
                skeletonAnimation.AnimationName = idleAnimName;
                break;
        }    
    }
}
