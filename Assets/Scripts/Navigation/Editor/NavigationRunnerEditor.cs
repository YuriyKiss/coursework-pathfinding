using UnityEditor;
using UnityEngine;

namespace ThetaStar.Navigation
{
    [CustomEditor(typeof(NavigationRunner))]
    public class NavigationRunnerEditor : Editor
    {
        private NavigationRunner _runner;

        private void Awake()
        {
            _runner = (NavigationRunner)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Compute Path"))
            {
                _runner.ComputePath();
            }
        }
    }
}
