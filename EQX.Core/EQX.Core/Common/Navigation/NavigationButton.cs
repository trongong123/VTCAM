using System.Windows.Input;

namespace EQX.Core.Common
{
    public interface INavigationButtonRepository
    {
        IEnumerable<NavigationButton> GetNavigationButtons(string groupName = "");
    }

    public class NavigationButtonRepository : INavigationButtonRepository
    {
        private readonly IEnumerable<NavigationButton> _navigationButtons;
        private readonly IUserStore _userStore;

        public NavigationButtonRepository(IEnumerable<NavigationButton> navigationButtons,
            IUserStore userStore)
        {
            _navigationButtons = navigationButtons;
            _userStore = userStore;
        }

        public IEnumerable<NavigationButton> GetNavigationButtons(string groupName = "")
        {
            List<NavigationButton> buttons = new();

            foreach (var button in _navigationButtons)
            {
                if (button.UseCurrentPermissionAsLabel)
                {
                    button.Label = _userStore.Permission.ToString();
                }

                if (_userStore.Permission >= button.RequiredRole)
                {
                    if (string.IsNullOrEmpty(groupName)) buttons.Add(button);
                    if (string.IsNullOrEmpty(groupName) == false && groupName == button.GroupName) buttons.Add(button);
                }
            }

            return buttons;
        }
    }

    public class NavigationButton
    {
        public string Label { get; set; }
        public string GroupName { get; set; }
        /// <summary>
        /// Must be <see cref="ViewModelBase"/> derived type
        /// </summary>
        public Type ViewModelType { get; set; }
        public EPermission RequiredRole { get; set; }

        public string ImageKey { get; set; }
        public string DisabledImageKey { get; set; }
        public bool UseCurrentPermissionAsLabel { get; set; }
    }
}
