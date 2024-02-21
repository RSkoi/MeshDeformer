using System;
using MessagePack;

namespace RSkoi_MeshDeformer.Scene
{
    public static class MeshDeformerSerializableObjects
    {
        [Serializable]
        [MessagePackObject]
        public class MeshDeformerTargetOptions(float springForce = 20f, float damping = 5f, float uniformScale = 1f)
        {
            [Key("springForce")]
            public float springForce = springForce;
            [Key("damping")]
            public float damping = damping;
            [Key("uniformScale")]
            public float uniformScale = uniformScale;
        }

        [Serializable]
        [MessagePackObject]
        public class MeshDeformerInputOptions(float force = 0.1f, float forceOffset = 0.2f, float distance = 0.3f)
        {
            [Key("force")]
            public float force = force;
            [Key("forceOffset")]
            public float forceOffset = forceOffset;
            [Key("distance")]
            public float distance = distance;
        }

        [Serializable]
        [MessagePackObject]
        public class TrackerDataContainer
        {
            [Key("parentItemKey")]
            public int parentItemKey;
            [Key("rendererTransformName")]
            public string rendererTransformName;
            [Key("isDisabled")]
            public bool isDisabled;
            [Key("inputOptions")]
            public MeshDeformerInputOptions inputOptions;
            [Key("targetOptions")]
            public MeshDeformerTargetOptions targetOptions;

            public TrackerDataContainer(int parentItemKey, string rendererTransformName, bool isDisabled, MeshDeformerInputOptions inputOptions)
            {
                this.parentItemKey = parentItemKey;
                this.rendererTransformName = rendererTransformName;
                this.isDisabled = isDisabled;
                this.inputOptions = inputOptions;
            }

            public TrackerDataContainer(int parentItemKey, string rendererTransformName, bool isDisabled, MeshDeformerTargetOptions targetOptions)
            {
                this.parentItemKey = parentItemKey;
                this.rendererTransformName = rendererTransformName;
                this.isDisabled = isDisabled;
                this.targetOptions = targetOptions;
            }
        }
    }
}
