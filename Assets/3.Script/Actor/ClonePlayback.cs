using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 녹화된 FrameInput 리스트를 재생하는 클론 액터.
/// PlaybackInputProvider가 IInputProvider를 구현하여 Actor에 주입되며,
/// 이동·점프 로직은 Actor.FixedUpdate가 전담한다.
/// </summary>
public class ClonePlayback : Actor
{
    [SerializeField] private bool isLooping;

    public UnityEvent OnPlaybackFinished = new UnityEvent();

    private PlaybackInputProvider _provider;
    private float _elapsedTime;
    private bool _isPlaying;

    protected override void Awake()
    {
        base.Awake();
        _provider = new PlaybackInputProvider();
        SetInputProvider(_provider);
    }

    protected override void FixedUpdate()
    {
        if (!_isPlaying) return;

        _elapsedTime += Time.fixedDeltaTime;

        bool finished = _provider.Advance(_elapsedTime);

        base.FixedUpdate();

        if (!finished) return;

        if (isLooping)
        {
            RestartPlayback();
        }
        else
        {
            _isPlaying = false;
            OnPlaybackFinished.Invoke();
        }
    }

    /// <summary>녹화 데이터를 주입하고 처음부터 재생을 시작한다.</summary>
    public void Play(List<FrameInput> recordedData)
    {
        _provider.SetFrames(recordedData);
        _elapsedTime = 0f;
        _isPlaying = true;
    }

    /// <summary>재생을 즉시 중단한다. OnPlaybackFinished는 발행하지 않는다.</summary>
    public void Stop()
    {
        _isPlaying = false;
    }

    private void RestartPlayback()
    {
        _provider.Rewind();
        _elapsedTime = 0f;
    }

    // ─────────────────────────────────────────────────────────────────────
    // 내부 클래스: 녹화 데이터를 timestamp 기준으로 소비하는 IInputProvider
    // ─────────────────────────────────────────────────────────────────────
    private class PlaybackInputProvider : IInputProvider
    {
        private List<FrameInput> _frames;
        private int _index;
        private FrameInput _current;
        private bool _pendingJump;
        private bool _pendingInteract;

        /// <inheritdoc/>
        public Vector2 MoveDirection => _current.moveDirection;

        /// <inheritdoc/>
        public bool ConsumeJump()
        {
            if (!_pendingJump) return false;
            _pendingJump = false;
            return true;
        }

        /// <inheritdoc/>
        public bool ConsumeInteract()
        {
            if (!_pendingInteract) return false;
            _pendingInteract = false;
            return true;
        }

        /// <summary>프레임 목록을 설정하고 재생 커서를 초기화한다.</summary>
        public void SetFrames(List<FrameInput> frames)
        {
            _frames = frames;
            Rewind();
        }

        /// <summary>재생 커서를 처음으로 되돌린다.</summary>
        public void Rewind()
        {
            _index = 0;
            _current = default;
            _pendingJump = false;
            _pendingInteract = false;
        }

        /// <summary>
        /// elapsedTime까지 도달한 모든 프레임을 순서대로 소비한다.
        /// jump·interact는 OR 누산(한 틱 내에 여러 프레임이 눌렸으면 유지).
        /// </summary>
        /// <returns>모든 프레임 소비 완료 시 true</returns>
        public bool Advance(float elapsedTime)
        {
            if (_frames == null || _frames.Count == 0) return true;

            while (_index < _frames.Count && _frames[_index].timestamp <= elapsedTime)
            {
                _current = _frames[_index];
                if (_current.jumpPressed)    _pendingJump    = true;
                if (_current.interactPressed) _pendingInteract = true;
                _index++;
            }

            return _index >= _frames.Count;
        }
    }
}
