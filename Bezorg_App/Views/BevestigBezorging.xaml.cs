using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Syncfusion.Maui.SignaturePad;

namespace Bezorg_App.Views;

public partial class BevestigBezorging : ContentPage
{
    private bool hasSigned = false;
    private bool hasPhoto = false;
    private bool isConfirmed = false;

    public BevestigBezorging()
    {
        InitializeComponent();
    }

    private void OnClearSignatureClicked(object sender, EventArgs e)
    {
        if (isConfirmed) return;

        SignaturePad.Clear();
        hasSigned = false;
        UpdateConfirmButtonVisibility();
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        if (isConfirmed) return;

        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                PreviewImage.Source = ImageSource.FromStream(() => stream);
                PreviewImageBorder.IsVisible = true;
                hasPhoto = true;

                await DisplayAlert("Foto gemaakt", "De foto is succesvol genomen.", "OK");
                UpdateConfirmButtonVisibility();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Niet ondersteund", "Camera wordt niet ondersteund op dit apparaat.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Geen toestemming", "Camera toegang geweigerd.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is iets misgegaan: {ex.Message}", "OK");
        }
    }

    private void UpdateConfirmButtonVisibility()
    {
        
    }


    private async void OnConfirmSignatureClicked(object sender, EventArgs e)
    {
        if (isConfirmed) return;

        hasSigned = true;
        PhotoLabel.IsVisible = true;
        TakePhotoButton.IsVisible = true;

        await DisplayAlert("Handtekening bevestigd", "U kunt nu een foto maken.", "OK");
    }


    private async void OnFinalConfirmClicked(object sender, EventArgs e)
    {
        isConfirmed = true;
        ConfirmButton.IsVisible = false;

        // Niet meer laten tekenen
        SignaturePad.IsEnabled = false;

        // Knoppen weghalen
        ConfirmationLabel.IsVisible = true;

        await DisplayAlert("Bevestigd", "Bezorging is succesvol bevestigd.", "OK");
    }
}
