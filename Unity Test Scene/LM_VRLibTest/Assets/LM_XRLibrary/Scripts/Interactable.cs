using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LM_XR
{
    //**********************************************//
    // Base class for objects that are interactable //
    // in virtual environments                      //
    // Only used with gazeCast!!                    //
    //**********************************************//

    public class Interactable : MonoBehaviour
    {
        /// <summary>
        /// virtual method for trigger events
        /// generic button events (span vr)
        /// </summary>

        // collection focus
        // local focus
        protected Collider objCollider;

        protected bool collectionFocus;
        protected bool localFocus;

        // Use this for initialization
        void Start()
        {
            objCollider = GetComponent<Collider>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void SetCollectionFocus(bool _focus)
        {
            collectionFocus = _focus;
        }

        public void SetLocalFocus(bool _focus)
        {
            localFocus = _focus;
        }

        public virtual Interactable ReturnInteractable()
        {
            return this;
        }
    }
}
