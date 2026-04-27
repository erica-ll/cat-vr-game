using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace NekoLegends
{
    public class NekoMew : MonoBehaviour
    {
        [SerializeField] protected AnimationClip[] poses;
        [SerializeField] protected SkinnedMeshRenderer skinnedMeshRendererMain, skinnedMeshEyes;
        [SerializeField] protected Material[] hairMaterials, eyeMaterials;
        [SerializeField] protected Material[] outlines;
        [SerializeField] protected GameObject collar, glasses;

        public Animator animator { get; private set; }


        protected int _featureIndex, _emotionIndex, _currentOutlineIndex, _hairColorIndex, _currentEyeIndex, _currentPoseIndex = 0;

        protected float _transitionDuration = 0.1f;
        protected Material[] _hairMaterials, _eyeMaterials;
        private bool _isShowEarFuzz;
        private int _featureCount = 9;
        private int _emotionCount = 5;


        protected virtual void Start()
        {
            animator = GetComponent<Animator>();

            _hairMaterials = skinnedMeshRendererMain.materials;
            _eyeMaterials = skinnedMeshEyes.materials;
            NekoMewDemo.Instance.DescriptionText.SetText("Neko Mew");
        }

        //AnimationEvent functions are used for some animations
        public void AnimationEventEyeHappy()
        {
            AnimationEventEyeReset();
            skinnedMeshEyes.SetBlendShapeWeight(1, 100);
        }
        public void AnimationEventEyeClosed()
        {
            AnimationEventEyeReset();
            skinnedMeshEyes.SetBlendShapeWeight(2, 100);
        }
        public void AnimationEventEyeReset()
        {
            skinnedMeshEyes.SetBlendShapeWeight(1, 0);
            skinnedMeshEyes.SetBlendShapeWeight(2, 0);
        }
        // end animationEvent functions


        public void CycleEmotion()
        {
            _emotionIndex = (_emotionIndex + 1) % _emotionCount;
            ResetBlendShapes();
            switch (_emotionIndex)
            {
                case 0:
                    ResetBlendShapes();
                    NekoMewDemo.Instance.SetDescriptionText("Emotion: None");
                    skinnedMeshEyes.SetBlendShapeWeight(1, 0);
                    skinnedMeshEyes.SetBlendShapeWeight(2, 0);
                    break;
                case 1:
                    NekoMewDemo.Instance.SetDescriptionText("Emotion: Happy");
                    skinnedMeshEyes.SetBlendShapeWeight(1, 100);
                    break;
                case 2:
                    NekoMewDemo.Instance.SetDescriptionText("Emotion: Closed");
                    skinnedMeshEyes.SetBlendShapeWeight(2, 100);
                    break;
                case 3:
                    NekoMewDemo.Instance.SetDescriptionText("Emotion: Sad");
                    SetBlendShapeWeightFace(7, 100f);
                    break;
                case 4:
                    NekoMewDemo.Instance.SetDescriptionText("Emotion:  Mad");
                    SetBlendShapeWeightFace(8, 100f);
                    break;
            }
        }


        public void CycleMisc()
        {
            _featureIndex = (_featureIndex + 1) % _featureCount;
            switch (_featureIndex)
            {
                case 0:
                    collar.SetActive(true);
                    glasses.SetActive(false);
                    ResetBlendShapes();
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Reset All Features");
                    break;
                case 1:
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Tail Up");
                    SetBlendShapeWeightFace(6, 100f);
                    break;
                case 2:
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Remove Whiskers");
                    SetBlendShapeWeightFace(5, 100f);
                    break;
                case 3:
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Remove Ear Fuzz");
                    SetBlendShapeWeightFace(9, 100f);
                    break;
                case 4:
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Eyes Tilt Down");
                    skinnedMeshEyes.SetBlendShapeWeight(3, 100f);
                    break;
                case 5:
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Eyes Tilt Up");
                    skinnedMeshEyes.SetBlendShapeWeight(4, 100f);
                    break;
                case 6:
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Remove Collar");
                    collar.SetActive(false);
                    break;
                case 7:
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Neck Thining");
                    SetBlendShapeWeightFace(10, 100f);
                    break;
                case 8:
                    NekoMewDemo.Instance.SetDescriptionText("Misc: Glasses");
                    glasses.SetActive(true);
                    break;
                    

            }
        }

        private void SetBlendShapeWeightFace(int blendShapeIndex, float targetWeight, float duration = .25f)
        {
            StartCoroutine(AnimateBlendShape(blendShapeIndex, targetWeight, duration));

        }
        private IEnumerator AnimateBlendShape(int blendShapeIndex, float targetWeight, float duration)
        {
            float startTime = Time.time;
            float startWeight = skinnedMeshRendererMain.GetBlendShapeWeight(blendShapeIndex);

            while (Time.time < startTime + duration)
            {
                float elapsed = Time.time - startTime;
                float normalizedTime = elapsed / duration;

                float weight = Mathf.Lerp(startWeight, targetWeight, normalizedTime);
                skinnedMeshRendererMain.SetBlendShapeWeight(blendShapeIndex, weight);

                yield return null;
            }

            skinnedMeshRendererMain.SetBlendShapeWeight(blendShapeIndex, targetWeight);
        }

        public int GetFeatureCount()
        {
            return this._featureCount;
        }

        public virtual void AnimPose()
        {
            // Increment the pose index
            _currentPoseIndex++;

            // If the index exceeds the array length, wrap around to the beginning
            if (_currentPoseIndex >= poses.Length)
            {
                _currentPoseIndex = 0;
            }

            // Get the next animation clip
            AnimationClip nextPose = poses[_currentPoseIndex];
           
            // Play the next animation clip
            this.animator.CrossFade(nextPose.name, _transitionDuration);
            NekoMewDemo.Instance.DescriptionText.SetText("Animation Pose: " + nextPose.name);
        }

        public void CycleEyes(int index=-1)
        {
            _currentEyeIndex = (_currentEyeIndex + 1) % eyeMaterials.Length;

            if (index != -1)
            {
                _currentEyeIndex = index;
            }
            
            if (_currentEyeIndex == eyeMaterials.Length - 1) // Last index (null)
            {
                _eyeMaterials[0] = eyeMaterials[3]; // Beige color eyes
                _eyeMaterials[1] = eyeMaterials[4]; // Blue color eyes
            }
            else
            {
                _eyeMaterials[0] = eyeMaterials[_currentEyeIndex];
                _eyeMaterials[1] = eyeMaterials[_currentEyeIndex];
            }

            skinnedMeshEyes.materials = _eyeMaterials;
        }

        
        public void ToggleEarFuzz()
        {
            _isShowEarFuzz = !_isShowEarFuzz;

            if (_isShowEarFuzz)
            {

                SetBlendShapeWeightFace(18, 10f);
            }
        }

        public void CycleFur()
        {
            _hairColorIndex++;
            if (_hairColorIndex >= hairMaterials.Length)
                _hairColorIndex = 0;
            // Store the current materials (including outlines)
            Material[] currentMaterials = skinnedMeshRendererMain.materials;

            _hairMaterials[0] = hairMaterials[_hairColorIndex];
            skinnedMeshRendererMain.materials = _hairMaterials;
            // Restore the outlines from the stored materials
            for (int i = 1; i < currentMaterials.Length; i++)
            {
                AddMaterialToRenderer(skinnedMeshRendererMain, currentMaterials[i]);
            }
            NekoMewDemo.Instance.DescriptionText.SetText(hairMaterials[_hairColorIndex].name);
        }

        public void AddMaterialToRenderer(SkinnedMeshRenderer renderer, Material materialToAdd)
        {
            Material[] currentMaterials = renderer.materials;
            Material[] newMaterials = new Material[currentMaterials.Length + 1];
            currentMaterials.CopyTo(newMaterials, 0);
            newMaterials[currentMaterials.Length] = materialToAdd;
            renderer.materials = newMaterials;
        }

        public void RemoveLastMaterialFromRenderer(SkinnedMeshRenderer renderer)
        {
            Material[] currentMaterials = renderer.materials;
            Material[] newMaterials = new Material[currentMaterials.Length - 1];
            Array.Copy(currentMaterials, newMaterials, newMaterials.Length);
            renderer.materials = newMaterials;
        }

        private void ResetBlendShapes()
        {
            for (int i = 0; i < skinnedMeshRendererMain.sharedMesh.blendShapeCount; i++)
            {
                skinnedMeshRendererMain.SetBlendShapeWeight(i, 0f);
            }
            for (int i = 0; i < skinnedMeshEyes.sharedMesh.blendShapeCount; i++)
            {
                skinnedMeshEyes.SetBlendShapeWeight(i, 0f);
            }

        }

        public void CycleOutlines()
        {
            if (skinnedMeshRendererMain.materials.Length > 1)
            {
                RemoveLastMaterialFromRenderer(skinnedMeshRendererMain);
            }

            _currentOutlineIndex = (_currentOutlineIndex + 1) % outlines.Length;
            if (outlines[_currentOutlineIndex] != null)
            {
                AddMaterialToRenderer(skinnedMeshRendererMain, outlines[_currentOutlineIndex]);

                NekoMewDemo.Instance.DescriptionText.SetText(outlines[_currentOutlineIndex].name);
            }
            else
            {
                NekoMewDemo.Instance.DescriptionText.SetText("No Outlines");
            }
            
        }

    }
}
