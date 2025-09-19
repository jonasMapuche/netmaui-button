using net_maui_app_v24.ViewModels;
using Plugin.BLE.Abstractions.EventArgs;

namespace net_maui_app_v24;

public partial class ModalView : ContentPage
{
    ModalViewModel viewModel;

    public ModalView()
	{
		InitializeComponent();
        viewModel = new ModalViewModel();
        BindingContext = viewModel;

        Application.Current.ModalPushed += OnModalPushed;
        Application.Current.ModalPopping += OnModalPopping;
    }

    private void OnModalPushed(object sender, ModalPushedEventArgs e)
    {
        this.BackgroundColor = Color.FromArgb("#80000000");
    }

    private void OnModalPopping(object sender, ModalPoppingEventArgs e)
    {
        this.BackgroundColor = Color.FromArgb("#00000000");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        viewModel.UpdateTimer.Stop();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}