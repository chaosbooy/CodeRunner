using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace CodeRunner.Resources.Controls
{
    public partial class Joystick : ContentView, INotifyPropertyChanged
    {
        public static readonly BindableProperty BigRadiusProperty =
            BindableProperty.Create(nameof(BigRadius), typeof(int), typeof(Joystick), 100, propertyChanged: (bindable, oldValue, NewValue) =>
            {
                if (bindable is Joystick instance)
                {
                    instance.BigCircle.WidthRequest = (int)NewValue;
                    instance.BigCircle.HeightRequest = (int)NewValue;
                }
            });

        public static readonly BindableProperty SmallRadiusProperty =
            BindableProperty.Create(nameof(SmallRadius), typeof(int), typeof(Joystick), 40, propertyChanged: (bindable, oldValue, NewValue) =>
            {
                if (bindable is Joystick instance)
                {
                    instance.SmallCircle.WidthRequest = (int)NewValue;
                    instance.SmallCircle.HeightRequest = (int)NewValue;
                }
            });

        public static readonly BindableProperty JoystickBackgroundProperty =
            BindableProperty.Create(nameof(JoystickBackground), typeof(Color), typeof(Joystick), Colors.Red, propertyChanged: (bindable, oldValue, NewValue) =>
            {
                if (bindable is Joystick instance)
                {
                    instance.BigCircle.Background = (Color)NewValue;
                    instance.SmallCircle.Background = (Color)NewValue;
                }
            });

        public static readonly BindableProperty DefaultVisibilityProperty =
            BindableProperty.Create(nameof(DefaultVisibility), typeof(bool), typeof(Joystick), true, propertyChanged: (bindable, oldValue, NewValue) =>
            {
                if (bindable is Joystick instance)
                {
                    instance.DefaultVisibility = (bool)NewValue;
                }
            });

        public int BigRadius
        {
            get => (int)GetValue(BigRadiusProperty);
            set => SetValue(BigRadiusProperty, value);
        }

        public int SmallRadius
        {
            get => (int)GetValue(SmallRadiusProperty);
            set => SetValue(SmallRadiusProperty, value);
        }

        public Color JoystickBackground
        {
            get => (Color)GetValue(JoystickBackgroundProperty);
            set => SetValue(JoystickBackgroundProperty, value);
        }

        public bool DefaultVisibility
        {
            get => (bool)GetValue(DefaultVisibilityProperty);
            set => SetValue(DefaultVisibilityProperty, value);
        }

        private PointF startingPosition;
        private PointF currentPosition;

        public event EventHandler<PointF> NavigationChanged;

        private PointF _navigation;
        public PointF Navigation
        {
            get => _navigation;
            private set
            {
                if (_navigation != value)
                {
                    _navigation = value;
                    NavigationChanged?.Invoke(this, _navigation); // Fire the event
                }
            }
        }


        // Bindable translations
        private double _smallCircleTranslationX, _smallCircleTranslationY, _bigCircleTranslationX, _bigCircleTranslationY;

        public double SmallCircleTranslationX { get => _smallCircleTranslationX; set { _smallCircleTranslationX = value; OnPropertyChanged(); } }
        public double SmallCircleTranslationY { get => _smallCircleTranslationY; set { _smallCircleTranslationY = value; OnPropertyChanged(); } }

        public double BigCircleTranslationX { get => _bigCircleTranslationX; set { _bigCircleTranslationX = value; OnPropertyChanged(); } }
        public double BigCircleTranslationY { get => _bigCircleTranslationY; set { _bigCircleTranslationY = value; OnPropertyChanged(); } }

        public Joystick()
        {
            InitializeComponent();
            BindingContext = this;

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
        //    if(!OperatingSystem.IsAndroid()) { return; }

            switch (e.StatusType)
            {

                case GestureStatus.Running:
                    currentPosition = new PointF(startingPosition.X + (float)e.TotalX, startingPosition.Y + (float)e.TotalY);

                    float dx = currentPosition.X - startingPosition.X;
                    float dy = currentPosition.Y - startingPosition.Y;

                    float distance = MathF.Sqrt(dx * dx + dy * dy);
                    float max = (BigRadius - SmallRadius) / 2;

                    if (distance > max)
                    {
                        // Normalize the vector and scale to the max radius
                        float scale = max / distance;
                        dx *= scale;
                        dy *= scale;
                    }

                    SmallCircleTranslationX = BigCircleTranslationX + dx;
                    SmallCircleTranslationY = BigCircleTranslationY + dy;

                    Navigation = new PointF(dx / max, dy / max); // Still normalized between -1 and 1
                    break;


                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    SmallCircleTranslationX = BigCircleTranslationX;
                    SmallCircleTranslationY = BigCircleTranslationY;
                    Navigation = new PointF(0, 0);
                    break;
            }
        }

        private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
        {
            // Record exact position inside the control
            var point = e.GetPosition(BigCircle);
            if (point is Point p)
            {
                startingPosition = new PointF((float)p.X, (float)p.Y);
                BigCircleTranslationX = p.X - BigRadius / 2;
                BigCircleTranslationY = p.Y - BigRadius / 2;

                SmallCircleTranslationX = p.X - SmallRadius / 2;
                SmallCircleTranslationY = p.Y - SmallRadius / 2;
            }
            Debug.WriteLine("KLIK");
        }


        // INotifyPropertyChanged implementation
        public new event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
