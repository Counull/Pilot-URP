using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;


namespace CustomAttribute {

    /// <summary>
    /// 幼稚且不灵活的Attribute，缺乏经验
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InGameInputActionAttribute : PropertyAttribute {
        private static readonly List<InputAction> Actions = new List<InputAction>();

        public InGameInputActionAttribute() {
        
        }

        public static void CatchAllInputAction(MonoBehaviour target) {
            var fields =
                target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields) {
                if (IsDefined(field, typeof(InGameInputActionAttribute))) {
                    if (field.GetValue(target) is InputAction inputAction) {
                        Actions.Add(inputAction);
                    }
                }
            }
        }

        public static void SetAllActionEnable(bool enable) {
            if (enable) {
                foreach (var inputAction in Actions) {
                    inputAction.Enable();
               
                }

                return;
            }

            foreach (var inputAction in Actions) {
                inputAction.Disable();
            }
        }
    }
}