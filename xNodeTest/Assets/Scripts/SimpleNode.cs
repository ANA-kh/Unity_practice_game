using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
public class SimpleNode : Node { 
	[Input] public float a;
	[Output] public float b;

	
	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}
	
	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		if (port.fieldName == "b") return GetInputValue<float>("a", a);
		else return null;
	}
}