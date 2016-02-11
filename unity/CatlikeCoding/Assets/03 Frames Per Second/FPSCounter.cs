using UnityEngine;

public class FPSCounter : MonoBehaviour {
    public int AverageFPS { get; private set; }
    public int HighestFPS { get; private set; }
    public int LowestFPS { get; private set; }

    public int frameRange;

    int[] _fpsBuffer;
    int _fpsBufferIndex;

    void Update() {
        if(_fpsBuffer == null || _fpsBuffer.Length != frameRange)
            InitializeBuffer();

        UpdateBuffer();
        CalculateFPS();
    }

    void InitializeBuffer() {
        frameRange = Mathf.Max(1, frameRange);
        _fpsBuffer = new int[frameRange];
        _fpsBufferIndex = 0;
    }

    void UpdateBuffer() {
        _fpsBuffer[_fpsBufferIndex] = (int)(1f / Time.unscaledDeltaTime);
        _fpsBufferIndex = (_fpsBufferIndex + 1) % _fpsBuffer.Length;
    }

    void CalculateFPS() {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;

        for(int i = 0; i < frameRange; i++) {
            int fps = _fpsBuffer[i];

            sum += fps;

            highest = Mathf.Max(highest, fps);
            lowest = Mathf.Min(lowest, fps);
        }

        AverageFPS = (int)((float)sum / frameRange);
        HighestFPS = highest;
        LowestFPS = lowest;
    }
}
