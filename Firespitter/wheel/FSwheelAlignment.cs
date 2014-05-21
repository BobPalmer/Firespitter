﻿using System;
using UnityEngine;

namespace Firespitter.wheel
{
    public class FSwheelAlignment : PartModule
    {
        public bool showGuides = false;
        private GameObject wheel;
        LineRenderer guideLine;
        LineRenderer wheelLine;
        Texture2D guideLineTex;
        Texture2D wheelLineTex;
        int textureSize = 1;
        float lineLength = 5f;

        [KSPField]
        public bool showToggle = true;

        [KSPEvent(guiActiveEditor = true, guiName = "Alignment guide")]
        public void toggleAlignmentGuide()
        {
            showGuides = !showGuides;
            setGuideVisibility(showGuides);
        }

        public override void OnStart(PartModule.StartState state)
        {
            findWheel();

            createTextures();

            Debug.Log("create guideLine");

            guideLine = part.gameObject.GetComponent<LineRenderer>();
            if (guideLine == null)
                guideLine = part.gameObject.AddComponent<LineRenderer>();
            guideLine.SetWidth(0.02f, 0.02f);
            guideLine.material = new Material(Shader.Find("Unlit/Texture"));
            guideLine.material.SetTexture("_MainTex", guideLineTex);
            guideLine.SetVertexCount(14);

            Debug.Log("create wheelLine");

            wheelLine = wheel.GetComponent<LineRenderer>();
            if (wheelLine == null)
                wheelLine = wheel.AddComponent<LineRenderer>();
            wheelLine.SetWidth(0.02f, 0.02f);
            wheelLine.material = new Material(Shader.Find("Unlit/Texture"));
            wheelLine.material.SetTexture("_MainTex", wheelLineTex);
            wheelLine.SetVertexCount(4);

            guideLine.useWorldSpace = true;
            wheelLine.useWorldSpace = true;            
        }

        private void findWheel()
        {
            WheelCollider firstWheelCollider = part.GetComponentInChildren<WheelCollider>();
            if (firstWheelCollider != null)
            {
                wheel = firstWheelCollider.gameObject;
            }
        }

        private void createTextures()
        {
            guideLineTex = new Texture2D(textureSize, textureSize);
            for (int i = 0; i < textureSize; i++)
            {
                guideLineTex.SetPixel(i, i, Color.red);
            }
            guideLineTex.Apply();

            wheelLineTex = new Texture2D(textureSize, textureSize);
            for (int i = 0; i < textureSize; i++)
            {
                wheelLineTex.SetPixel(i, i, Color.green);
            }
            wheelLineTex.Apply();
        }

        public void setGuideVisibility(bool newState)
        {
            wheelLine.enabled = newState;
            guideLine.enabled = newState;
        }

        void Update()
        {
            if (!HighLogic.LoadedSceneIsEditor) return;

            if (Input.GetKeyDown(KeyCode.F2))
            {
                showGuides = !showGuides;
                setGuideVisibility(showGuides);
            }
            if (showGuides)
            {
                updateLinePositions();
            }
        }

        private void updateLinePositions()
        {
            if (wheel != null)
            {
                Vector3 anglePointForwardHorizontal;
                Vector3 anglePointForwardVertical;
                Vector3 anglePointBackHorizontal;
                Vector3 anglePointBackVertical;
                Vector3 anglePointUpX;
                Vector3 anglePointUpZ;

                Vector3 centerPoint = wheel.transform.position;
                Vector3 guidePointForward = wheel.transform.position + Vector3.forward.normalized * lineLength;
                Vector3 guidePointBack = wheel.transform.position - Vector3.forward.normalized * lineLength;
                Vector3 guidePointUp = wheel.transform.position + Vector3.up.normalized * lineLength;

                Vector3 wheelPointForward = wheel.transform.position + wheel.transform.forward.normalized * lineLength;
                Vector3 wheelPointBack = wheel.transform.position - wheel.transform.forward.normalized * lineLength;
                Vector3 wheelPointUp = wheel.transform.position + wheel.transform.up.normalized * lineLength;

                if (Vector3.Distance(wheelPointForward, guidePointForward) < Vector3.Distance(wheelPointForward, guidePointBack))
                {
                    anglePointForwardHorizontal = new Vector3(wheelPointForward.x, guidePointBack.y, guidePointForward.z);
                    anglePointForwardVertical = new Vector3(guidePointBack.x, wheelPointForward.y, guidePointForward.z);
                    anglePointBackHorizontal = new Vector3(wheelPointBack.x, guidePointForward.y, guidePointBack.z);
                    anglePointBackVertical = new Vector3(guidePointForward.x, wheelPointBack.y, guidePointBack.z);

                }
                else
                {
                    anglePointForwardHorizontal = new Vector3(wheelPointBack.x, guidePointBack.y, guidePointForward.z);
                    anglePointForwardVertical = new Vector3(guidePointBack.x, wheelPointBack.y, guidePointForward.z);
                    anglePointBackHorizontal = new Vector3(wheelPointForward.x, guidePointForward.y, guidePointBack.z);
                    anglePointBackVertical = new Vector3(guidePointForward.x, wheelPointForward.y, guidePointBack.z);
                }
                anglePointUpX = new Vector3(wheelPointUp.x, guidePointUp.y, guidePointUp.z);
                anglePointUpZ = new Vector3(guidePointUp.x, guidePointUp.y, wheelPointUp.z);

                guideLine.SetPosition(0, anglePointUpX);
                guideLine.SetPosition(1, guidePointUp);
                guideLine.SetPosition(2, anglePointUpZ);
                guideLine.SetPosition(3, guidePointUp);
                guideLine.SetPosition(4, centerPoint);
                guideLine.SetPosition(5, guidePointForward);
                guideLine.SetPosition(6, anglePointForwardHorizontal);
                guideLine.SetPosition(7, guidePointForward);
                guideLine.SetPosition(8, anglePointForwardVertical);
                guideLine.SetPosition(9, guidePointForward);
                guideLine.SetPosition(10, guidePointBack);
                guideLine.SetPosition(11, anglePointBackHorizontal);
                guideLine.SetPosition(12, guidePointBack);
                guideLine.SetPosition(13, anglePointBackVertical);

                wheelLine.SetPosition(0, wheelPointUp);
                wheelLine.SetPosition(1, centerPoint);
                wheelLine.SetPosition(2, wheelPointForward);
                wheelLine.SetPosition(3, wheelPointBack);
            }
        }
    }
}