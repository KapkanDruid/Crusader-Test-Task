using R3;
using System;
using UnityEngine;

namespace Game.Runtime.Ticks
{
    public sealed class TickService : IDisposable
    {
        private readonly Subject<float> _onTick = new();
        private readonly CompositeDisposable _disposable = new();

        private float _speed = 1f;
        private bool _isPaused;

        public Observable<float> OnTick => _onTick;
        public float Speed => _speed;
        public bool IsPaused => _isPaused;

        public TickService()
        {
            Observable.EveryUpdate()
                .Subscribe(_ => Tick())
                .AddTo(_disposable);
        }

        public void SetSpeed(float speed)
        {
            if (speed < 0f
                || float.IsNaN(speed)
                || float.IsInfinity(speed))
            {
                throw new ArgumentOutOfRangeException(nameof(speed));
            }

            _speed = speed;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        private void Tick()
        {
            if (_isPaused || _speed == 0f)
                return;

            float deltaTime = Time.unscaledDeltaTime * _speed;
            _onTick.OnNext(deltaTime);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
