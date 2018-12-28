using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
     
namespace UnityEngine.UI
{
     
    public class DoubleClick : InputField
     
    {
     
        public override void OnPointerClick(PointerEventData eventData) {
     
            if (eventData.clickCount == 2) 
                ActivateInputField ();
            
        }
        
        public override void OnSelect(BaseEventData eventData)
        {
             
            //base.OnSelect(eventData);
            //ActivateInputField();
     
        }
     
    }
     
}
