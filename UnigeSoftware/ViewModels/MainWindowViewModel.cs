using OgeSharp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace UnigeSoftware.ViewModels {
    public class MainWindowViewModel : ReactiveObject {

        public Oge Oge = new Oge();

        [Reactive] public ReactiveObject Content { get; set; }

        public MainWindowViewModel() {

            // Initialize the connect view model
            Content = new ConnectViewModel(this);

        }

        public void OnConnected() => Content = new HomeViewModel(this);

    }
}
