using System;

// See https://github.com/Zetrith/HotSwap/wiki
namespace BiomesIslands
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class HotSwappableAttribute : Attribute
	{
	}
}