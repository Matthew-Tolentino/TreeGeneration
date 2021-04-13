using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAnimation : MonoBehaviour
{
    public float scaleUpFactor = .01f;
    public float scaleTick = .001f;

    public float angleFactor = .01f;
    public float angleTick = .001f;

    NoiseTreeGenerator nGen;
    float branchAngle;

    void Start() {
        nGen = GetComponent<NoiseTreeGenerator>();
        branchAngle = nGen.branchAngle;
        nGen.branchAngle = 179.9f;
        nGen.GenerateTree();
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp());
        StartCoroutine(AnimateBranches());
    }

    IEnumerator ScaleUp() {
        Vector3 scaleVec = transform.localScale;
        Vector3 stepVec = new Vector3(scaleUpFactor, scaleUpFactor, scaleUpFactor);

        float lastTimeScaled = Time.time;
        while (transform.localScale.x < 1) {
            if (Time.time > lastTimeScaled + scaleTick) {
                scaleVec += stepVec;
                lastTimeScaled = Time.time;
            }
            yield return transform.localScale = scaleVec;
        }

        // StartCoroutine(AnimateBranches());
    }

    IEnumerator AnimateBranches() {
        float lastTimeTick = Time.time;
        while (nGen.branchAngle > branchAngle) {
            if (Time.time > lastTimeTick + angleTick) {
                nGen.branchAngle += angleFactor;
                nGen.GenerateTree();
                lastTimeTick = Time.time;
            }
            yield return null;
        }
    }
}
