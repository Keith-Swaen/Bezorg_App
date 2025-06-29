using System;
using System.ComponentModel;
using Bezorg_App.Models.Enums;

namespace Bezorg_App.Models
{
    //Inotify voor automatische Ui update
    public class DeliveryState : INotifyPropertyChanged
    {
        private DeliveryStateEnum _state;
        public DeliveryStateEnum State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        private DateTime _dateTime;
        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value;
                    OnPropertyChanged(nameof(DateTime));
                }
            }
        }

        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int? DeliveryServiceId { get; set; }
        public DeliveryService? DeliveryService { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
