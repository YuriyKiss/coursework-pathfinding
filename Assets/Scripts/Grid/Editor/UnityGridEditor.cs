using UnityEditor;
using UnityEngine;

namespace ThetaStar.Grid.Generator
{
    [CustomEditor(typeof(UnityGrid))]
    public class UnityGridEditor : Editor
    {
        private UnityGrid _grid;

        private void Awake()
        {
            _grid = (UnityGrid)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Clear Grid"))
            {
                _grid.Clear();
            }
        }
    }
}
