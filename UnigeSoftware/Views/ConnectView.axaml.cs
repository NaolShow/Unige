using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Disposables;
using UnigeSoftware.ViewModels;

namespace UnigeSoftware.Views {

    public partial class ConnectView : ReactiveUserControl<ConnectViewModel> {

        public ConnectView() {
            InitializeComponent();

            // Input validation
            this.WhenActivated(disposables => {

                this.BindValidation(ViewModel, viewModel => viewModel.Username, view => view.UsernameError.Text).DisposeWith(disposables);
                this.BindValidation(ViewModel, viewModel => viewModel.Password, view => view.PasswordError.Text).DisposeWith(disposables);

            });

        }

    }
}
