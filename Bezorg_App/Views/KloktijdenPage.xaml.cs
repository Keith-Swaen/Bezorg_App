//using Windows.UI.Notifications;

namespace Bezorg_App.Views;

public partial class KloktijdenPage : ContentPage
{
    private bool _isClockedIn;
    private bool _isOnBreak;
    private DateTime _clockInTime;
    private DateTime? _breakStartTime;
    private TimeSpan _totalWorkTime;
    private Timer _workTimeTimer;

    public KloktijdenPage()
    {
        InitializeComponent();
        LoadState();
        UpdateStatus();
        StartWorkTimeTimer();
    }

    private void LoadState()
    {
       
    }

    private void UpdateStatus()
    {
       
    }

    private void StartWorkTimeTimer()
    {
       
    }

    private void UpdateWorkTime()
    {
       
    }

    private async void OnClockInClicked(object sender, EventArgs e)
    {
       
    }

    private async void OnClockOutClicked(object sender, EventArgs e)
    {
       
    }

    private async void OnStartBreakClicked(object sender, EventArgs e)
    {
       
    }

    private async void OnEndBreakClicked(object sender, EventArgs e)
    {
       
    }

    private async Task ScheduleBreakNotification(int breakMinutes)
    {
        
    }
}