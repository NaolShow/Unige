using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;

namespace UnigeSoftware.ViewModels {

    public class ConnectViewModel : ReactiveObject, IValidatableViewModel {

        private readonly MainWindowViewModel MainWindow;
        public ValidationContext ValidationContext { get; } = new ValidationContext();

        [Reactive] public string Username { get; set; }
        [Reactive] public string Password { get; set; }

        public ConnectViewModel(MainWindowViewModel mainWindow) {

            // Save the main window view model
            MainWindow = mainWindow;

            // Create the commands
            ConnectCommand = ReactiveCommand.CreateFromObservable(DoConnect);
            OpenFromExplorerCommand = ReactiveCommand.Create<string>(DoOpenWebpage);

            // Username and password cannot be empty
            this.ValidationRule(viewModel => viewModel.Username, username => !string.IsNullOrEmpty(username), "Le nom d'utilisateur ne peut pas être vide");
            this.ValidationRule(viewModel => viewModel.Password, password => !string.IsNullOrEmpty(password), "Le mot de passe ne peut pas être vide");

        }

        #region Commands

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<string, Unit> OpenFromExplorerCommand { get; }

        // TODO: Maybe not working on other OS
        private void DoOpenWebpage(string url) => Process.Start("explorer", url);

        private IObservable<Unit> DoConnect() => Observable.Start(() => {

            // If the fields are not validated
            if (!ValidationContext.GetIsValid()) return;

            // If the credentials are correct
            if (MainWindow.Oge.Login(Username, Password)) {

                // Tell the main window that we are connected
                MainWindow.OnConnected();

            }

        });

        #endregion

    }

}
