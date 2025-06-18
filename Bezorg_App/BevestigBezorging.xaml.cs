using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System;

namespace Bezorg_App;

public partial class BevestigBezorging : ContentPage
{
    public BevestigBezorging()
    {
        InitializeComponent();
    }

    private async void OnBevestigBezorgingClicked(object sender, EventArgs e)
    {
        try
        {
            FileResult photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                PreviewImage.Source = ImageSource.FromStream(() => stream);

                // Show the image and confirmation text
                PreviewImageBorder.IsVisible = true;
                ConfirmationLabel.IsVisible = true;

                await DisplayAlert("Foto gemaakt", $"Opgeslagen op: {photo.FullPath}", "OK");
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
}
