using System;
using System.Timers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Bezorg_App.Views
{
    public partial class KloktijdenPage : ContentPage
    {
        private bool _isClockedIn;
        private bool _isOnBreak;
        private DateTime _clockInTime;
        private DateTime? _breakStartTime;
        private TimeSpan _currentSessionWorkTime;
        private TimeSpan _allTimeWorkTime;
        private TimeSpan _totalBreakTime;
        private TimeSpan _lastWorkTime;
        private DateTime _lastWorkTimeStart;
        private System.Timers.Timer _workTimeTimer;
        private System.Timers.Timer _breakReminderTimer;

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
            _currentSessionWorkTime = TimeSpan.Zero; // Huidige sessie start op 0
            _allTimeWorkTime = TimeSpan.Parse(Preferences.Get("AllTimeWorkTime", "00:00:00"));
            _totalBreakTime = TimeSpan.Parse(Preferences.Get("TotalBreakTime", "00:00:00"));
            _lastWorkTime = TimeSpan.Zero;
            if (_isClockedIn)
            {
                _clockInTime = DateTime.Parse(Preferences.Get("ClockInTime", DateTime.Now.ToString()));
                _lastWorkTimeStart = _clockInTime;
                if (!_isOnBreak)
                {
                    _currentSessionWorkTime = DateTime.Now - _clockInTime; // Herstel sessietijd bij laden
                }
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
            if (_isClockedIn)
            {
                _workTimeTimer.Start(); // Timer blijft aan, ook tijdens pauze, om pauzetijd te updaten
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
                _currentSessionWorkTime = _lastWorkTime + (DateTime.Now - _lastWorkTimeStart);
                WorkTimeLabel.Text = $"Huidige werktijd: {_currentSessionWorkTime.Hours} uur {_currentSessionWorkTime.Minutes} minuten ({_currentSessionWorkTime.Seconds})";
            }
            else
            {
                WorkTimeLabel.Text = $"Huidige werktijd: {_currentSessionWorkTime.Hours} uur {_currentSessionWorkTime.Minutes} minuten ({_currentSessionWorkTime.Seconds})";
            }
        }

        private void UpdateBreakTime()
        {
            TimeSpan totalBreak;

            if (_isOnBreak)
            {
                var currentBreakTime = DateTime.Now - _breakStartTime ?? TimeSpan.Zero;
                totalBreak = _totalBreakTime + currentBreakTime;
            }
            else
            {
                totalBreak = _totalBreakTime;
            }

            BreakTimeLabel.Text = $"Pauze: {totalBreak.Hours} uur {totalBreak.Minutes} minuten ({totalBreak.Seconds})";
        }
        private void BreakReminderElapsed(object sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Pauze afgelopen", "Je ingevoerde pauze is voorbij. Vergeet niet om de pauzen te stoppen en weer aan het werk te gaan!", "OK");
            });
        }

        private async void OnClockInClicked(object sender, EventArgs e)
        {
            if (!_isClockedIn)
            {
                _isClockedIn = true;
                _clockInTime = DateTime.Now;
                _lastWorkTimeStart = _clockInTime;
                _currentSessionWorkTime = TimeSpan.Zero;
                _lastWorkTime = TimeSpan.Zero;
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
                bool confirm = await DisplayAlert("Bevestiging", "Weet je zeker dat je wilt stoppen met werken? De tijd wordt toegevoegd aan je totale werktijd.", "Ja", "Nee");
                if (confirm)
                {
                    _allTimeWorkTime += _currentSessionWorkTime;
                    Preferences.Set("AllTimeWorkTime", _allTimeWorkTime.ToString());
                    _currentSessionWorkTime = TimeSpan.Zero;
                    _totalBreakTime = TimeSpan.Zero;
                    _lastWorkTime = TimeSpan.Zero;
                    Preferences.Set("TotalBreakTime", _totalBreakTime.ToString());
                    _isClockedIn = false;
                    Preferences.Set("IsClockedIn", false);
                    Preferences.Remove("ClockInTime");
                    _workTimeTimer.Stop();
                    UpdateStatus();
                    await DisplayAlert("Succes", "Je bent uitgeklokt!", "OK");
                }
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
                    _lastWorkTime = _currentSessionWorkTime; // Bewaar huidige werktijd
                    Preferences.Set("IsOnBreak", true);
                    Preferences.Set("BreakStartTime", _breakStartTime.ToString());
                    UpdateStatus();

                    // Start de pauze timer voor een herrnnering
                    _breakReminderTimer = new System.Timers.Timer(breakMinutes * 1000 * 60); // pauzeduur in ms
                    _breakReminderTimer.Elapsed += BreakReminderElapsed;
                    _breakReminderTimer.AutoReset = false;
                    _breakReminderTimer.Start();

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
                _lastWorkTimeStart = DateTime.Now; // Start nieuwe werksessie zonder pauzetijd mee te tellen
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
}