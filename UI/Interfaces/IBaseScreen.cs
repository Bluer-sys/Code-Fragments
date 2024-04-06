using System;

namespace UI.Interfaces
{
	public interface IBaseScreen
	{
		event Action<IBaseScreen> OnDestroyed;
		void Show();
		void Hide();
		void Close();
	}
}