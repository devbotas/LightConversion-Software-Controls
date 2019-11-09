// Copyright 2019 Light Conversion, UAB
// Licensed under the Apache 2.0, see LICENSE.md for more details.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace LightConversion.Software.Controls {
    /// <summary>
    /// A control that acts mostly like popup, but has a customizable Anchor property, and may contain any controls. 
    /// Do not forget to include Generic.xaml theme for styling.
    /// </summary>
    public class PopupAction : UserControl {
        static PopupAction() {
            // This is not needed. However, user must include Generic.xaml theme in application resources for control to look as it should:
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupAction), new FrameworkPropertyMetadata(typeof(PopupAction)));
        }

        /// <summary>
        /// Gets or sets the anchor, which is launches a popup once mouse hovers over it.
        /// </summary>
        /// <value>The anchor control content.</value>
        public FrameworkElement Anchor {
            get { return (FrameworkElement)GetValue(AnchorProperty); }
            set { SetValue(AnchorProperty, value); }
        }

        private static readonly DependencyProperty AnchorProperty = DependencyProperty.Register("Anchor", typeof(FrameworkElement), typeof(PopupAction), new PropertyMetadata(null, AnchorChanged));

        private static void AnchorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs) {
            var myPopup = (PopupAction)dependencyObject;
            myPopup.Anchor = (FrameworkElement)eventArgs.NewValue;

            if (myPopup.Template == null) return;

            var grid = myPopup.Template.FindName("PART_Anchor", myPopup) as Grid;
            if (grid != null) {
                grid.Children.Clear();
                grid.Children.Add((FrameworkElement)eventArgs.NewValue);
            }
        }

        /// <summary>
        /// Internal use only switch. May have meaning of checked/unchecked Anchor.
        /// </summary>
        private bool _isToggled;

        private void HandleMouseLeftButtonUpEvent(object sender, MouseButtonEventArgs args) {
            var popup = Template.FindName("PART_Popup", this) as Popup;
            if (popup != null) {
                _isToggled = !_isToggled;
                popup.IsOpen = _isToggled;
            }
        }

        /// <inheritdoc />
        public override void OnApplyTemplate() {
            ApplyTemplate();

            var anchor = Template.FindName("PART_Anchor", this) as Grid;
            var popup = Template.FindName("PART_Popup", this) as Popup;

            if ((anchor != null) && (popup != null)) {
                anchor.MouseLeftButtonUp += HandleMouseLeftButtonUpEvent;
                popup.Closed += (sender, args) => {
                    if (anchor.IsMouseOver) return; // <-- Skip, HandleMouseUpEvent will occur.

                    _isToggled = false;
                };

                anchor.Children.Clear();
                if (Anchor != null) {
                    anchor.Children.Add(Anchor);
                } else {
                    anchor.Children.Add((Path)XamlReader.Parse("<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Data=\"M0.027999941,29.538998L27.204502,29.538998 28.626901,35.958527 30.412,33.839217 30.412,37.416998 0.027999941,37.416998z M27.377998,24.160999L35.200302,28.861491 32.589702,29.707208 35.369003,33.263178 33.984703,34.430001 31.240499,30.786629 29.345499,33.036674z M0,19.190999L30.439998,19.190999 30.439998,24.445254 29.106708,23.6444 29.106708,20.524389 1.3334911,20.524389 1.3334911,26.738409 26.583324,26.738409 26.878723,28.071999 0,28.071999z M0.027999941,9.8459988L30.412,9.8459988 30.412,17.724 0.027999941,17.724z M0.027999941,0L30.412,0 30.412,7.8780003 0.027999941,7.8780003z\" Stretch=\"Uniform\" Fill=\"#FF44474B\" Width=\"26\" Height=\"26\" Margin=\"0,0,0,0\" RenderTransformOrigin=\"0.5,0.5\"><Path.RenderTransform><TransformGroup><TransformGroup.Children><RotateTransform Angle=\"0\" /><ScaleTransform ScaleX=\"0.5\" ScaleY=\"0.5\" /></TransformGroup.Children></TransformGroup></Path.RenderTransform></Path>"));
                }
            }
        }
    }
}