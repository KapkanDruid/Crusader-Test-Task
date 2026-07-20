using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Input;
using Game.Runtime.Ticks;
using R3;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Runtime.UI
{
    [Serializable]
    public class SpeedButtonsView : IDisposable
    {
        [SerializeField] private RectTransform _buttonsGroup;

        private readonly List<Button> _buttons = new();
        private readonly CompositeDisposable _disposable = new();

        private TickService _tickService;
        private InputService _inputService;
        private SpeedButtonsViewConfigComponent _config;

        [Inject]
        private void Construct(TickService tickService, InputService inputService)
        {
            _tickService = tickService;
            _inputService = inputService;
        }

        public void Setup()
        {
            _config = CMSContainer
                .Get(CMSPath.Configs.SpeedButtonsConfig)
                .GetComponent<SpeedButtonsViewConfigComponent>();

            foreach (float speed in _config.SpeedValues.OrderBy(speed => speed))
            {
                AddSpeedButton(speed);
            }

            _inputService.SetSpeed
                .Subscribe(ClickSpeedButton)
                .AddTo(_disposable);
        }

        private void ClickSpeedButton(int index)
        {
            if (index < 0 || index >= _buttons.Count)
                return;

            _buttons[index].onClick.Invoke();
        }

        private void AddSpeedButton(float speed)
        {
            var buttonView = Object.Instantiate(_config.SpeedButtonPrefab, _buttonsGroup);
            var button = buttonView.Button;

            buttonView.SetText("x" + speed.ToString("0.##", CultureInfo.InvariantCulture));
            button.interactable = !Mathf.Approximately(speed, _tickService.Speed);

            button.OnClickAsObservable()
                .Subscribe(_ => SetSpeed(button, speed))
                .AddTo(_disposable);

            _buttons.Add(button);
        }

        private void SetSpeed(Button selectedButton, float speed)
        {
            _tickService.SetSpeed(speed);

            foreach (var button in _buttons)
            {
                button.interactable = true;
            }

            selectedButton.interactable = false;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
