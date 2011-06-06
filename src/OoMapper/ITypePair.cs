namespace OoMapper
{
	using System;

	public interface ITypePair
	{
		Type SourceType { get; }
		Type DestinationType { get; }
	}
}