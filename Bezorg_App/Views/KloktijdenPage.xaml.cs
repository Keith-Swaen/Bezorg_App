//using Windows.UI.Notifications;

using System.Timers;

namespace Bezorg_App.Views;

public partial class KloktijdenPage : ContentPage
{
    private bool _isClockedIn;
    private bool _isOnBreak;
    private DateTime _clockInTime;
    private DateTime? _breakStartTime;
    private TimeSpan _totalWorkTime;
    private TimeSpan _totalBreakTime;
    private System.Timers.Timer _workTimeTimer;

    public KloktijdenPage()
    {
        InitializeComponent();
        LoadState();
        UpdateStatus();
        StartWorkTimeTimer();
    }

    private void LoadState()
    {
        _isClockedIn = Preferences.Get("IsClockedIn", false);
        _isOnBreak = Preferences.Get("IsOnBreak", false);
        _totalWorkTime = TimeSpan.Parse(Preferences.Get("TotalWorkTime", "00:00:00"));
        _totalBreakTime = TimeSpan.Parse(Preferences.Get("TotalBreakTime", "00:00:00"));
        if (_isClockedIn)
        {
            _clockInTime = DateTime.Parse(Preferences.Get("ClockInTime", DateTime.Now.ToString()));
        }
        if (_isOnBreak)
        {
            _breakStartTime = DateTime.Parse(Preferences.Get("BreakStartTime", DateTime.Now.ToString()));
        }
    }

    private void UpdateStatus()
    {
        ClockInButton.IsEnabled = !_isClockedIn;
        ClockOutButton.IsEnabled = _isClockedIn && !_isOnBreak;
        StartBreakButton.IsEnabled = _isClockedIn && !_isOnBreak;
        EndBreakButton.IsEnabled = _isOnBreak;

        if (_isOnBreak)
        {
            StatusLabel.Text = $"Op pauze sinds {_breakStartTime?.ToString("HH:mm:ss")}";
        }
        else if (_isClockedIn)
        {
            StatusLabel.Text = $"Ingeklokt sinds {_clockInTime.ToString("HH:mm:ss")}";
        }
        else
        {
            StatusLabel.Text = "Nog niet ingeklokt";
        }

        UpdateWorkTime();
        UpdateBreakTime();
    }

    private void StartWorkTimeTimer()
    {
        _workTimeTimer = new System.Timers.Timer(1000); // Update elke seconde
        _workTimeTimer.Elapsed += WorkTimeTimerElapsed;
        if (_isClockedIn && !_isOnBreak)
        {
            _workTimeTimer.Start();
        }
    }

    private void WorkTimeTimerElapsed(object sender, ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateWorkTime();
            UpdateBreakTime();
        });
    }

    private void UpdateWorkTime()
    {
        if (_isClockedIn && !_isOnBreak)
        {
            var currentSessionTime = DateTime.Now - _clockInTime;
            WorkTimeLabel.Text = $"Gewerkt: {currentSessionTime.Hours} uur {currentSessionTime.Minutes} minuten";
        }
        else
        {
            WorkTimeLabel.Text = $"Gewerkt: {_totalWorkTime.Hours} uur {_totalWorkTime.Minutes} minuten";
        }
    }

    private void UpdateBreakTime()
    {
        if (_isOnBreak)
        {
            var currentBreakTime = DateTime.Now - _breakStartTime ?? TimeSpan.Zero;
            BreakTimeLabel.Text = $"Pauze: {_totalBreakTime.Hours + currentBreakTime.Hours} uur {_totalBreakTime.Minutes + currentBreakTime.Minutes} minuten";
        }
        else
        {
            BreakTimeLabel.Text = $"Pauze: {_totalBreakTime.Hours} uur {_totalBreakTime.Minutes} minuten";
        }
    }

    private async void OnClockInClicked(object sender, EventArgs e)
    {
        if (!_isClockedIn)
        {
            _isClockedIn = true;
            _clockInTime = DateTime.Now;
            Preferences.Set("IsClockedIn", true);
            Preferences.Set("ClockInTime", _clockInTime.ToString());
            _workTimeTimer.Start();
            UpdateStatus();
            await DisplayAlert("Succes", "Je bent ingeklokt!", "OK");
        }
    }

    private async void OnClockOutClicked(object sender, EventArgs e)
    {
        if (_isClockedIn && !_isOnBreak)
        {
            var currentSessionTime = DateTime.Now - _clockInTime;
            _totalWorkTime += currentSessionTime;
            Preferences.Set("TotalWorkTime", _totalWorkTime.ToString());
            _isClockedIn = false;
            Preferences.Set("IsClockedIn", false);
            Preferences.Remove("ClockInTime");
            _workTimeTimer.Stop();
            UpdateStatus();
            await DisplayAlert("Succes", "Je bent uitgeklokt!", "OK");
        }
    }

    private async void OnStartBreakClicked(object sender, EventArgs e)
    {
        if (_isClockedIn && !_isOnBreak)
        {
            if (int.TryParse(BreakDurationEntry.Text, out int breakMinutes) && breakMinutes > 0)
            {
                _isOnBreak = true;
                _breakStartTime = DateTime.Now;
                Preferences.Set("IsOnBreak", true);
                Preferences.Set("BreakStartTime", _breakStartTime.ToString());
                _workTimeTimer.Stop();
                UpdateStatus();
                await DisplayAlert("Succes", $"Pauze gestart voor {breakMinutes} minuten.", "OK");
            }
            else
            {
                await DisplayAlert("Fout", "Voer een geldige pauzetijd in (in minuten).", "OK");
            }
        }
    }

    private async void OnEndBreakClicked(object sender, EventArgs e)
    {
        if (_isOnBreak)
        {
            var breakDuration = DateTime.Now - _breakStartTime ?? TimeSpan.Zero;
            _totalBreakTime += breakDuration;
            Preferences.Set("TotalBreakTime", _totalBreakTime.ToString());
            _isOnBreak = false;
            Preferences.Set("IsOnBreak", false);
            Preferences.Remove("BreakStartTime");
            _workTimeTimer.Start();
            UpdateStatus();
            await DisplayAlert("Succes", "Pauze gestopt!", "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _workTimeTimer?.Stop();
        _workTimeTimer?.Dispose();
    }
}