namespace Install_1.DOM
{
	using System;

	public class ItemProgressEventArgs : EventArgs
	{
		public ItemProgressEventArgs(int items) => Items = items;

		public int Items { get; internal set; }
	}
}