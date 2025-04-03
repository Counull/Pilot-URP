using System;
using UnityEngine.Serialization;

namespace Data {
    [Serializable]
    public struct FloatRange {
        public float Min;
        public float Max;

        public FloatRange(float min, float max) {
            Min = min;
            Max = max;
        }

        public float RandomValueInRange() {
            return UnityEngine.Random.Range(Min, Max);
        }
    }
}