using System;
using Skripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.Serialization;

namespace Skripts
{
	public interface ILogic : System.Runtime.Serialization.ISerializable
	{
		List<Wire> In { get; set; }
		List<Wire> Out { get; set; }
		void Think();
	}

	[Serializable]
	public abstract class LogicBlock : ILogic
	{
		public List<Wire> In { get; set; }
		public List<Wire> Out { get; set; }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("In", In, typeof(List<Wire>));
			info.AddValue("Out", Out, typeof(List<Wire>));
		}

		public abstract void Think();
	}
	public class Not : LogicBlock
	{
		public Not()
		{
			In = new List<Wire>() { null };
			Out = new List<Wire>() { new Wire(this) };
		}
		public override void Think()
		{
			Out[0].Value = !In[0].Value;
		}
	}
	public class Or : LogicBlock
	{
		public Or()
		{
			In = new List<Wire>() { null, null };
			Out = new List<Wire>() { new Wire(this) };
		}
		public override void Think()
		{
			Out[0].Value = In[0].Value || In[1].Value;
		}
	}
	public class And : LogicBlock
	{
		public And()
		{
			In = new List<Wire>() { null,null};
			Out = new List<Wire>() { new Wire(this) };
		}
		public override void Think()
		{
			Out[0].Value = In[0].Value && In[1].Value;
		}
	}
	public class LogicObject : MonoBehaviour , ILogic
	{
		public virtual List<Wire> In { get; set; }
		public virtual List<Wire> Out { get; set; }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("In", In, typeof(List<Wire>));
			info.AddValue("Out", Out, typeof(List<Wire>));
		}

		public void Think()
		{
			throw new NotImplementedException();
		}
	}
	public class Door : LogicObject
	{

	}
	public class TriggerArea : LogicObject
	{
		
	}

	[Serializable]
	public class Wire
	{
		public ILogic Parent;
		public bool Value;

		public Wire(ILogic parent) 
		{
			Parent = parent;
		}
	}
	/*//
	[CustomPropertyDrawer(typeof(List<Wire>))]
	public class Wires : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty scale = prop.FindPropertyRelative("scale");
			SerializedProperty curve = prop.FindPropertyRelative("curve");

			// Draw scale
			EditorGUI.Slider(
				new Rect(pos.x, pos.y, pos.width - 50, pos.height),
				scale, 0, 1, label);

			// Draw curve
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUI.PropertyField(
				new Rect(pos.width - 50, pos.y, 50, pos.height),
				curve, GUIContent.none);
			EditorGUI.indentLevel = indent;
		}
	}
	public class Regex : PropertyAttribute
	{

		public readonly string pattern;
		public readonly string helpMessage;

		public Regex(string pattern, string helpMessage)
		{
			this.pattern = pattern;
			this.helpMessage = helpMessage;
		}
	}
	//*/
}