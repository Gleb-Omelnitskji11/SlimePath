using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class GateController : MonoBehaviour
    {
        private const float OpenDuration = 0.8f;
        private const float GateHeight = 3.5f;
        private const float GateDepth = 0.4f;
        private const float OpenAngle = 100f;

        private Transform _leftHinge;
        private Transform _rightHinge;
        private bool _isOpen;

        public static GateController Create(Transform parent, float positionZ, float playerStartRadius)
        {
            var gateObject = new GameObject("Gate");
            gateObject.transform.SetParent(parent, false);

            var gate = gateObject.AddComponent<GateController>();
            gate.Initialize(positionZ, playerStartRadius);
            return gate;
        }

        public void Open()
        {
            if (_isOpen) return;

            _isOpen = true;
            StartCoroutine(OpenRoutine());
        }

        private void Initialize(float positionZ, float playerStartRadius)
        {
            float gateWidth = playerStartRadius * 4f + 1f;
            float leafWidth = gateWidth * 0.5f;

            _leftHinge = CreateHinge("Left Hinge", -leafWidth, positionZ);
            _rightHinge = CreateHinge("Right Hinge", leafWidth, positionZ);
            CreateLeaf("Left Leaf", _leftHinge, leafWidth * 0.5f, leafWidth);
            CreateLeaf("Right Leaf", _rightHinge, -leafWidth * 0.5f, leafWidth);
        }

        private Transform CreateHinge(string hingeName, float x, float positionZ)
        {
            var hinge = new GameObject(hingeName).transform;
            hinge.SetParent(transform, false);
            hinge.localPosition = new Vector3(x, GateHeight * 0.5f, positionZ);
            return hinge;
        }

        private static void CreateLeaf(string leafName, Transform hinge, float centerX, float width)
        {
            var leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(leaf.GetComponent<Collider>());
            leaf.name = leafName;
            leaf.transform.SetParent(hinge, false);
            leaf.transform.localPosition = new Vector3(centerX, 0f, 0f);
            leaf.transform.localScale = new Vector3(width, GateHeight, GateDepth);
            leaf.GetComponent<Renderer>().material.color = new Color(0.6f, 0.2f, 0.2f);
        }

        private IEnumerator OpenRoutine()
        {
            Quaternion leftClosed = _leftHinge.localRotation;
            Quaternion rightClosed = _rightHinge.localRotation;
            Quaternion leftOpen = Quaternion.Euler(0f, -OpenAngle, 0f);
            Quaternion rightOpen = Quaternion.Euler(0f, OpenAngle, 0f);

            for (float elapsed = 0f; elapsed < OpenDuration; elapsed += Time.deltaTime)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / OpenDuration);
                _leftHinge.localRotation = Quaternion.Slerp(leftClosed, leftOpen, t);
                _rightHinge.localRotation = Quaternion.Slerp(rightClosed, rightOpen, t);
                yield return null;
            }

            _leftHinge.localRotation = leftOpen;
            _rightHinge.localRotation = rightOpen;
        }
    }
}
