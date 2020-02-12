using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MPlayer.Commands
{
    public class MediaBehaviour : DependencyObject
    {
        private static DispatcherTimer timerTrack;

        #region DependencyProperties
        public static readonly DependencyProperty TimerTrackCommandProperty =
            DependencyProperty.RegisterAttached("TimerTrackCommand", typeof(ICommand), typeof(MediaBehaviour),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(TimerTrackCommandChanged)));

        public static readonly DependencyProperty MediaEndedCommandParameterProperty =
            DependencyProperty.RegisterAttached("MediaEndedCommandParameter", typeof(object), typeof(MediaBehaviour),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty MediaOpenedCommandProperty =
            DependencyProperty.RegisterAttached("MediaOpenedCommand", typeof(ICommand), typeof(MediaBehaviour),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(MediaOpenedCommandChanged)));

        public static readonly DependencyProperty MediaEndedCommandProperty =
            DependencyProperty.RegisterAttached("MediaEndedCommand", typeof(ICommand), typeof(MediaBehaviour),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(MediaEndedCommandChanged)));

        public static readonly DependencyProperty SliderValueChangedCommandProperty =
        DependencyProperty.RegisterAttached("SliderValueChangedCommand", typeof(ICommand), typeof(MediaBehaviour),
        new FrameworkPropertyMetadata(new PropertyChangedCallback(SliderValueChangedCommandChanged)));


        #endregion

        #region PropertyChangedCallbacks
        public static void TimerTrackCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is MediaElement mediaElement)
            {
                timerTrack = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(0.3),
                    Tag = mediaElement,
                };

                timerTrack.Tick += (sender, e) =>
                {
                    if (sender is DispatcherTimer dispatcherTimer && dispatcherTimer.Tag is MediaElement mElement)
                    {
                        var command = GetTimerTrackCommand(mElement);
                        command?.Execute(mediaElement.Position);
                    }
                };
            }
        }

        public static void MediaOpenedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is MediaElement mediaElement)
            {
                if (args.OldValue == null && args.NewValue != null)
                {
                    mediaElement.MediaOpened += MediaOpened;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    mediaElement.MediaOpened -= MediaOpened;
                }
            }
        }
        public static void MediaEndedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is MediaElement mediaElement)
            {
                if (args.OldValue == null && args.NewValue != null)
                {
                    mediaElement.MediaEnded += MediaEnded;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    mediaElement.MediaEnded -= MediaEnded;
                }
            }
        }

        public static void SliderValueChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is Slider slider)
            {
                if (args.OldValue == null && args.NewValue != null)
                {
                    slider.ValueChanged += SliderValueChanged;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    slider.ValueChanged -= SliderValueChanged;
                }
            }
        }
        #endregion

        #region Callbacks

        private static void MediaOpened(object sender, RoutedEventArgs e)
        {
            if (sender is MediaElement mediaElement)
            {
                var command = GetMediaOpenedCommand(mediaElement);

                if (command != null)
                {
                    e.Handled = true;
                    command.Execute(mediaElement.NaturalDuration);
                    timerTrack.Start();
                }
            }
        }

        private static void MediaEnded(object sender, RoutedEventArgs e)
        {
            if (sender is MediaElement mediaElement)
            {
                var command = GetMediaEndedCommand(mediaElement);
                var parameter = GetMediaEndedCommandParameter(mediaElement);

                if (command != null && command.CanExecute(parameter))
                {
                    e.Handled = true;
                    command.Execute(parameter);
                    timerTrack.Stop();
                }
            }
        }

        private static void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timerTrack.Tag is MediaElement mediaElement)
            {
                mediaElement.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        #endregion

        #region Getters/Setters
        public static void SetTimerTrackCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(TimerTrackCommandProperty, value);
        }
        public static ICommand GetTimerTrackCommand(UIElement element) => (ICommand)element.GetValue(TimerTrackCommandProperty);


        public static void SetMediaOpenedCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(MediaOpenedCommandProperty, value);
        }
        public static ICommand GetMediaOpenedCommand(UIElement element) => (ICommand)element.GetValue(MediaOpenedCommandProperty);


        public static void SetMediaEndedCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(MediaEndedCommandProperty, value);
        }
        public static ICommand GetMediaEndedCommand(UIElement element) => (ICommand)element.GetValue(MediaEndedCommandProperty);


        public static void SetMediaEndedCommandParameter(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MediaEndedCommandParameterProperty, value);
        }
        public static object GetMediaEndedCommandParameter(DependencyObject obj) => obj.GetValue(MediaEndedCommandParameterProperty);


        public static void SetSliderValueChangedCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(SliderValueChangedCommandProperty, value);
        }
        public static ICommand GetSliderValueChangedCommand(UIElement element) => (ICommand)element.GetValue(SliderValueChangedCommandProperty);
        #endregion
    }
}
