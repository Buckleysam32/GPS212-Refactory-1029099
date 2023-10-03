using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class TimedObjectDestructor : MonoBehaviour
    {
        [SerializeField] private float timeOut = 1.0f;
        [SerializeField] private bool detachChildren = false;

        // Called when game starts.
        private void Awake()
        {
            Invoke("DestroyNow", timeOut);
        }

        /// <summary>
        /// A function that detaches all children and destroys the game object.
        /// </summary>
        private void DestroyNow()
        {
            if (detachChildren)
            {
                transform.DetachChildren();
            }
            Destroy(gameObject);
        }
    }
}
