using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
  public class TestEase : MonoBehaviour
  {
    [SerializeField] private Ease type;
    [SerializeField] private Transform mover;
    [SerializeField] private Transform moveFrom;
    [SerializeField] private Transform moveTo;

    private Tweener _tweener;
    private Tween tween;
    private Sequence _sq;

    [Button()]
    private void Move()
    {
      //MoveFunc(type);
      MoveSq(type);
    }
    [Button()]
    private void DOKill()
    {
      tween.Kill();
    }
    [Button()]
    private void DORestart()
    {
      tween.Restart();
    }
    [Button()]
    private void DORewind()
    {
      tween.Rewind();
    }

    private async void MoveFunc(Ease type)
    {
      //mover.DOKill();
      mover.position = moveFrom.position;
      tween = mover.DOMove(moveTo.position, 2f).SetEase(type).OnComplete(() => tween.Rewind());
      //await UniTask.Delay(TimeSpan.FromSeconds(2f));
    }
    private async void MoveSq(Ease type)
    {
      //mover.DOKill();
      mover.position = moveFrom.position;
      _sq = DOTween.Sequence();
      _sq.Append(mover.DOScale(Vector3.zero, 2f));
      _sq.Insert(0,mover.DOMove(moveTo.position, 2f).SetEase(type));
      tween = _sq;
      tween.OnComplete(() =>
      {
        mover.gameObject.SetActive(false);
        tween.Rewind();
      });
      //tween = mover.DOMove(moveTo.position, 2f).SetEase(type).OnComplete(() => tween.Rewind());
      //await UniTask.Delay(TimeSpan.FromSeconds(2f));
    }
  }
}