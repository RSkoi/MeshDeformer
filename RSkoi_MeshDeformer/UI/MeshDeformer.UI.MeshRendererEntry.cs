using UnityEngine;
using UnityEngine.UI;

namespace RSkoi_MeshDeformer.UI
{
    public class MeshDeformerEntry
    {
        public GameObject container;
        public Toggle toggle;
        public Text label;
        public Transform target;

        public MeshDeformerEntry(GameObject container, Toggle toggle, Text label) {
            this.container = container;
            this.toggle = toggle;
            this.label = label;
        }

        public MeshDeformerEntry(GameObject container, Toggle toggle, Text label, Transform target)
        {
            this.container = container;
            this.toggle = toggle;
            this.label = label;
            this.target = target;
        }

        public new string ToString()
        {
            return $"MeshDeformerEntry[ " +
                (container != null ? $"container: {container.name}, " : "") +
                (toggle != null ? $"toggle: {toggle.transform.name}, " : "") +
                (label != null ? $"text: {label.text}, " : "") +
                (target != null ? $"target: {target.name}" : "") + " ]";
        }
    }
}
