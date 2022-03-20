using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace UnigeSoftware.ViewModels {

    public class HomeViewModel : ReactiveObject {

        [Reactive] public string Note { get; set; }

        public HomeViewModel(MainWindowViewModel mainWindow) {

        }

    }

}
